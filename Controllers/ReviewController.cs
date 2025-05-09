using System.Security.Claims;
using System.Text.Json;
using api.Authentication;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

    [ApiController]
    [Route("api/v1/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewService _reviewService;

        public ReviewsController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReviews()
        {
            var settings = HttpContext.RequestServices.GetRequiredService<ClerkAuthSettings>();
            if (settings.SecretKey == null)
            {
                throw new InvalidOperationException("Clerk Secret Key is not configured.");
            }
            
            if (Request == null)
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

                foreach (var claim in state.Claims.Claims)
                {
                    Console.WriteLine(claim);
                }
                
                var userId = state.Claims.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                
                var reviews = await _reviewService.GetReviewsAsync(userId);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in ClerkAuthHelper: " + ex.ToString());
                throw;
            }
        }
    }