using APIs_Faundamentals.DTO;
using APIs_Faundamentals.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APIs_Faundamentals.Repository;
using APIs_Faundamentals.UnitOfWork;

namespace APIs_Faundamentals.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        //GenericRepos<Department> GenericRepos;    
        //public DepartmentController(GenericRepos<Department> genericRepos )
        //{
        //   GenericRepos = genericRepos;
        //}


        UnitWork _unit; // Unit of Work to manage repositories and transactions
        public DepartmentController(UnitWork unit)
        {
            _unit = unit;
        }

        [HttpGet("{id}")]
        public ActionResult Get(int id )
        {
            //Department department = GenericRepos.SelectById(id);

            Department department = _unit.DepartmentRepository.SelectById(id);

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
            //List<Department> departments = GenericRepos.SelectAll();

            List<Department> departments = _unit.DepartmentRepository.SelectAll();  
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
