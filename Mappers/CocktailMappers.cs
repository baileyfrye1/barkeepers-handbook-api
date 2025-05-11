using api.DTOs;
using api.DTOs.CocktailDTOs;
using api.DTOs.RatingDTOs;
using api.Models;

namespace api.Mappers
{
	public static class CocktailMappers
	{
		public static CocktailDto ToCocktailDto(this Cocktail cocktailModel)
		{
			return new CocktailDto
			{
				Id = cocktailModel.Id,
				Name = cocktailModel.Name,
				Featured = cocktailModel.Featured,
				Tags = cocktailModel.Tags,
				CocktailIngredients = cocktailModel.CocktailIngredients.Select(c => c.ToCocktailIngredientDto()).ToList(),
				RatingsData = new CocktailRatingObjectDto
				{
					Ratings = cocktailModel.Ratings.Select(r => r.ToCocktailRatingDto()).ToList()
				},
				CreatedAt = cocktailModel.CreatedAt,
				UpdatedAt = cocktailModel.UpdatedAt
			};
		}

		public static ReviewedCocktailDto ToCocktailReviewDto(this Cocktail cocktailModel)
		{
			return new ReviewedCocktailDto
			{
				Id = cocktailModel.Id,
				Name = cocktailModel.Name
			};
		}

		public static Cocktail ToCocktailFromCreateDto(this CreateCocktailRequestDto cocktailRequestDto)
		{
			return new Cocktail
			{
				Name = cocktailRequestDto.Name,
				Featured = cocktailRequestDto.Featured,
				Tags = cocktailRequestDto.Tags,
				// Image = cocktailRequestDto.Image,
				CreatedAt = DateTime.Now,
				UpdatedAt = DateTime.Now,
			};
		}
	}
}