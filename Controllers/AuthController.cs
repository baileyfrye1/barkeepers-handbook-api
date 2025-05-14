// using api.Data;
// using api.DTOs.Auth;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;

// namespace api.Controllers
// {
//     [ApiController]
//     [Route("v1/[controller]")]
//     public class AuthController : ControllerBase
//     {
//         private readonly AuthService _authService;

//         public AuthController(AuthService authService)
//         {
//             _authService = authService;
//         }

//         [Route("register")]
//         [HttpPost]
//         public async Task<IActionResult> Register([FromBody] UserCredsDto userCreds)
//         {
//             try
//             {
//                 var session = await _authService.RegisterUser(userCreds.email, userCreds.password);

//                 return Ok(session);
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(400, $"Error creating new user: {ex.Message}");
//             }
//         }

//         [Route("login")]
//         [HttpPost]
//         public async Task<IActionResult> Login([FromBody] UserCredsDto userCreds)
//         {
//             try
//             {
//                 var userSession = await _authService.LoginUser(userCreds.email, userCreds.password);

//                 if (userSession is null)
//                 {
//                     return Unauthorized(new { message = "Invalid credentials" });
//                 }

//                 var accessToken = userSession.AccessToken;
//                 var refreshToken = userSession.RefreshToken;

//                 Response.Cookies.Append(
//                     "access_token",
//                     accessToken,
//                     new CookieOptions
//                     {
//                         HttpOnly = true,
//                         Secure = true,
//                         SameSite = SameSiteMode.Strict,
//                         Expires = DateTime.UtcNow.AddSeconds(3600),
//                         Domain = "localhost",
//                     }
//                 );

//                 Response.Cookies.Append(
//                     "refresh_token",
//                     refreshToken,
//                     new CookieOptions
//                     {
//                         HttpOnly = true,
//                         Secure = true,
//                         SameSite = SameSiteMode.Strict,
//                         Expires = DateTime.UtcNow.AddDays(90),
//                         Domain = "localhost",
//                     }
//                 );

//                 return Ok(new { message = "Login successful" });
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(400, $"Error creating new user: {ex.Message}");
//             }
//         }

//         [Authorize]
//         [Route("status")]
//         [HttpGet]
//         public IActionResult UserInfo()
//         {
//             try
//             {
//                 var user = _authService.RetrieveUser();

//                 return Ok(user);
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(400, $"Error getting user status: {ex.Message}");
//             }
//         }

//         [Route("refresh")]
//         [HttpGet]
//         public async Task<IActionResult> RefreshToken()
//         {
//             try
//             {
//                 var session = await _authService.RefreshToken();

//                 Console.WriteLine(session);

//                 return Ok();
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(400, $"Error refreshing access token: {ex.Message}");
//             }
//         }
//     }
// }
