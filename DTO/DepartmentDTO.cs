namespace APIs_Faundamentals.DTO
{
    public class DepartmentDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
       
        public List<string> EmployeesNames { get; set; } = new List<string>();
    }
}
