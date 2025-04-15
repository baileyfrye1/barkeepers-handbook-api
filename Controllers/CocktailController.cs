using api.Data;
using api.DTOs.Cocktails;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CocktailsController : ControllerBase
    {
        private readonly CocktailService _cocktailService;
        private readonly CocktailIngredientService _cocktailIngredientService;
        private readonly IngredientService _ingredientService;

        public CocktailsController(
            CocktailService cocktailService,
            CocktailIngredientService cocktailIngredientService,
            IngredientService ingredientService
        )
        {
            _cocktailService = cocktailService;
            _cocktailIngredientService = cocktailIngredientService;
            _ingredientService = ingredientService;
        }

        // TODO: Finish adding in query functionality
        // TODO: Use generic 'search' parameter for query parameter that would search cocktail names and tags and return whatever contains the search term in either the name or the tags
        [HttpGet]
        public async Task<IActionResult> GetAllCocktails([FromQuery] string? search)
        {
            var cocktailsDto = await _cocktailService.GetAllAsync(search);
            return Ok(cocktailsDto);
        }

        [HttpGet("featured")]
        public async Task<IActionResult> GetFeaturedCocktails()
        {
            var cocktailsDto = await _cocktailService.GetFeaturedAsync();
            return Ok(cocktailsDto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOneCocktailById([FromRoute] int id)
        {
            var result = await _cocktailService.GetOneByIdAsync(id);

            return result.Match<IActionResult>(
               c => Ok(c),
               _ => NotFound()
            );
        }

        // TODO: Refactor this endpoint to handle OneOf return from services class
        // TODO: Review this code to make sure it's efficient
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddCocktail(
            [FromBody] CreateCocktailRequestDto cocktailRequestDto
        )
        {
            if (!cocktailRequestDto.CocktailIngredients.Any())
            {
                return BadRequest(
                    "Error creating cocktail. Please provide cocktail ingredients."
                );
            }

            var ingredientMap = new Dictionary<string, Ingredient>();

            foreach (var cocktailIngredient in cocktailRequestDto.CocktailIngredients)
            {
                if (string.IsNullOrWhiteSpace(cocktailIngredient.Ingredient.Name))
                {
                    return BadRequest(
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

            var newCocktailResult = await _cocktailService.AddOneAsync(cocktailRequestDto);

            return await newCocktailResult.Match<IActionResult>(
                async dto =>
                {
                    var newCocktailIngredientsList = new List<CocktailIngredient> { };

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
                            CocktailId = dto.Id,
                            IngredientId = ingredient.Id,
                            Amount = amountValue,
                            Unit = unitValue,
                            CreatedAt = DateTime.Now,
                        };

                        newCocktailIngredientsList.Add(newCocktailIngredientModel);
                    }

                    var newCocktailIngredients = await _cocktailIngredientService.AddManyAsync(
                        newCocktailIngredientsList
                    );

                    return CreatedAtAction(nameof(GetOneCocktailById), new { id = dto.Id }, dto);
                },
                validate => ValidationProblem(validate.Errors),
                error => StatusCode(500, error.Message)
            );

        }

        // TODO: Review this code to make sure it's efficient
        [Authorize]
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> UpdateCocktail(
            [FromRoute] int id,
            [FromBody] UpdateCocktailRequestDto updateCocktailDto
        )
        {
            var cocktailResult = await _cocktailService.GetOneByIdAsync(id);

            return await cocktailResult.Match<Task<IActionResult>>(
                async dto =>
                {

                    if (string.IsNullOrWhiteSpace(updateCocktailDto.Name))
                    {
                        updateCocktailDto.Name = dto.Name;
                    }

                    if (!updateCocktailDto.Featured.HasValue)
                    {
                        updateCocktailDto.Featured = dto.Featured;
                    }

                    if (updateCocktailDto.Tags.Count == 0)
                    {
                        updateCocktailDto.Tags = dto.Tags;
                    }

                    var cocktailModel = new Cocktail
                    {
                        Name = updateCocktailDto.Name,
                        Featured = updateCocktailDto.Featured.Value,
                        Tags = updateCocktailDto.Tags,
                        CocktailIngredients = updateCocktailDto.CocktailIngredients,
                        CreatedAt = dto.CreatedAt,
                        UpdatedAt = DateTime.Now,
                    };

                    var updatedCocktail = await _cocktailService.UpdateOneAsync(id, cocktailModel);

                    return updatedCocktail.Match<IActionResult>(
                        updated => Ok(updated),
                        nf => NotFound(),
                        validate => BadRequest(validate.Errors)
                    );
                },
                _ => Task.FromResult<IActionResult>(NotFound())
            );

        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCocktail([FromRoute] int id)
        {
            var result = await _cocktailService.DeleteOneAsync(id);

            return result.Match<IActionResult>(
                c => NoContent(),
                _ => NotFound()
            );
        }
    }
}
