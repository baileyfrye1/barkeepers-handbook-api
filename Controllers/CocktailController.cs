using api.DTOs.CocktailDTOs;
using api.Mappers;
using api.Models;
using api.Services;
using api.Services.CocktailServices;
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
        private readonly CocktailManagementService _cocktailManagementService;

        public CocktailsController(
            CocktailService cocktailService,
            CocktailIngredientService cocktailIngredientService,
            IngredientService ingredientService,
            CocktailManagementService cocktailManagementService
        )
        {
            _cocktailService = cocktailService;
            _cocktailIngredientService = cocktailIngredientService;
            _ingredientService = ingredientService;
            _cocktailManagementService = cocktailManagementService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCocktails([FromQuery] string? search, int page)
        {
            var cocktailsDto = await _cocktailService.GetAllAsync(search, page);
            return Ok(new { cocktailsDto.Cocktails, cocktailsDto.TotalCount});
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
        
            var ingredientMap = await _cocktailManagementService.EnsureCocktailIngredientsExistAsync(cocktailRequestDto);
        
            var newCocktailResult = await _cocktailService.AddOneAsync(cocktailRequestDto);
        
            return await newCocktailResult.Match<Task<IActionResult>>(
                async cocktail =>
                {
                    var newCocktailIngredientsList = _cocktailManagementService.MapCocktailIngredients(ingredientMap, cocktail, cocktailRequestDto);
                    
                    await _cocktailIngredientService.AddManyAsync(
                        newCocktailIngredientsList
                    );
                    
                    return CreatedAtAction(nameof(GetOneCocktailById), new { id = cocktail.Id }, cocktail);
                },
                validate => Task.FromResult<IActionResult>(BadRequest(validate.MapToResponse())),
                error => Task.FromResult<IActionResult>(StatusCode(500, error.Message))
            );
        }

        // TODO: Look into centralizing validation using FluentValidation
        [Authorize]
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> UpdateCocktail(
            [FromRoute] int id,
            [FromBody] UpdateCocktailRequestDto updateCocktailDto
        )
        {
            var cocktailResult = await _cocktailService.GetOneByIdAsync(id);

            return await cocktailResult.Match<Task<IActionResult>>(
                async result =>
                {

                    if (string.IsNullOrWhiteSpace(updateCocktailDto.Name))
                    {
                        updateCocktailDto.Name = result.Name;
                    }

                    if (!updateCocktailDto.Featured.HasValue)
                    {
                        updateCocktailDto.Featured = result.Featured;
                    }

                    if (updateCocktailDto.Tags != null && updateCocktailDto.Tags.Count == 0)
                    {
                        updateCocktailDto.Tags = result.Tags;
                    }

                    var cocktailModel = new Cocktail
                    {
                        Name = updateCocktailDto.Name,
                        Featured = updateCocktailDto.Featured.Value,
                        Tags = updateCocktailDto.Tags,
                        CocktailIngredients = updateCocktailDto.CocktailIngredients,
                        CreatedAt = result.CreatedAt,
                        UpdatedAt = DateTime.Now,
                    };

                    var updatedCocktail = await _cocktailService.UpdateOneAsync(id, cocktailModel);

                    return updatedCocktail.Match<IActionResult>(
                        updated => NoContent(),
                        nf => NotFound()
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
