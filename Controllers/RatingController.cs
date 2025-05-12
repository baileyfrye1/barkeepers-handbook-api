using System.Security.Claims;
using System.Text.Json;
using api.Authentication;
using api.DTOs.RatingDTOs;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

    [ApiController]
    [Route("api/v1/[controller]")]
    public class RatingsController : ControllerBase
    {
        private readonly RatingService _ratingService;

        public RatingsController(RatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRatings()
        {
            var settings = HttpContext.RequestServices.GetRequiredService<ClerkAuthSettings>();
            if (settings.SecretKey == null)
            {
                throw new InvalidOperationException("Clerk Secret Key is not configured.");
            }
            
            if (Request.Cookies == null)
            {
                throw new InvalidOperationException("Request object is null.");
            }

            try
            {
                var (isSignedIn, state) = await ClerkAuthHelper.IsSignedInAsync(Request, settings.SecretKey);
                
                if (!isSignedIn)
                {
                    return Unauthorized();
                }

                var userId = state.Claims.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                
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
            var settings = HttpContext.RequestServices.GetRequiredService<ClerkAuthSettings>();
            
            var (isSignedIn, state) = await ClerkAuthHelper.IsSignedInAsync(Request, settings.SecretKey);
            
            if (!isSignedIn)
            {
                return Unauthorized();
            }

            var userId = state.Claims.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                
            var newRatingResult = await _ratingService.CreateRatingAsync(ratingDto, cocktailId, userId);
            
            return CreatedAtAction(nameof(GetAllRatings), new { id = newRatingResult.Id }, newRatingResult);
        }
    }