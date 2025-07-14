using Microsoft.EntityFrameworkCore;
using Quartile.Domain.Entities;
using Quartile.Domain.Interfaces.Entities;

namespace Quartile.Infrastructure.Context
{
    public class QuartileContext : DbContext
    {
        public DbSet<Company> Companies { get; set; }
        public DbSet<Store> Stores { get; set; }

        public QuartileContext(DbContextOptions<QuartileContext> options) : base(options)
        {
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            base.OnConfiguring(optionsBuilder);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(QuartileContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>().ToList())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.UpdatedAt = entry.Entity.CreatedAt;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }

            var result = await base.SaveChangesAsync(cancellationToken);
            foreach (var entry in ChangeTracker.Entries().ToList())
                entry.State = EntityState.Detached;

            return result;
        }
    }
}

