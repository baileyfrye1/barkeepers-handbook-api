using System.Security.Claims;
using api.DTOs.RatingDTOs;
using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

    [ApiController]
    [Route("/v1/[controller]")]
    public class RatingsController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public RatingsController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRatings()
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                
                var ratings = await _ratingService.GetAllRatingsByUserAsync(userId);
                return Ok(ratings);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in ClerkAuthHelper: " + ex.ToString());
                throw;
            }
        }

        [HttpPost("{cocktailId:int}")]
        public async Task<IActionResult> CreateRating([FromBody] CocktailRatingDto ratingDto, [FromRoute] int cocktailId)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            
            var newRatingResult = await _ratingService.CreateRatingAsync(ratingDto, cocktailId, userId);
            
            return CreatedAtAction(nameof(GetAllRatings), new { id = newRatingResult.Id }, newRatingResult);
        }
    }