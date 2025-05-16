using System.Net.Mime;
using api.DTOs.CocktailDTOs;
using api.DTOs.IngredientDTOs;
using api.Services;
using api.Services.CocktailServices;
using api.Validators;
using FluentValidation;
using Supabase.Storage;
using Supabase.Storage.Interfaces;

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
            services.AddScoped<ICocktailService, CocktailService>();
            services.AddScoped<ICocktailIngredientService, CocktailIngredientService>();
            services.AddScoped<IIngredientService,IngredientService>();
            services.AddScoped<ICocktailManagementService, CocktailManagementService>();
            services.AddScoped<IValidator<CreateCocktailRequestDto>, CreateCocktailValidator>();
            services.AddScoped<IValidator<IngredientDto>, CreateIngredientValidator>();
            services.AddScoped<IRatingService, RatingService>();
            services.AddScoped<ICocktailImageService, CocktailImageService>();
            return services;
        }
    }
}