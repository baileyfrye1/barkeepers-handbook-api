using api.DTOs.Cocktails;
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
				CreatedAt = cocktailModel.CreatedAt,
				UpdatedAt = cocktailModel.UpdatedAt
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