using BarkeepersHandbook.Api.DTOs.CocktailDTOs;
using BarkeepersHandbook.Api.DTOs.IngredientDTOs;
using BarkeepersHandbook.Api.Services;
using BarkeepersHandbook.Api.Services.CocktailServices;
using BarkeepersHandbook.Api.Validators;
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
            services.AddScoped<IValidator<IngredientDto>, CreateIngredientValidator>();
            services.AddScoped<IRatingService, RatingService>();
            services.AddScoped<ICocktailImageService, CocktailImageService>();
            services.AddScoped<IFavoriteService, FavoriteService>();
            return services;
        }
    }
}