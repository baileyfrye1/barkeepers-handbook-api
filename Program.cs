using System.Text;
using api.Extensions;
using api.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Supabase;

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

var bytes = Encoding.UTF8.GetBytes(jwtSecret);

builder
    .Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(bytes),
            ValidAudience = builder.Configuration["Authentication:ValidAudience"],
            ValidIssuer = builder.Configuration["Authentication:ValidIssuer"],
            ValidateLifetime = true,
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["access_token"];
                return Task.CompletedTask;
            }
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
                .WithOrigins("http://localhost:3000", "https://barkeepers-handbook-frontend.pages.dev")
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

var app = builder.Build();

app.UseGlobalErrorHandling();

app.UseCors(myAllowSpecificOrigins);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
