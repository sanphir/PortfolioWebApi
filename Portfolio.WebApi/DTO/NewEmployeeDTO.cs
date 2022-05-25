namespace Portfolio.WebApi.DTO
{
    public class NewEmployeeDTO : BaseEmployeeDTO
    {
        public string Password { get; set; } = string.Empty;
    }
}
