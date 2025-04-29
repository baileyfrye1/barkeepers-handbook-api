using api.DTOs.IngredientDTOs;
using api.Models;

namespace api.Mappers
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