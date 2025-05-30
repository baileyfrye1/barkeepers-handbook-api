using BarkeepersHandbook.Api.DTOs.CocktailIngredientDTOs;
using BarkeepersHandbook.Api.Models;

namespace BarkeepersHandbook.Api.Mappers
{
	public static class CocktailIngredientWrapper
	{
		public static CocktailIngredientDto ToCocktailIngredientDto(this CocktailIngredient cocktailIngredientModel)
		{
			return new CocktailIngredientDto
			{
				Ingredient = cocktailIngredientModel.Ingredient.ToIngredientDto(),
				Amount = cocktailIngredientModel.Amount,
				Unit = cocktailIngredientModel.Unit,
			};
		}

		public static CocktailIngredient ToCocktailIngredientFromDto(this CocktailIngredientDto cocktailIngredientDto)
		{
			return new CocktailIngredient
			{
				Ingredient = cocktailIngredientDto.Ingredient.ToIngredientFromDto(),
				Amount = cocktailIngredientDto.Amount,
				Unit = cocktailIngredientDto.Unit,
			};
		}
	}
}