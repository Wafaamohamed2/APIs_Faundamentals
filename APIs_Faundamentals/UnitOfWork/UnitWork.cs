namespace APIs_Faundamentals.UnitOfWork
{
    using APIs_Faundamentals.Models;
    using APIs_Faundamentals.Repository;


    // Unit of Work pattern implementation to manage repositories and transactions within a single context to avoid multiple database calls and ensure consistency
    public class UnitWork
    {
        private readonly Models.PracticContext _context;

        public GenericRepos<Employee> _employeerepo;
        public GenericRepos<Department> _departmentrepo;
        public UnitWork(Models.PracticContext context)
        {
            _context = context;

            // Initialize the repositories with the same context to ensure they share the same database connection
            //_employeerepo = new GenericRepos<Employee>(_context);
            //_departmentrepo = new GenericRepos<Department>(_context);
        }
        public GenericRepos<Department> DepartmentRepository
        {
            get
            {
                // Lazy initialization of the Department repository to ensure it is created only when needed
                if (_departmentrepo == null)
                {
                    _departmentrepo = new GenericRepos<Department>(_context);
                }
                return _departmentrepo;
            }
        }
        public GenericRepos<Employee> EmployeeRepository
        {
            get
            {
                // Lazy initialization of the Employee repository to ensure it is created only when needed
                if (_employeerepo == null)
                {
                    _employeerepo = new GenericRepos<Employee>(_context);
                }
                return _employeerepo;
            }
        }

         
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
