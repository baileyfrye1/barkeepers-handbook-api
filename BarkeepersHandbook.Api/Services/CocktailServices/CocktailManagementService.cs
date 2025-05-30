using BarkeepersHandbook.Api.DTOs.CocktailDTOs;
using BarkeepersHandbook.Api.Errors;
using BarkeepersHandbook.Api.Models;
using BarkeepersHandbook.Api.Validators;

namespace BarkeepersHandbook.Api.Services.CocktailServices;

public class CocktailManagementService : ICocktailManagementService
{
   private readonly IIngredientService _ingredientService;
   private readonly ICocktailIngredientService _cocktailIngredientService;

   public CocktailManagementService(IIngredientService ingredientService, ICocktailIngredientService cocktailIngredientService)
   {
      _ingredientService = ingredientService;
      _cocktailIngredientService = cocktailIngredientService;
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
            var getByNameResult = await _ingredientService.GetOneByNameAsync(cocktailIngredient.Ingredient.Name);

            if (getByNameResult.TryPickT0(out var foundIngredient, out var notFound))
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
                  vf => new ValidationFailed(vf.Errors),
                  error => new UnexpectedError("Ingredient could not be added.", error.Details)
               );
            }
            
            if (ingredient != null)
            {
               ingredientMap[cocktailIngredient.Ingredient.Name] = ingredient;
            } 
         }

         cocktailRequestDto.Tags.Add(cocktailIngredient.Ingredient.Name.ToLower());
      }
      
      return ingredientMap;
   }
   
   public List<CocktailIngredient> MapCocktailIngredients(
      Dictionary<string, Ingredient> ingredientMap, 
      Cocktail cocktailModel, 
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

   public async Task AddCocktailIngredients(CreateCocktailRequestDto cocktailRequestDto, Cocktail cocktailModel)
   {
      var ingredientMap = await EnsureCocktailIngredientsExistAsync(cocktailRequestDto);
        
      var newCocktailIngredientsList = MapCocktailIngredients(ingredientMap, cocktailModel, cocktailRequestDto);
        
      await _cocktailIngredientService.AddManyAsync(
         newCocktailIngredientsList
      );
   }
}

public interface ICocktailManagementService
{
   Task<Dictionary<string, Ingredient>> EnsureCocktailIngredientsExistAsync(CreateCocktailRequestDto cocktailRequestDto);
   List<CocktailIngredient> MapCocktailIngredients(
      Dictionary<string, Ingredient> ingredientMap,
      Cocktail cocktail,
      CreateCocktailRequestDto cocktailRequestDto
   );

   Task AddCocktailIngredients(CreateCocktailRequestDto cocktailRequestDto, Cocktail cocktailModel);
}