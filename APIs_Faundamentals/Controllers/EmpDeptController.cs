using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APIs_Faundamentals.Models;
using APIs_Faundamentals.Repository;
using APIs_Faundamentals.UnitOfWork;

namespace APIs_Faundamentals.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpDeptController : ControllerBase
    {
        UnitOfWork _UnitfWork;
        public EmpDeptController()
        {
            
        }
    }
}
