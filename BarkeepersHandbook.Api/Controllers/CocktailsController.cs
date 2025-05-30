using System.Security.Claims;
using BarkeepersHandbook.Api.Exceptions;
using BarkeepersHandbook.Application.Models;
using BarkeepersHandbook.Api.Services.CocktailServices;
using BarkeepersHandbook.Application.DTOs.CocktailDTOs;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarkeepersHandbook.Api.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class CocktailsController : ControllerBase
    {
        private readonly ICocktailService _cocktailService;
        private readonly ILogger<CocktailsController> _logger;
        private readonly IValidator<CreateCocktailRequestDto> _createValidator;

        public CocktailsController(
            ICocktailService cocktailService,
            ILogger<CocktailsController> logger,
            IValidator<CreateCocktailRequestDto> createValidator
        )
        {
            _cocktailService = cocktailService;
            _logger = logger;
            _createValidator = createValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCocktails([FromQuery] string? search, int page, bool countOnly = false)
        {
            var cocktailsDto = await _cocktailService.GetAllAsync(search, page, countOnly);
            return Ok(new { cocktailsDto.Cocktails, cocktailsDto.TotalCount });
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
            [FromForm] CreateCocktailRequestDto cocktailRequestDto
        )
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            
            var validationResult = _createValidator.Validate(cocktailRequestDto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            
            var newCocktailResult = await _cocktailService.CreateCocktailAsync(cocktailRequestDto, userId);
        
            return await newCocktailResult.Match<Task<IActionResult>>(
                cocktail =>
                {
                    return Task.FromResult<IActionResult>(CreatedAtAction(nameof(GetOneCocktailById), new { id = cocktail.Id }, cocktail));
                },
                error =>
                {
                    _logger.LogError($"Unexpected error while creating cocktail: {error.Message}");
                    return Task.FromResult<IActionResult>(StatusCode(500, error.Message));
                });
        }

        // TODO: Look into centralizing validation using FluentValidation
        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCocktail(
            [FromRoute] int id,
            [FromBody] UpdateCocktailRequestDto updateCocktailDto
        )
        {
            var cocktailResult = await _cocktailService.GetOneByIdAsync(id);

            return await cocktailResult.Match<Task<IActionResult>>(
                async result =>
                {
                    updateCocktailDto.Name ??= result.Name;
                    updateCocktailDto.Featured ??= result.Featured;
                    updateCocktailDto.Tags = (updateCocktailDto.Tags != null || updateCocktailDto.Tags?.Count == 0
                        ? result.Tags
                        : updateCocktailDto.Tags);
                    
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
            try
            {
                await _cocktailService.DeleteOneAsync(id);
                return NoContent();
            }
            catch (ServiceLayerException e)
            {
                _logger.LogError(e, "Delete failed");
                return StatusCode(500, "An error occured while deleting the cocktail");
            }
        }
    }
}
