using APIs_Faundamentals.DTO;
using APIs_Faundamentals.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIs_Faundamentals.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {

        PracticContext _context;
        public EmployeeController(PracticContext context)  // Constructor Dependency Injection
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<List<EmployeeDTO>> GetAll()
        {
            List<Employee> employees = _context.Employees.ToList();
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


        [HttpGet("{SSN}")]   
        public ActionResult Get(int SSN)   // ActionResult allow to return Statuse code cases
        {

           Employee employee=  _context.Employees.Find(SSN);
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
            _context.Employees.Add(e);
            _context.SaveChanges();

            //return Created("ayhaga",e);  it location is "ayhaga"
            return CreatedAtAction(nameof(Get), new { SSN = e.SSN },e);  // frist 2 parmeters to create the IRL so ots location is "https://localhost:7163/api/Employee/5544"

        }

        [HttpPut("{SSN}")]
        public ActionResult Edit(Employee e , int SSN) {

            if (!ModelState.IsValid) { return BadRequest(); }
            if (e == null) { return BadRequest(); }
            if (e.SSN != SSN ) { return BadRequest(); }
            _context.Employees.Update(e); _context.SaveChanges();
            
            return NoContent();
        }

        [HttpDelete("{SSN}")]
        public ActionResult Delete(int SSN) {
           
            Employee e = _context.Employees.Find(SSN);
            if (e == null) { return NotFound(); }

            _context.SaveChanges();
            return Ok(e);
        }
    }
}
