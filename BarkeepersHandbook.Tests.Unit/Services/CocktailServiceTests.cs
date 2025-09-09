using System.Net;
using BarkeepersHandbook.Application.Models;
using BarkeepersHandbook.Application.Services;
using BarkeepersHandbook.Application.Services.CocktailServices;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Supabase.Postgrest;
using Supabase.Postgrest.Responses;

namespace BarkeepersHandbook.Tests.Services;

public class CocktailServiceTests
{
    // Mocks
    private readonly Mock<ICocktailImageService> _imageService;
    private readonly Mock<ICocktailManagementService> _cocktailManagementService;
    private readonly Mock<ISupabaseClientService<Cocktail>> _supabase;
    private readonly Mock<IRatingService> _ratingService;
    private readonly Mock<ILogger<CocktailService>> _logger;
    
    
    private readonly CocktailService _cocktailService;

    protected CocktailServiceTests()
    {
        // Mocks
        _imageService = new Mock<ICocktailImageService>();
        _cocktailManagementService = new Mock<ICocktailManagementService>();
        _supabase = new Mock<ISupabaseClientService<Cocktail>>();
        _ratingService = new Mock<IRatingService>();
        _logger = new Mock<ILogger<CocktailService>>();
        
        
        _cocktailService = new CocktailService(
            _supabase.Object,
            _ratingService.Object,
            _logger.Object,
            _imageService.Object,
            _cocktailManagementService.Object
        );
    }

    public class CreateCocktailAsyncTests : CocktailServiceTests
    {
        [Fact]
        public async Task ValidCocktailModelWithImage_ReturnsCreatedCocktail()
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
                Tags = ["classic"],
                ImageUrl = "https://fakeimageurl.com/image.png",
                CocktailIngredients = cocktailIngredients,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            var imageMock = new Mock<IFormFile>();

            _imageService.Setup(i => i.UploadImage(imageMock.Object)).ReturnsAsync("https://vqrfdghklwccdbvqmntd.supabase.co/storage/v1/object/public/cocktail-images/15c4a158-25ec-42fa-9a56-4d329f217802.jpg");
            _supabase.Setup(s => s.InsertAsync(It.IsAny<Cocktail>()))
                .ReturnsAsync((Cocktail c) =>
                {
                    var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
                    { 
                        RequestMessage = new HttpRequestMessage(HttpMethod.Post, "https://example.supabase.co/rest/v1/cocktails")
                    };
                    var jsonObj = JsonConvert.SerializeObject(new[] { c });
                    var baseResponse = new BaseResponse(new ClientOptions(), httpResponse, jsonObj);
                    return new ModeledResponse<Cocktail>(baseResponse, new JsonSerializerSettings());
                });
            
            // Act
            var result = await _cocktailService.CreateCocktailAsync(cocktail, imageMock.Object);

            // Assert
            result.Should().NotBeNull();
            result.AsT0.Should().BeOfType<Cocktail>();
            result.AsT0.Tags.Should().BeEquivalentTo("lime juice",  "light rum",  "simple syrup", "classic");
        }
    }
}