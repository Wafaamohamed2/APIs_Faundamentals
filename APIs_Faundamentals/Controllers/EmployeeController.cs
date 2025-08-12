using APIs_Faundamentals.DTO;
using APIs_Faundamentals.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using APIs_Faundamentals.Repository;
using APIs_Faundamentals.UnitOfWork;
using Microsoft.AspNetCore.Authorization;

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


       private readonly UnitWork _unit; // Unit of Work to manage repositories and transactions

        public EmployeeController(UnitWork unit)
        {
            _unit = unit;   
        }

        [HttpGet]
        [Authorize(Roles = "Admin")] 
        public ActionResult<List<EmployeeDTO>> GetAllemps()
        {
            List<Employee> employees = _unit.EmployeeRepository.SelectAll(); // Get all employees from the repository
            List<EmployeeDTO> employeesDTO = new List<EmployeeDTO>();

            if (employees == null || employees.Count == 0)
            {
                return NotFound("no employees exist"); // Return 404 Not Found if no employees exist
            }

            foreach (var e in employees)
            {
                EmployeeDTO employeeDTO = new EmployeeDTO()
                {
                    Id = e.SSN,
                    FullName = e.Fname + " " + e.Lname,
                    Salary = e.Salary ?? 0,  // if Salary is null then return 0
                    DeparetmentNum = e.Dno ?? 0,
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

            Employee employee = _unit.EmployeeRepository.SelectById(SSN);  // Get specific employee by SSN from the repository
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
                    DeparetmentNum = employee.Dno ?? 0,
                };

                return Ok(employeeDTO);  // 200 OK
            }
               

        }

        [HttpPost
            ]
        public ActionResult Create(EmployeeDTO e) {

            if (e == null) { return BadRequest(); }
            if (!ModelState.IsValid) { return BadRequest(); }

            //GenericRepos.Add(e);// Add new employee to the repository
            //GenericRepos.Save();  
            
            Employee employee = new Employee()
            {
                SSN = e.Id,
                Fname  = e.FullName,
                Salary = e.Salary ,  // if Salary is null then return 0
                Dno = e.DeparetmentNum,  // Assuming you have a method to get department by name  
            };

            _unit.EmployeeRepository.Add(employee);  // Add new employee to the repository
            _unit.Save();  // Save changes to the database

            //return Created("ayhaga",e);  it location is "ayhaga"
            return CreatedAtAction(nameof(Get), new { SSN = e.Id },e);  // frist 2 parmeters to create the IRL so ots location is "https://localhost:7163/api/Employee/5544"

        }

        [HttpPut("{SSN}")]
        [Authorize(Roles ="Admin")]
        public ActionResult Edit(EmployeeDTO e , int SSN) {

            if (!ModelState.IsValid) { return BadRequest(); }
            if (e == null) { return BadRequest("Employee data is required."); }
            if (e.Id != SSN ) { return BadRequest("SSN mismatch."); }


            //GenericRepos.Update(e); // Update existing employee in the repository
            // GenericRepos.Save();  

            var employee = _unit.EmployeeRepository.SelectById(SSN);
            if (employee == null)
                return NotFound($"Employee with SSN {SSN} not found.");


            var names = e.FullName?.Split(' ') ?? new string[] { "Unknown", "" };
            employee.Fname = names[0];
            employee.Lname = names.Length > 1 ? string.Join(" ", names.Skip(1)) : "";


            employee.Salary = e.Salary ;
            employee.Address = e.Address;

            var department = _unit.DepartmentRepository.SelectById(e.DeparetmentNum);
            if (department == null)
                return BadRequest("Invalid department number.");

            employee.Dno = department.Dnum;

            _unit._employeerepo.Update(employee);  // Update existing employee in the repository
            _unit.Save();  // Save changes to the database

            return NoContent();
        }

        [HttpDelete("{SSN}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int SSN) {
           
         //GenericRepos.Delete(SSN);  // Delete employee by SSN from the repository
         //   GenericRepos.Save();

            _unit.EmployeeRepository.Delete(SSN);  // Delete employee by SSN from the repository
            _unit.Save();  // Save changes to the database

            return NoContent();  // 204 No Content
        }
    }
}
