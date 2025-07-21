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
        private UnitWork _unit;
        public EmpDeptController(UnitWork unit)
        {
         
            _unit = unit;
        }

        [HttpPost]

        public ActionResult Add(Employee employee)
        {
            _unit._departmentrepo.Add(employee.DnoNavigation);
            _unit._employeerepo.Add(employee);

            _unit.Save();
            return Ok("Employee and Department added successfully");
        }
    }
}
