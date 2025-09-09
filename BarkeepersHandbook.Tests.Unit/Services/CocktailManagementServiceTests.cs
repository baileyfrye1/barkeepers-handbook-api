using BarkeepersHandbook.Application.Errors;
using BarkeepersHandbook.Application.Models;
using BarkeepersHandbook.Application.Services;
using BarkeepersHandbook.Application.Services.CocktailServices;
using BarkeepersHandbook.Application.Validators;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using OneOf;
using OneOf.Types;

namespace BarkeepersHandbook.Tests.Services;

public class CocktailManagementServiceTests
{
    private readonly Mock<IIngredientService> _ingredientServiceMock;
   private readonly Mock<ICocktailIngredientService> _cocktailIngredientServiceMock;
   private readonly CocktailManagementService _cocktailManagementService;

   protected CocktailManagementServiceTests()
   { 
       _ingredientServiceMock = new Mock<IIngredientService>();
       _cocktailIngredientServiceMock = new Mock<ICocktailIngredientService>();
       _cocktailManagementService = new CocktailManagementService(_ingredientServiceMock.Object, _cocktailIngredientServiceMock.Object);
   }

   public class EnsureCocktailIngredientsExistAsyncTests : CocktailManagementServiceTests
   {
       [Theory]
       [InlineData("Light Rum", "Lime Juice", "Simple Syrup")]
       [InlineData("Light Rum", "Lime Juice", "Lime Juice")]
       public async Task AllIngredientsExist_ReturnsIngredientDictionary(params string[] ingredientNames)
       {
         // Arrange
         var cocktailIngredients = ingredientNames.Select((name, i) =>
                 new CocktailIngredient
                 {
                     Id = i + 1,
                     CocktailId = 1,
                     IngredientId = i + 1,
                     Ingredient = new Ingredient { Id = i + 1, Name = name, CreatedAt = DateTime.Now },
                     Amount = 2,
                     Unit = "oz",
                     CreatedAt = DateTime.Now,
                 }
         ).ToList();
         
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
         result.Should().ContainKeys(ingredientNames.Distinct());
       }
       
       [Fact]
       public async Task IngredientNotInDB_ReturnsIngredientDictionary()
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
                 .ReturnsAsync(OneOf<Ingredient, NotFound>.FromT1(new NotFound()));
             _ingredientServiceMock.Setup(i => i.AddOneAsync(It.IsAny<Ingredient>())).ReturnsAsync(OneOf<Ingredient, ValidationFailed, UnexpectedError>.FromT0(ci.Ingredient));
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

       [Theory]
       [InlineData(null)]
       [InlineData("")]
       [InlineData("    ")]
       public async Task IngredientNameIsInvalid_ThrowsArgumentException(string? invalidIngredientName)
       {
           // Arrange
           List<CocktailIngredient> cocktailIngredients =
           [
               new CocktailIngredient
               {
                   Id = 1,
                   CocktailId = 1,
                   IngredientId = 1,
                   Ingredient = new Ingredient { Id = 1, Name = invalidIngredientName, CreatedAt = DateTime.Now },
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
               Tags = ["lime juice", "simple syrup", "classic"],
               ImageUrl = "https://fakeimageurl.com/image.png",
               CocktailIngredients = cocktailIngredients,
               CreatedAt = DateTime.Now,
               UpdatedAt = DateTime.Now,
           };
           
           // Act
           var result = () => _cocktailManagementService.EnsureCocktailIngredientsExistAsync(cocktail);
       
           // Assert
           await result.Should().ThrowAsync<ArgumentException>();
       }

       [Fact]
       public async Task AddOneAsyncValidationError_ThrowsValidationException()
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
                 .ReturnsAsync(OneOf<Ingredient, NotFound>.FromT1(new NotFound()));
             _ingredientServiceMock.Setup(i => i.AddOneAsync(It.IsAny<Ingredient>())).ReturnsAsync(OneOf<Ingredient, ValidationFailed, UnexpectedError>.FromT1(new ValidationFailed(new ValidationFailure())));
         }
           
           // Act
           var result = () => _cocktailManagementService.EnsureCocktailIngredientsExistAsync(cocktail);
       
           // Assert
           await result.Should().ThrowAsync<ValidationException>();
       }
       
       [Fact]
       public async Task AddOneAsyncUnexpectedError_ThrowsInvalidOperationException()
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
                 .ReturnsAsync(OneOf<Ingredient, NotFound>.FromT1(new NotFound()));
             _ingredientServiceMock.Setup(i => i.AddOneAsync(It.IsAny<Ingredient>())).ReturnsAsync(OneOf<Ingredient, ValidationFailed, UnexpectedError>.FromT2(new UnexpectedError("Unexpected Error")));
         }
           
           // Act
           var result = () => _cocktailManagementService.EnsureCocktailIngredientsExistAsync(cocktail);
       
           // Assert
           await result.Should().ThrowAsync<InvalidOperationException>();
       }
   }

   public class MapCocktailIngredientsTests : CocktailManagementServiceTests
   {
       [Fact]
       public void HealthyIngredientDictionary_ReturnsIngredientList()
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

             var ingredientDict = cocktailIngredients.ToDictionary(ci => ci.Ingredient.Name, ci => ci.Ingredient);
           // Act
           var result = _cocktailManagementService.MapCocktailIngredients(ingredientDict, cocktail);

           // Assert
           result.Should().NotBeNull();
           result.Should().HaveCount(cocktail.CocktailIngredients.Count);
       }
   }
}