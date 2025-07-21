using APIs_Faundamentals.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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


        [HttpPost("Login")]
        public ActionResult Login(UserDataDTO user)
        {
            if (user.UserName == "admin" && user.Password == "1234")
            {
                #region Create claims
                List<Claim> userdata = new List<Claim>();
                userdata.Add(new Claim ("name" , user.UserName ));
                userdata.Add(new Claim(ClaimTypes.MobilePhone,"0104679627"));

                #endregion

                #region Security key
                string secretkey = "Welcome to my app wish you interest with us";
                var Key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes( secretkey));


                var signcer = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);

                #endregion


                #region create token
                // header type ,algo
                //payload claims , expiration
                //signature => hash secret key +...

                var token = new JwtSecurityToken(
                    claims: userdata,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: signcer
                    
                );

                var stringToken = new JwtSecurityTokenHandler().WriteToken(token);




                #endregion

                return Ok(stringToken);
            }
            else
            {
                return Unauthorized("Invalid credentials");
            }
          
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetAll()
        {


            return Ok();
        }

      
    }
}
