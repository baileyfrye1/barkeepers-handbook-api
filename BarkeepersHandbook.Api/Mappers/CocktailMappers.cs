using BarkeepersHandbook.Application.Models;
using BarkeepersHandbook.Contracts.DTOs.CocktailDTOs;

namespace BarkeepersHandbook.Api.Mappers
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
				ImageUrl = cocktailModel.ImageUrl,
				CreatedAt = cocktailModel.CreatedAt,
				UpdatedAt = cocktailModel.UpdatedAt
			};
		}

		public static ReferenceCocktailDto ToCocktailReviewDto(this Cocktail cocktailModel)
		{
			return new ReferenceCocktailDto
			{
				Id = cocktailModel.Id,
				Name = cocktailModel.Name
			};
		}

		public static Cocktail ToCocktailFromCreateDto(this CreateCocktailRequestDto cocktailRequestDto, string imageUrl, string userId, List<string> allTags)
		{
			return new Cocktail
			{
				Name = cocktailRequestDto.Name,
				Featured = cocktailRequestDto.Featured,
				Tags = allTags,
				ImageUrl = imageUrl,
				UserId = userId,
				CreatedAt = DateTime.Now,
				UpdatedAt = DateTime.Now,
			};
		}
	}
}