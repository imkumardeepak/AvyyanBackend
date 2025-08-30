using AvyyanBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TallyERPWebApi.Model;

namespace AvyyanBackend.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor? httpContextAccessor = null) : base(options)
        {
            _httpContextAccessor = httpContextAccessor ?? null;
        }

        public DbSet<MachineManager> MachineManagers { get; set; }
        public DbSet<FabricStructureMaster> FabricStructureMasters { get; set; }
        public DbSet<LocationMaster> LocationMasters { get; set; }
        public DbSet<YarnTypeMaster> YarnTypeMasters { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<RoleMaster> RoleMasters { get; set; }
        public DbSet<PageAccess> PageAccesses { get; set; }

        public DbSet<SalesOrder> SalesOrders { get; set; }

          public DbSet<SalesOrderItem> SalesOrderItems { get; set; }

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
                .HasIndex(p => p.RoleId);

            // PageAccess relationship
            modelBuilder.Entity<PageAccess>()
                .HasOne(pa => pa.Role)
				.WithMany(r => r.PageAccesses)
                .HasForeignKey(pa => pa.RoleId)
                .OnDelete(DeleteBehavior.Cascade);


			modelBuilder.Entity<SalesOrder>()
			.HasMany(v => v.Items)
			.WithOne(i => i.Voucher)
			.HasForeignKey(i => i.SalesOrderId)
			.OnDelete(DeleteBehavior.Cascade);

			// Add indexes for better performance
			modelBuilder.Entity<SalesOrder>()
				.HasIndex(v => v.VoucherNumber)
				.IsUnique();

			modelBuilder.Entity<SalesOrder>()
				.HasIndex(v => v.SalesDate);

			modelBuilder.Entity<SalesOrder>()
				.HasIndex(v => v.PartyName);

            modelBuilder.Entity<SalesOrder>()
                .HasIndex(v => v.ProcessFlag);

			modelBuilder.Entity<SalesOrderItem>()
				.HasIndex(i => i.SalesOrderId);

			modelBuilder.Entity<SalesOrderItem>()
				.HasIndex(i => i.StockItemName);
		}

        public override int SaveChanges()
        {
            UpdateTimestampsAndUserFields();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestampsAndUserFields();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestampsAndUserFields()
        {
            var entries = ChangeTracker.Entries<BaseEntity>()
                .Where(e => e.Entity is BaseEntity && (
                    e.State == EntityState.Added ||
                    e.State == EntityState.Modified));

            var currentUser = GetCurrentUser();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    ((BaseEntity)entry.Entity).CreatedAt = DateTime.Now;
                    ((BaseEntity)entry.Entity).CreatedBy = currentUser;
                }

                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    ((BaseEntity)entry.Entity).UpdatedAt = DateTime.Now;
                    ((BaseEntity)entry.Entity).UpdatedBy = currentUser;
                }
            }
        }

        private string GetCurrentUser()
        {
            if (_httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated == true)
            {
                // Try to get the user's full name first
                var fullName = _httpContextAccessor.HttpContext.User.FindFirst("FullName")?.Value;
                if (!string.IsNullOrEmpty(fullName))
                {
                    return fullName;
                }

                // Fallback to email
                var email = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
                if (!string.IsNullOrEmpty(email))
                {
                    return email;
                }
            }

            // Default value when no user is authenticated
            return "System";
        }
    }
}