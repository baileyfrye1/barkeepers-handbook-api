using api.DTOs.Cocktails;
using api.Models;

namespace api.Data;

public class CocktailManagementService
{
   private readonly IngredientService _ingredientService;

   public CocktailManagementService(IngredientService ingredientService, CocktailService cocktailService)
   {
      _ingredientService = ingredientService;
   }
   public async Task<Dictionary<string, Ingredient>> EnsureCocktailIngredientsExistAsync(CreateCocktailRequestDto cocktailRequestDto)
   {
      var ingredientMap = new Dictionary<string, Ingredient>();

      foreach (var cocktailIngredient in cocktailRequestDto.CocktailIngredients)
      {
         if (string.IsNullOrWhiteSpace(cocktailIngredient.Ingredient.Name))
         {
            throw new ArgumentException(
               "Error creating cocktail. Please provide ingredient name."
            );
         }

         if (
            !ingredientMap.TryGetValue(
               cocktailIngredient.Ingredient.Name,
               out var ingredient
            )
         )
         {
            ingredient = await _ingredientService.GetOneByNameAsync(
               cocktailIngredient.Ingredient.Name
            );

            if (ingredient == null)
            {
               var newIngredientModel = new Ingredient
               {
                  Name = cocktailIngredient.Ingredient.Name,
                  CreatedAt = DateTime.Now,
               };

               ingredient = await _ingredientService.AddOneAsync(newIngredientModel);
            }

            ingredientMap[cocktailIngredient.Ingredient.Name] = ingredient;
         }

         cocktailRequestDto.Tags.Add(cocktailIngredient.Ingredient.Name.ToLower());
      }

      return ingredientMap;
   }
   
   public List<CocktailIngredient> MapCocktailIngredients(
      Dictionary<string, Ingredient> ingredientMap, 
      Cocktail cocktail, 
      CreateCocktailRequestDto cocktailRequestDto
      )
   {
      var newCocktailIngredientsList = new List<CocktailIngredient>();

      foreach (var cocktailIngredientDto in cocktailRequestDto.CocktailIngredients)
      {
         var ingredient = ingredientMap[cocktailIngredientDto.Ingredient.Name];

         var unitValue = string.IsNullOrWhiteSpace(cocktailIngredientDto.Unit)
            ? "oz"
            : cocktailIngredientDto.Unit;

         var amountValue =
            cocktailIngredientDto.Amount != 0 ? cocktailIngredientDto.Amount : null;

         var newCocktailIngredientModel = new CocktailIngredient
         {
            CocktailId = cocktail.Id,
            IngredientId = ingredient.Id,
            Amount = amountValue,
            Unit = unitValue,
            CreatedAt = DateTime.Now,
         };

         newCocktailIngredientsList.Add(newCocktailIngredientModel);
      }
      return newCocktailIngredientsList;
   }
}