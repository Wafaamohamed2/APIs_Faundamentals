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
            if (result.IsAuthenticated)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result.Message);
            }
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
            if (result.IsAuthenticated)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result.Message);
            }


    
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


       

    }
}
