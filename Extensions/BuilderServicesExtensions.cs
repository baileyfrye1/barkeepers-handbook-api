using api.Data;
using api.DTOs.Cocktails;
using api.Validators;
using FluentValidation;

namespace api.Extensions
{
    public static class BuilderServicesExtensions
    {
        public static IServiceCollection AddGlobalErrorHandling(this IServiceCollection services)
        {
            services.AddProblemDetails();
            return services;
        }

        public static IServiceCollection AddDependencies(this IServiceCollection services)
        {
            services.AddScoped<CocktailService>();
            services.AddScoped<CocktailIngredientService>();
            services.AddScoped<IngredientService>();
            services.AddScoped<CocktailManagementService>();
            services.AddScoped<IValidator<CreateCocktailRequestDto>, CreateCocktailValidator>();
            return services;
        }
    }
}