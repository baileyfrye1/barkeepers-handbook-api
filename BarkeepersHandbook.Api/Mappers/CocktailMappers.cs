using BarkeepersHandbook.Application.Models;
using BarkeepersHandbook.Contracts.DTOs.CocktailDTOs;
using BarkeepersHandbook.Contracts.Requests;

namespace BarkeepersHandbook.Api.Mappers
{
	public static class CocktailMappers
	{
		public static CocktailDto ToCocktailDto(this Cocktail cocktailModel)
		{
			return new CocktailDto(
				cocktailModel.Id,
				cocktailModel.Name,
				cocktailModel.Featured,
				cocktailModel.Tags,
				cocktailModel.UserId,
				cocktailModel.CocktailIngredients.Select(c => c.ToCocktailIngredientDto()).ToList(),
				cocktailModel.ImageUrl,
				cocktailModel.Ratings.ToCocktailRatingsOverviewDto(),
				cocktailModel.CreatedAt,
				cocktailModel.UpdatedAt
				);
		}

		public static ReferenceCocktailDto ToReferenceCocktailDto(this Cocktail cocktailModel)
		{
			return new ReferenceCocktailDto
			{
				Id = cocktailModel.Id,
				Name = cocktailModel.Name
			};
		}

		public static Cocktail ToCocktailFromCreateDto(this CreateCocktailRequestDto cocktailRequestDto, string userId)
		{
			return new Cocktail
			{
				Name = cocktailRequestDto.Name,
				Featured = cocktailRequestDto.Featured,
				Tags = cocktailRequestDto.Tags ?? [],
				ImageUrl = string.Empty,
				UserId = userId,
				CocktailIngredients = cocktailRequestDto.CocktailIngredients.ToCocktailIngredientFromDtoList(),
				CreatedAt = DateTime.Now,
				UpdatedAt = DateTime.Now,
			};
		}

		public static Cocktail ToCocktailFromUpdateDto(this UpdateCocktailRequestDto updateCocktailDto, CocktailDto existing)
		{
			return new Cocktail
			{
					Name = updateCocktailDto.Name ?? existing.Name,
					Featured = updateCocktailDto.Featured ?? existing.Featured,
					Tags = updateCocktailDto.Tags ?? existing.Tags,
					CocktailIngredients = updateCocktailDto.CocktailIngredients.ToCocktailIngredientFromDtoList(),
					CreatedAt = existing.CreatedAt,
					UpdatedAt = DateTime.Now,
			};
		}
	}
}