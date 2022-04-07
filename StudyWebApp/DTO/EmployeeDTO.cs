namespace StudyProj.WebApp.DTO
{
    public class EmployeeDTO : BaseEmployeeDTO
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }

        public DateTime LastModifiedDate { get; set; }

    }
}
