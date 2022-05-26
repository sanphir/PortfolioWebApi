namespace Portfolio.WebApi.DTO
{
    public class BaseEmployeeDTO
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }

        public double Salary { get; set; }
    }
    public class UpdateEmployeeDTO : BaseEmployeeDTO
    {
        public Guid Id { get; set; }
    }
    public class EmployeeDTO : UpdateEmployeeDTO
    {
        public DateTime CreatedDate { get; set; }

        public DateTime LastModifiedDate { get; set; }
    }

    public class NewEmployeeDTO : BaseEmployeeDTO
    {
        public string Password { get; set; } = string.Empty;
    }
}
