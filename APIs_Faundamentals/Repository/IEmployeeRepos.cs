using APIs_Faundamentals.Models;
using Microsoft.EntityFrameworkCore;

namespace APIs_Faundamentals.Repository
{
    // DI (Dependency inversion) interface for Employee repository
    public interface IEmployeeRepos
    {
        public List<Models.Employee> GetAllEmployees();


        public Models.Employee GetEmployeeBySSN(int ssn);



        public void AddEmployee(Employee employee);



        public void UpdateEmployee(Employee employee);



        public void DeleteEmployee(int ssn);


        public void SaveChanges();
        
    }
}
