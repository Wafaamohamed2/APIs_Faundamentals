using APIs_Faundamentals.DTO;
using APIs_Faundamentals.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APIs_Faundamentals.Repository;

namespace APIs_Faundamentals.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        GenericRepos<Department> GenericRepos;    
        public DepartmentController(GenericRepos<Department> genericRepos )
        {
           GenericRepos = genericRepos;
        }

        [HttpGet("{id}")]
        public ActionResult Get(int id )
        {
            Department department = GenericRepos.SelectById(id);

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
            List<Department> departments = GenericRepos.SelectAll();
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
