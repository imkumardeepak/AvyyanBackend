using Microsoft.EntityFrameworkCore;
using AvyyanBackend.Models;

namespace AvyyanBackend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets for Knitfab business models
        public DbSet<MachineManager> MachineManagers { get; set; }

        // DbSets for Authentication
        public DbSet<User> Users { get; set; }
        public DbSet<RoleMaster> RoleMasters { get; set; }
        public DbSet<PageAccess> PageAccesses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure entity relationships and constraints

            // MachineManager configurations
            modelBuilder.Entity<MachineManager>()
                .HasIndex(m => m.MachineName)
                .IsUnique();

            // Authentication configurations
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<RoleMaster>()
                .HasIndex(r => r.RoleName)
                .IsUnique();

            modelBuilder.Entity<PageAccess>()
                .HasIndex(p => p.PageName)
                .IsUnique();

            // PageAccess relationship
            modelBuilder.Entity<PageAccess>()
                .HasOne(pa => pa.Role)
				.WithMany(r => r.PageAccesses)
                .HasForeignKey(pa => pa.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}