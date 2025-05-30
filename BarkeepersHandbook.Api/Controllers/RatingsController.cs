using System.Security.Claims;
using BarkeepersHandbook.Api.Services;
using BarkeepersHandbook.Application.DTOs.RatingDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarkeepersHandbook.Api.Controllers;

    [Authorize]
    [ApiController]
    [Route("/v1/[controller]")]
    public class RatingsController : ControllerBase
    {
        private readonly IRatingService _ratingService;
        private readonly ILogger<RatingsController> _logger;

        public RatingsController(IRatingService ratingService,  ILogger<RatingsController> logger)
        {
            _ratingService = ratingService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRatings()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            
            var ratings = await _ratingService.GetRatingsByUserAsync(userId);
            return Ok(ratings);
        }

        [HttpPost("{cocktailId:int}")]
        public async Task<IActionResult> CreateRating([FromBody] CocktailRatingDto ratingDto, [FromRoute] int cocktailId)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            
            var newRatingResult = await _ratingService.CreateRatingAsync(ratingDto, cocktailId, userId);

            return newRatingResult.Match<IActionResult>(
                r => CreatedAtAction(nameof(GetAllRatings), new { id = r.Id }, r),
                ue =>
                {
                    _logger.LogError($"Unexpected error while creating rating: {ue.Message}");
                    return StatusCode(500, ue.Message);
                },
                af => Conflict(new ProblemDetails
                {
                    Detail = "Rating already exists"
                })
                );
        }
        
        [HttpPatch("{id:int}")] public async Task<IActionResult> UpdateRating([FromRoute] int id, [FromBody] CocktailRatingDto ratingDto)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var result = await _ratingService.UpdateRatingAsync(ratingDto, id, userId);

            return result.Match<IActionResult>(
                s => Ok(),
                nf => NotFound()
            );
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteRating([FromRoute] int id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            await _ratingService.DeleteRatingByIdAsync(userId, id);

            return NoContent();
        }
    }