using BarkeepersHandbook.Application.Models;
using BarkeepersHandbook.Application.Services;
using BarkeepersHandbook.Application.Services.CocktailServices;
using FluentAssertions;
using Moq;
using OneOf;
using OneOf.Types;
using Xunit.Abstractions;

namespace BarkeepersHandbook.Tests.Services;

public class CocktailManagementServiceTests
{
    private readonly Mock<IIngredientService> _ingredientServiceMock;
   private readonly Mock<ICocktailIngredientService> _cocktailIngredientServiceMock;
   private readonly CocktailManagementService _cocktailManagementService;

   public CocktailManagementServiceTests(ITestOutputHelper testOutputHelper)
   {
       _ingredientServiceMock = new Mock<IIngredientService>();
      _cocktailIngredientServiceMock = new Mock<ICocktailIngredientService>();
      _cocktailManagementService = new CocktailManagementService(_ingredientServiceMock.Object, _cocktailIngredientServiceMock.Object);
   }

   [Fact]
   public async Task EnsureCocktailIngredientsExistAsync_AllIngredientsExist_ReturnsIngredientDictionary()
   {
     // Arrange
     List<CocktailIngredient> cocktailIngredients =
     [
         new CocktailIngredient
         {
             Id = 1,
             CocktailId = 1,
             IngredientId = 1,
             Ingredient = new Ingredient { Id = 1, Name = "Light Rum", CreatedAt = DateTime.Now },
             Amount = 2,
             Unit = "oz",
             CreatedAt = DateTime.Now,
         },

         new CocktailIngredient
         {
             Id = 2,
             CocktailId = 1,
             IngredientId = 2,
             Ingredient = new Ingredient { Id = 2, Name = "Lime Juice", CreatedAt = DateTime.Now },
             Amount = 0.75,
             Unit = "oz",
             CreatedAt = DateTime.Now,
         },

         new CocktailIngredient
         {
             Id = 3,
             CocktailId = 1,
             IngredientId = 3,
             Ingredient = new Ingredient { Id = 3, Name = "Simple Syrup", CreatedAt = DateTime.Now },
             Amount = 0.75,
             Unit = "oz",
             CreatedAt = DateTime.Now,
         }

     ];
     
     var cocktail = new Cocktail
     {
         Id = 1,
         Name = "Daiquiri",
         Featured = true,
         UserId = Guid.NewGuid().ToString(),
         Tags = ["lime juice", "light rum", "simple syrup", "classic"],
         ImageUrl = "https://fakeimageurl.com/image.png",
         CocktailIngredients = cocktailIngredients,
         CreatedAt = DateTime.Now,
         UpdatedAt = DateTime.Now,
     };
     foreach (var ci in cocktailIngredients)
     {
         _ingredientServiceMock.Setup(i => i.GetOneByNameAsync(ci.Ingredient.Name))
             .ReturnsAsync(OneOf<Ingredient, NotFound>.FromT0(ci.Ingredient));
     }

     // Act
     var result = await _cocktailManagementService.EnsureCocktailIngredientsExistAsync(cocktail);

     // Assert
     result.Should().NotBeNull();
     result.Should().ContainKey("Light Rum");
     result.Should().ContainKey("Lime Juice");
     result.Should().ContainKey("Simple Syrup");
     result["Light Rum"].Should().BeOfType<Ingredient>();
     result["Lime Juice"].Should().BeOfType<Ingredient>();
     result["Simple Syrup"].Should().BeOfType<Ingredient>();
   }
}