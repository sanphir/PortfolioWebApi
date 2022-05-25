using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Portfolio.DAL.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Employee : BaseEntity
    {

        /// <summary>
        /// Login name
        /// </summary>        
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string Email { get; set; }

        [MaxLength(100)]
        public string Password { get; set; }

        public Role Role { get; set; }

        public DateTime BirthDate { get; set; }

        public double Salary { get; set; }

    }
}
