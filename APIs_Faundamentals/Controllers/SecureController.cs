using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIs_Faundamentals.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SecureController : ControllerBase
    {
        [HttpGet]
        public IActionResult SecureAction()
        {

            // This action is secured and can only be accessed by authenticated users
            return Ok("This is a secure endpoint. You are authenticated!");
        }


    }
}
