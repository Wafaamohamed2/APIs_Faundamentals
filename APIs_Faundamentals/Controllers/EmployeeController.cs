using APIs_Faundamentals.DTO;
using APIs_Faundamentals.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using APIs_Faundamentals.Repository;
using APIs_Faundamentals.UnitOfWork;

namespace APIs_Faundamentals.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        //IEmployeeRepos _employeeRepos;  // dependence inversion principle, to avoid tight coupling with the repository
        //GenericRepos<Employee> GenericRepos;
        //public EmployeeController(GenericRepos<Employee> GenericRepos)  
        //{
        //   GenericRepos = GenericRepos;
        //}


        UnitWork _unit; // Unit of Work to manage repositories and transactions

        public EmployeeController(UnitWork unit)
        {
            _unit = unit;   
        }

        [HttpGet]
 
        public ActionResult<List<EmployeeDTO>> GetAll()
        {
            List<Employee> employees = _unit._employeerepo.SelectAll(); // Get all employees from the repository
            List<EmployeeDTO> employeesDTO = new List<EmployeeDTO>();

            foreach (var e in employees)
            {
                EmployeeDTO employeeDTO = new EmployeeDTO()
                {
                    Id = e.SSN,
                    FullName = e.Fname + " " + e.Lname,
                    Salary = e.Salary ?? 0,  // if Salary is null then return 0
                    DeparetmentName = e.DnoNavigation?.Dname ?? "No Department",
                };
                employeesDTO.Add(employeeDTO);
            }

            return Ok(employeesDTO);
        }


        //[HttpGet("{SSN}")]   // only return the Employee whithout specify the Statuse code cases
        //public Employee Get(int SSN) {

        //    return _context.Employees.Find(SSN);

        //}


        ///<summary>
        ///get specific employee by SSN  
        ///</summary>
        ///<param name="SSN">Employee SSN</param>
        ///<returns>Specific Employee</returns>
        ///<remarks>
        ///request example:
        ///api/Employee/102660
        ///</remarks>
        
      
        
        [HttpGet("{SSN}")]
        [ProducesResponseType<EmployeeDTO>(200)]
        [ProducesResponseType(404)]  // NotFound
        public ActionResult Get(int SSN)   // ActionResult allow to return Statuse code cases
        {

         /*  Employee employee=  GenericRepos.SelectById(SSN);*/  // Get specific employee by SSN from the repository

            Employee employee = _unit._employeerepo.SelectById(SSN);  // Get specific employee by SSN from the repository
            if (employee == null)
            {
                return NotFound();
            }
            else
            {
                EmployeeDTO employeeDTO = new EmployeeDTO()
                {
                    
                    Id = employee.SSN,
                    FullName = employee.Fname + " " + employee.Lname,
                    Salary = employee.Salary ?? 0,
                    Address = employee.Address,
                    DeparetmentName = employee.DnoNavigation?.Dname ?? "No Department"
                };

                return Ok(employeeDTO);  // 200 OK
            }
               

        }

        [HttpPost
            ]
        public ActionResult Create(Employee e) {

            if (e == null) { return BadRequest(); }
            if (!ModelState.IsValid) { return BadRequest(); }

            //GenericRepos.Add(e);// Add new employee to the repository
            //GenericRepos.Save();  
            
            _unit._employeerepo.Add(e);  // Add new employee to the repository
            _unit.Save();  // Save changes to the database

            //return Created("ayhaga",e);  it location is "ayhaga"
            return CreatedAtAction(nameof(Get), new { SSN = e.SSN },e);  // frist 2 parmeters to create the IRL so ots location is "https://localhost:7163/api/Employee/5544"

        }

        [HttpPut("{SSN}")]
        public ActionResult Edit(Employee e , int SSN) {

            if (!ModelState.IsValid) { return BadRequest(); }
            if (e == null) { return BadRequest(); }
            if (e.SSN != SSN ) { return BadRequest(); }


            //GenericRepos.Update(e); // Update existing employee in the repository
            // GenericRepos.Save();  

            _unit._employeerepo.Update(e);  // Update existing employee in the repository
            _unit.Save();  // Save changes to the database

            return NoContent();
        }

        [HttpDelete("{SSN}")]
        public ActionResult Delete(int SSN) {
           
         //GenericRepos.Delete(SSN);  // Delete employee by SSN from the repository
         //   GenericRepos.Save();

            _unit._employeerepo.Delete(SSN);  // Delete employee by SSN from the repository
            _unit.Save();  // Save changes to the database

            return NoContent();  // 204 No Content
        }
    }
}
