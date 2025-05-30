using Microsoft.AspNetCore.Mvc;

namespace BarkeepersHandbook.Api.Controllers;

[ApiController]
[Route("v1/[controller]")]
public class FavoritesController : ControllerBase
{
   [HttpGet]
   public async Task<IActionResult> GetFavoritesByUser()
   {
      return Ok();
   }
   
   [HttpPost]
   public async Task<IActionResult> CreateFavorite()
   {
      return Ok();
   }

   [HttpDelete]
   public async Task<IActionResult> DeleteFavorite()
   {
      return NoContent();
   }
}