using BarkeepersHandbook.Application.Models;
using BarkeepersHandbook.Application.Services;
using BarkeepersHandbook.Application.Services.CocktailServices;
using BarkeepersHandbook.Application.Validators;
using BarkeepersHandbook.Contracts.DTOs.IngredientDTOs;
using BarkeepersHandbook.Contracts.Requests;
using FluentValidation;

namespace BarkeepersHandbook.Api.Extensions
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
            services.AddScoped<IValidator<Ingredient>, CreateIngredientValidator>();
            services.AddScoped<IRatingService, RatingService>();
            services.AddScoped<ICocktailImageService, CocktailImageService>();
            services.AddScoped<IFavoriteService, FavoriteService>();
            return services;
        }
    }
}