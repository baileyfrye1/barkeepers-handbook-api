using BarkeepersHandbook.Application.Models;
using BarkeepersHandbook.Contracts.DTOs.IngredientDTOs;

namespace BarkeepersHandbook.Api.Mappers
{
	public static class IngredientWrapper
	{
		public static IngredientDto ToIngredientDto(this Ingredient ingredientModel)
		{
			return new IngredientDto
			{
				Name = ingredientModel.Name,
			};
		}

		public static Ingredient ToIngredientFromDto(this IngredientDto ingredientDto)
		{
			return new Ingredient
			{

			};
		}
	}
}