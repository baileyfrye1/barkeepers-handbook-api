using BarkeepersHandbook.Api.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Supabase;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGlobalErrorHandling();

builder
    .Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft
            .Json
            .ReferenceLoopHandling
            .Ignore;
    });

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

var jwtSecret = builder.Configuration["Authentication:JwtSecret"];

if (jwtSecret is null)
{
    throw new InvalidOperationException("JwtSecret is missing");
}

var issuer = builder.Environment.IsDevelopment() ? "https://simple-phoenix-47.clerk.accounts.dev" : "https://clerk.barkeepershandbook.com";

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = issuer;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateLifetime = true,
            ValidateAudience = false,
        };
    });

builder.Services.AddControllers();

const string myAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: myAllowSpecificOrigins,
        policy =>
        {
            policy
                .WithOrigins("http://localhost:3000", "https://barkeepers-handbook-frontend.pages.dev", "https://barkeepershandbook.com")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        }
    );
});

builder.Services.AddOpenApi();

// Add Dependency Injection
builder.Services.AddDependencies();

// Load Supabase URL and Key from appsettings.json or environment variables
var supabaseUrl = builder.Configuration["Supabase:Url"];
var supabaseKey = builder.Configuration["Supabase:Key"];

// Initialize Supabase client
var options = new SupabaseOptions { AutoConnectRealtime = true, AutoRefreshToken = true };
var supabase = new Client(supabaseUrl, supabaseKey, options);

// Register Supabase as a singleton service
builder.Services.AddSingleton(supabase);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear(); // Clear defaults so it trusts all
    options.KnownProxies.Clear();
});

var app = builder.Build();

// Trust forwarded headers from proxies (like Fly.io)

app.UseForwardedHeaders();

app.UseGlobalErrorHandling();

app.UseCors(myAllowSpecificOrigins);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
    app.MapOpenApi();
    app.MapScalarApiReference();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
