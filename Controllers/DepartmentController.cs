using APIs_Faundamentals.DTO;
using APIs_Faundamentals.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIs_Faundamentals.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        public Models.PracticContext _context;
        public DepartmentController(PracticContext context )
        {
           _context = context;
        }

        [HttpGet("{id}")]
        public ActionResult Get(int id )
        {
            Department department = _context.Departments.Find(id);

            if (department == null)
            {
                return NotFound();
            }
            else
            {
                DepartmentDTO departmentDTO = new DepartmentDTO()
                {
                    Id = department.Dnum,
                    Name = department.Dname,
                    EmployeesNames = department.Employees.Select(e => e.Fname + " " + e.Lname).ToList()
                
                };

                //foreach (Employee e in department.Employees) {
                //    departmentDTO.EmployeesNames.Add(e.Fname + " " + e.Lname);
                //}

            return Ok(departmentDTO);
            }
            
        }


        [HttpGet ]

        public ActionResult<List<DepartmentDTO>> GetAll()
        {
            List<Department> departments = _context.Departments.ToList();
            List<DepartmentDTO> departmentsDTO = new List<DepartmentDTO>();

            foreach (var d in departments)
            {
                DepartmentDTO departmentDTO = new DepartmentDTO()
                {
                    Id = d.Dnum,
                    Name = d.Dname,
                    EmployeesNames = d.Employees.Select(e => e.Fname + " " + e.Lname).ToList()
                };
                departmentsDTO.Add(departmentDTO);
            }

            return Ok(departmentsDTO);
        }
    }
}
