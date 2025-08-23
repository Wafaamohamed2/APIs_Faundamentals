using APIs_Faundamentals.DTO;
using APIs_Faundamentals.Models;
using APIs_Faundamentals.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace APIs_Faundamentals.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("GeneralPolicy")] // Apply rate limiting to the entire controller "DefaultPolicy" is defined in Program.cs
    public class AccountController : ControllerBase
    {
        private readonly IAuthoServ _authoServ;
  


        public AccountController(IAuthoServ authoServ )
        {
            _authoServ = authoServ;
          
        }



        // public endpoint for user registration but with rate limiting
        [HttpPost("Register")] 
        [EnableRateLimiting("AuthPolicy")] // Apply rate limiting for auth requests
        public async Task<IActionResult> Register([FromBody] UserDataDTO userData)
        {
            if (userData == null)
            {
                return BadRequest("Invalid registration data");
            }

          
            if (!ModelState.IsValid)
            {
                // generate a bad request response with validation errors
                return BadRequest("Please provide valid registration information");
            }



            userData.Email = userData.Email.Trim().ToLower(); // Normalize email to lowercase (input sanitization)
            var result = await _authoServ.RegisterAsync(userData);


            if (!result.IsAuthenticated)
            {
                return BadRequest(result.Message);
            }

            // Set the refresh token in a secure cookie
            if (!string.IsNullOrEmpty(result.RefreshToken))
            {
                
                SetRefreshTokenCookies(result.RefreshToken, result.RefreshTokenExpired); 
            }

            


            // dont return the password or sensitive data in the response
            return Ok(new
            {
                massege = result.Message,
                username = result.UserName,
                email = result.Email,
                token = result.Token,
            });
        }



        [HttpPost("Login")]
        [EnableRateLimiting("AuthPolicy")] // Apply rate limiting for auth requests
        public async Task<IActionResult> Login([FromBody] TokenReqModel tokenReq)
        {
           

            if (tokenReq == null || string.IsNullOrEmpty(tokenReq.Email) || string.IsNullOrEmpty(tokenReq.Password))
            {
                return BadRequest("Invalid login request");
            }


            var result = await _authoServ.GetTokenAsync(tokenReq);
            if (!result.IsAuthenticated)
            {

                // general error message for security reasons
                return BadRequest("Invalid credentials");
            }
           
            


            if (!string.IsNullOrEmpty(result.RefreshToken))
                SetRefreshTokenCookies(result.RefreshToken, result.RefreshTokenExpired);

            return Ok(result);
        }


        [HttpPost("AssignRole")]
        [Authorize(Roles = "Admin,SuperAdmin")]  // Ensure only Admin can assign roles
        [EnableRateLimiting("AdminPolicy")] // Apply rate limiting for admin requests
        public async Task<IActionResult> AssignRole(RoleModel roleModel)
        {
            var userClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            var logger = HttpContext.RequestServices.GetRequiredService<ILogger<AccountController>>();

            logger.LogInformation("User Claims: {Claims}",
                string.Join(", ", userClaims.Select(c => $"{c.Type}={c.Value}")));


            if (roleModel == null || string.IsNullOrEmpty(roleModel.Email) || string.IsNullOrEmpty(roleModel.Role))
            {
                return BadRequest(new { success = false, message = "Email and Role are required" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data provided");
            }


            // Normalize email to lowercase (input sanitization)
            roleModel.Email = roleModel.Email.Trim().ToLower();
            var requestedRole = roleModel.Role.Trim();


            var isCurrentUserSuperAdmin = User.IsInRole("SuperAdmin");

            if (requestedRole.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase) && !isCurrentUserSuperAdmin)
                return Forbid("Only SuperAdmin can assign SuperAdmin role");

            if (requestedRole.Equals("Admin", StringComparison.OrdinalIgnoreCase) && !isCurrentUserSuperAdmin)
                return Forbid("Only SuperAdmin can assign Admin role");


            var result = await _authoServ.AssignRoleAsync(roleModel);

            if (result.Contains("successfully", StringComparison.OrdinalIgnoreCase))
                return Ok(new { message = result });

            if (result.Contains("not found", StringComparison.OrdinalIgnoreCase))
                return NotFound(new { message = result });

            return BadRequest(new { message = result });
        }

        [HttpGet("RefreshToken")]
        [AllowAnonymous] 
        public async Task<IActionResult> RefreshToken()
        {

           var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken)) {
               
                return BadRequest("Refresh token not found");
            }
           
            var result = await _authoServ.RefreshTokenAsync(refreshToken);

            if(!result.IsAuthenticated)
            {

                Response.Cookies.Delete("refreshToken"); // Clear the cookie if the token is invalid or expired
                return Unauthorized("Invalid or expired refresh token");
            }

            SetRefreshTokenCookies(result.RefreshToken, result.RefreshTokenExpired);    
            return Ok(result);
        }


        [HttpPost]
        public void SetRefreshTokenCookies(string refreshToken, DateTime expir) {
        
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new ArgumentException("Refresh token cannot be null or empty", nameof(refreshToken));
            }

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expir.ToLocalTime(),
                Secure = true, // Use Secure cookies in production
                SameSite = SameSiteMode.Strict // Set SameSite to Strict for better security
               
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        
        }


        [HttpPost("RevokeToken")]
        [Authorize] // Ensure the user is authenticated to revoke the token

        public async Task<IActionResult> RevokeToken([FromBody] RevokeDTO revokeDTO )
        {
            // Check if the token is provided in the request body or as a cookie
            var token = revokeDTO.Token?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is required for revocation");
            }



            var result = await _authoServ.RevokeTokenAsync(token);

            if (result)
            {
                // Clear the cookie if revocation is successful
                Response.Cookies.Delete("refreshToken");
                return Ok("Token revoked successfully");
            }
            else
            {
                return BadRequest("Failed to revoke token");
            }



        }


        [HttpGet("Profile")]
        [Authorize] // Ensure the user is authenticated to access their profile
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst("uid")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated");
            }

            return Ok(new
            {
                // Return user profile information
                UserId = userId,
                UserName = User.FindFirst(ClaimTypes.Name)?.Value,
                Email = User.FindFirst(ClaimTypes.Email)?.Value,
                Roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList()
            });
        }




    }
}
    