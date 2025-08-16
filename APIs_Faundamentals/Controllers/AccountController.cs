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

namespace APIs_Faundamentals.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthoServ _authoServ;

        public AccountController(IAuthoServ authoServ)
        {
            _authoServ = authoServ;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserDataDTO userData)
        {
            if (userData == null)
            {
                return BadRequest(ModelState);
            }

            var result = await _authoServ.RegisterAsync(userData);
            if (!result.IsAuthenticated)
            {
                return BadRequest(result.Message);
            }


            SetRefreshTokenCookies(result.RefreshToken, result.RefreshTokenExpired);
            return Ok(result);
        }



        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] TokenReqModel tokenReq)
        {
            Console.WriteLine($"Email from request: '{tokenReq.Email}'");
            Console.WriteLine($"Password from request: '{tokenReq.Password}'");

            if (tokenReq == null || string.IsNullOrEmpty(tokenReq.Email) || string.IsNullOrEmpty(tokenReq.Password))
            {
                return BadRequest("Invalid login request");
            }

           


            var result = await _authoServ.GetTokenAsync(tokenReq);
            if (!result.IsAuthenticated)
            {
                return BadRequest(result.Message);
            }
           
            if (!string.IsNullOrEmpty(result.RefreshToken))
                SetRefreshTokenCookies(result.RefreshToken, result.RefreshTokenExpired);

            return Ok(result);
        }


        [HttpPost("AssignRole")]

        public async Task<IActionResult> AssignRole(RoleModel roleModel)
        {
            if (roleModel == null || string.IsNullOrEmpty(roleModel.Id) || string.IsNullOrEmpty(roleModel.Role))
            {
                return BadRequest("Invalid role assignment request");
            }

            var result = await _authoServ.AssignRoleAsync(roleModel);
            if (result != null)
            {
                return Ok(result); 
            }
            else
            {
                return BadRequest("Role assignment failed");
            }
        }


        [HttpGet("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
           var refreshToken = Request.Cookies["refreshToken"];
           
            var result = await _authoServ.RefreshTokenAsync(refreshToken);

            if(!result.IsAuthenticated)
            {
                return Unauthorized("Invalid or expired refresh token");
            }

            SetRefreshTokenCookies(result.RefreshToken, result.RefreshTokenExpired);    
            return Ok(result);
        }


        [HttpPost]
        public void SetRefreshTokenCookies(string refreshToken, DateTime expir) {
        
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expir.ToLocalTime(),
               
              
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        
        }


        [HttpPost("RevokeToken")]

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

    }
}
    