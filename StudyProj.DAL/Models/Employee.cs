using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace StudyProj.DAL.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Employee
    {
        [Key]
        public Guid Id { get; set; }
        
        /// <summary>
        /// Login name
        /// </summary>        
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }

        public double Salary { get; set; }
        
        public DateTime CreatedDate { get; set; }

        public DateTime LastModifiedDate { get; set; }
    }
}
