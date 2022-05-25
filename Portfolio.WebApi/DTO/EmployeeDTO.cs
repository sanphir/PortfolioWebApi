namespace Portfolio.WebApi.DTO
{
    public class EmployeeDTO : UpdateEmployeeDTO
    {
        public DateTime CreatedDate { get; set; }

        public DateTime LastModifiedDate { get; set; }

    }
}
