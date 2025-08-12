namespace APIs_Faundamentals.DTO
{
    public class EmployeeDTO 
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public int Salary { get; set; }
        public int DeparetmentNum { get; set; } 
    }
}
