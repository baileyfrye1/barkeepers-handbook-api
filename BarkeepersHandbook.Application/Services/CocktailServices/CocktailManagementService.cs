using BarkeepersHandbook.Application.Models;
using FluentValidation;

namespace BarkeepersHandbook.Application.Services.CocktailServices;

public class CocktailManagementService : ICocktailManagementService
{
   private readonly IIngredientService _ingredientService;
   private readonly ICocktailIngredientService _cocktailIngredientService;
   
   public CocktailManagementService(IIngredientService ingredientService, ICocktailIngredientService cocktailIngredientService)
   {
      _ingredientService = ingredientService;
      _cocktailIngredientService = cocktailIngredientService;
   }
   public async Task<Dictionary<string, Ingredient>> EnsureCocktailIngredientsExistAsync(Cocktail cocktailModel)
   {
      var ingredientMap = new Dictionary<string, Ingredient>();

      foreach (var cocktailIngredient in cocktailModel.CocktailIngredients)
      {
         if (string.IsNullOrWhiteSpace(cocktailIngredient.Ingredient.Name))
         {
            throw new ArgumentException(
               "Ingredient name cannot be empty"
            );
         }

         if (ingredientMap.TryGetValue(
                cocktailIngredient.Ingredient.Name,
                out var ingredient
             )) continue;
         
         var getByNameResult = await _ingredientService.GetOneByNameAsync(cocktailIngredient.Ingredient.Name);

         if (getByNameResult.TryPickT0(out var foundIngredient, out _))
         {
            ingredient = foundIngredient;
         }
         else
         {
            var newIngredientModel = new Ingredient
            {
               Name = cocktailIngredient.Ingredient.Name,
               CreatedAt = DateTime.Now,
            };
               
            var addIngredientResult = await _ingredientService.AddOneAsync(newIngredientModel);
               
            addIngredientResult.Switch(
               i => ingredient = i,
               vf => throw new ValidationException(string.Join(", ", vf.Errors)),
               error => throw new InvalidOperationException(error.Details)
            );
         }
            
         if (ingredient != null)
         {
            ingredientMap[cocktailIngredient.Ingredient.Name] = ingredient;
         }
      }
      
      return ingredientMap;
   }
   
   public List<CocktailIngredient> MapCocktailIngredients(
      Dictionary<string, Ingredient> ingredientMap, 
      Cocktail cocktailModel
      )
   {
      var newCocktailIngredientsList = new List<CocktailIngredient>();

      foreach (var cocktailIngredient in cocktailModel.CocktailIngredients)
      {
         var ingredient = ingredientMap[cocktailIngredient.Ingredient.Name];

         var unitValue = string.IsNullOrWhiteSpace(cocktailIngredient.Unit)
            ? "oz"
            : cocktailIngredient.Unit;

         var amountValue =
            cocktailIngredient.Amount != 0 ? cocktailIngredient.Amount : null;

         var newCocktailIngredientModel = new CocktailIngredient
         {
            CocktailId = cocktailModel.Id,
            IngredientId = ingredient.Id,
            Amount = amountValue,
            Unit = unitValue,
            CreatedAt = DateTime.Now,
         };

         newCocktailIngredientsList.Add(newCocktailIngredientModel);
      }
      return newCocktailIngredientsList;
   }

   public async Task AddCocktailIngredients(Cocktail cocktailModel)
   {
      var ingredientMap = await EnsureCocktailIngredientsExistAsync(cocktailModel);
        
      var newCocktailIngredientsList = MapCocktailIngredients(ingredientMap, cocktailModel);
        
      await _cocktailIngredientService.AddManyAsync(
         newCocktailIngredientsList
      );
   }
}

public interface ICocktailManagementService
{
   Task<Dictionary<string, Ingredient>> EnsureCocktailIngredientsExistAsync(Cocktail cocktailModel);
   List<CocktailIngredient> MapCocktailIngredients(
      Dictionary<string, Ingredient> ingredientMap,
      Cocktail cocktailModel
   );

   Task AddCocktailIngredients(Cocktail cocktailModel);
}