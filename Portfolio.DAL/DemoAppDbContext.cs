using Microsoft.EntityFrameworkCore;
using Portfolio.DAL.Models;

namespace Portfolio.DAL
{
    /* Create migration
     * dotnet ef migrations add InitialCreate --startup-project ../Portfolio.WebApi/
     * Use migration to create DB
     * dotnet ef database update --startup-project ../Portfolio.WebApi/
     */
    public class DemoAppDbContext : DbContext
    {
        public DemoAppDbContext(DbContextOptions<DemoAppDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<WorkTask> WorkTasks { get; set; }

        public override int SaveChanges()
        {
            UpdateEntitiesDates();

            return base.SaveChanges();
        }
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            UpdateEntitiesDates();

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            UpdateEntitiesDates();

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateEntitiesDates();

            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateEntitiesDates()
        {
            var entries = ChangeTracker.Entries().Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            var utcNow = DateTime.Now.ToUniversalTime();
            foreach (var entityEntry in entries)
            {
                ((BaseEntity)entityEntry.Entity).LastModifiedDate = utcNow;

                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntry.Entity).CreatedDate = utcNow;
                }
            }
        }
    }
}
