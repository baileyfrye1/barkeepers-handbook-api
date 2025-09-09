using BarkeepersHandbook.Api.Mappers;
using BarkeepersHandbook.Application.Models;
using BarkeepersHandbook.Contracts.DTOs.CocktailIngredientDTOs;
using BarkeepersHandbook.Contracts.DTOs.IngredientDTOs;
using BarkeepersHandbook.Contracts.Requests;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace BarkeepersHandbook.Tests.Mappers;

public class CocktailMappersTests
{
   public class ToCocktailFromCreateDtoTests : CocktailMappersTests
   {
      [Fact]
      public void ValidDtoWithNullTags_ReturnsCocktailModelWithGeneratedTags()
      {
         // Arrange
         var imageMock = new Mock<IFormFile>();
         List<CocktailIngredientDto> cocktailIngredients = [
            new()
            {
               Ingredient = new IngredientDto
               {
                  Name = "Light Rum",
               },
               Amount = 2,
               Unit = "oz"
            },
            new()
            {
               Ingredient = new IngredientDto
               {
                  Name = "Lime Juice",
               },
               Amount = 0.75,
               Unit = "oz"
            },
            new()
            {
               Ingredient = new IngredientDto
               {
                  Name = "Simple Syrup",
               },
               Amount = 0.75,
               Unit = "oz"
            },
         ];
         
         var sut = new CreateCocktailRequestDto(Name: "Daiquiri", Featured: true, Tags: null,
            CocktailIngredients: cocktailIngredients, Image: imageMock.Object);
         const string userId = "user_2wohmk3B6o2pcGAAdXfetQB670K";

         // Act
         var result = sut.ToCocktailFromCreateDto(userId);

         // Assert
         result.Should().BeOfType<Cocktail>();
         result.Featured.Should().BeTrue();
         result.UserId.Should().Be(userId);
         result.Tags.Should().BeOfType<HashSet<string>>();
         result.Tags.Should().BeEmpty();
      }
      
      [Fact]
      public void ValidDtoWithProvidedTags_ReturnsCocktailModelWithProvidedTags()
      {
         // Arrange
         var imageMock = new Mock<IFormFile>();
         List<CocktailIngredientDto> cocktailIngredients = [
            new()
            {
               Ingredient = new IngredientDto
               {
                  Name = "Light Rum",
               },
               Amount = 2,
               Unit = "oz"
            },
            new()
            {
               Ingredient = new IngredientDto
               {
                  Name = "Lime Juice",
               },
               Amount = 0.75,
               Unit = "oz"
            },
            new()
            {
               Ingredient = new IngredientDto
               {
                  Name = "Simple Syrup",
               },
               Amount = 0.75,
               Unit = "oz"
            },
         ];

         HashSet<string> tags = ["classic", "sweet"];
         
         var sut = new CreateCocktailRequestDto(Name: "Daiquiri", Featured: true, Tags: tags,
            CocktailIngredients: cocktailIngredients, Image: imageMock.Object);
         const string userId = "user_2wohmk3B6o2pcGAAdXfetQB670K";

         // Act
         var result = sut.ToCocktailFromCreateDto(userId);

         // Assert
         result.Should().BeOfType<Cocktail>();
         result.Featured.Should().BeTrue();
         result.UserId.Should().Be(userId);
         result.Tags.Should().BeEquivalentTo(new List<string> { "classic", "sweet" });
         result.Tags.Should().HaveCount(2);
      }
   }
}