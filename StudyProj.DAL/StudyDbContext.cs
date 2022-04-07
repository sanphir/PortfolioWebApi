using Microsoft.EntityFrameworkCore;
using StudyProj.DAL.Models;

namespace StudyProj.DAL
{
    public class StudyDbContext : DbContext
    {
        public StudyDbContext(DbContextOptions<StudyDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
    }
}
