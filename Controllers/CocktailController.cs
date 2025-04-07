using api.Data;
using api.DTOs.Cocktails;
// using api.Helpers;
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
            try
            {
                var cocktailsDto = await _cocktailService.GetAllAsync(search);
                return Ok(cocktailsDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpGet("featured")]
        public async Task<IActionResult> GetFeaturedCocktails()
        {
            try
            {
                var cocktailsDto = await _cocktailService.GetFeaturedAsync();
                return Ok(cocktailsDto);
            }
            catch (Exception ex)
            {
                return StatusCode(400, $"Error fetching cocktails: {ex.Message}");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOneCocktailById([FromRoute] int id)
        {
            try
            {
                var cocktailDto = await _cocktailService.GetOneByIdAsync(id);

                if (cocktailDto == null)
                {
                    return NotFound();
                }

                return Ok(cocktailDto);
            }
            catch (Exception ex)
            {
                return StatusCode(400, $"Error fetching cocktails: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddCocktail(
            [FromBody] CreateCocktailRequestDto cocktailRequestDto
        )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Please enter all fields.");
            }

            try
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

                var newCocktail = await _cocktailService.AddOneAsync(cocktailRequestDto);

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
                        CocktailId = newCocktail.Id,
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

                return StatusCode(201, newCocktail);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error while adding cocktail: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> UpdateCocktail(
            [FromRoute] int id,
            [FromBody] UpdateCocktailRequestDto updateCocktailDto
        )
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var cocktailDto = await _cocktailService.GetOneByIdAsync(id);

            if (cocktailDto == null)
            {
                return BadRequest();
            }

            if (string.IsNullOrWhiteSpace(updateCocktailDto.Name))
            {
                updateCocktailDto.Name = cocktailDto.Name;
            }

            if (!updateCocktailDto.Featured.HasValue)
            {
                updateCocktailDto.Featured = cocktailDto.Featured;
            }

            if (updateCocktailDto.Tags.Count == 0)
            {
                updateCocktailDto.Tags = cocktailDto.Tags;
            }

            var cocktailModel = new Cocktail
            {
                Name = updateCocktailDto.Name,
                Featured = updateCocktailDto.Featured.Value,
                Tags = updateCocktailDto.Tags,
                CocktailIngredients = updateCocktailDto.CocktailIngredients,
                CreatedAt = cocktailDto.CreatedAt,
                UpdatedAt = DateTime.Now,
            };

            var updatedCocktail = await _cocktailService.UpdateOneAsync(id, cocktailModel);

            return Ok(updatedCocktail);
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCocktail([FromRoute] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                    BadRequest();

                var cocktail = await _cocktailService.GetOneByIdAsync(id);

                if (cocktail == null)
                {
                    return NotFound();
                }

                _cocktailService.DeleteOneAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(404, ex);
            }
        }
    }
}
