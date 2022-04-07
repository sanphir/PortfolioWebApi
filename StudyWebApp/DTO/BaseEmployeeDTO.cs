namespace StudyProj.WebApp.DTO
{
    public class BaseEmployeeDTO
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }

        public double Salary { get; set; }
    }
}
