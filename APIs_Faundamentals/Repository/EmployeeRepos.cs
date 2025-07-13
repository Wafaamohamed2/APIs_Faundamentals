using APIs_Faundamentals.Models;

namespace APIs_Faundamentals.Repository
{
    public class EmployeeRepos : IEmployeeRepos
    {
        // Repository class to handle Employee data operations querying the database

        private readonly Models.PracticContext _context;

        public EmployeeRepos(Models.PracticContext context)
        {
            _context = context;
        }

        public List<Models.Employee> GetAllEmployees()
        {
            // Fetch all employees from the database
            return _context.Employees.ToList();
        }
        public Models.Employee GetEmployeeBySSN(int ssn)
        {
            // Fetch a specific employee by SSN
            return _context.Employees.Find(ssn);
        }

        public void AddEmployee(Employee employee)
        {
            // Add a new employee to the database
            _context.Employees.Add(employee);
           // _context.SaveChanges();
        }

        public void UpdateEmployee(Employee employee)
        {
            // Update an existing employee in the database
            _context.Employees.Update(employee);
           // _context.SaveChanges();
        }

        public void DeleteEmployee(int ssn)
        {
            // Delete an employee by SSN
            var employee = _context.Employees.Find(ssn);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
              //  _context.SaveChanges();
            }
        }

        // to give fliexibility to the controller to save changes when needed and avoid multiple calls to SaveChanges in the repository methods
        public void SaveChanges()
        {
            // Save changes to the database
            _context.SaveChanges();
        }
    }
}
