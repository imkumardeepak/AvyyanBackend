using AvyyanBackend.Data;
using AvyyanBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace AvyyanBackend.Services
{
    public class DataSeedService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DataSeedService> _logger;

        public DataSeedService(ApplicationDbContext context, ILogger<DataSeedService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                await SeedRolesAsync();
                await SeedPageAccessesAsync();
                await SeedDefaultUserAsync();
                await SeedRolePageAccessesAsync();
                
                _logger.LogInformation("Data seeding completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during data seeding");
                throw;
            }
        }

        private async Task SeedRolesAsync()
        {
            var roles = new[]
            {
                new Role { Name = "Admin", Description = "System Administrator with full access", IsSystemRole = true },
                new Role { Name = "Manager", Description = "Manager with elevated permissions", IsSystemRole = true },
                new Role { Name = "Employee", Description = "Regular employee access", IsSystemRole = true },
                new Role { Name = "User", Description = "Basic user access", IsSystemRole = true }
            };

            foreach (var role in roles)
            {
                if (!await _context.Roles.AnyAsync(r => r.Name == role.Name))
                {
                    _context.Roles.Add(role);
                    _logger.LogInformation("Added role: {RoleName}", role.Name);
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task SeedPageAccessesAsync()
        {
            var pageAccesses = new[]
            {
                // Dashboard
                new PageAccess { PageName = "Dashboard", PageUrl = "/dashboard", Description = "Main dashboard", Category = "Main", Icon = "dashboard", SortOrder = 1, IsMenuItem = true },
                
                // Machine Management
                new PageAccess { PageName = "Machines", PageUrl = "/machines", Description = "Machine management", Category = "Operations", Icon = "precision_manufacturing", SortOrder = 10, IsMenuItem = true },
                new PageAccess { PageName = "Machine Details", PageUrl = "/machines/details", Description = "View machine details", Category = "Operations", Icon = "info", SortOrder = 11, IsMenuItem = false },
                
                // Chat & Communication
                new PageAccess { PageName = "Chat", PageUrl = "/chat", Description = "Chat and messaging", Category = "Communication", Icon = "chat", SortOrder = 20, IsMenuItem = true },
                new PageAccess { PageName = "Notifications", PageUrl = "/notifications", Description = "Notifications center", Category = "Communication", Icon = "notifications", SortOrder = 21, IsMenuItem = true },
                
                // User Management
                new PageAccess { PageName = "Users", PageUrl = "/users", Description = "User management", Category = "Administration", Icon = "people", SortOrder = 30, IsMenuItem = true },
                new PageAccess { PageName = "Roles", PageUrl = "/roles", Description = "Role management", Category = "Administration", Icon = "admin_panel_settings", SortOrder = 31, IsMenuItem = true },
                new PageAccess { PageName = "Permissions", PageUrl = "/permissions", Description = "Permission management", Category = "Administration", Icon = "security", SortOrder = 32, IsMenuItem = true },
                
                // Reports
                new PageAccess { PageName = "Reports", PageUrl = "/reports", Description = "Reports and analytics", Category = "Reports", Icon = "assessment", SortOrder = 40, IsMenuItem = true },
                new PageAccess { PageName = "Machine Reports", PageUrl = "/reports/machines", Description = "Machine performance reports", Category = "Reports", Icon = "bar_chart", SortOrder = 41, IsMenuItem = false },
                
                // Settings
                new PageAccess { PageName = "Settings", PageUrl = "/settings", Description = "System settings", Category = "Administration", Icon = "settings", SortOrder = 50, IsMenuItem = true },
                new PageAccess { PageName = "Profile", PageUrl = "/profile", Description = "User profile", Category = "Personal", Icon = "account_circle", SortOrder = 60, IsMenuItem = true }
            };

            foreach (var pageAccess in pageAccesses)
            {
                if (!await _context.PageAccesses.AnyAsync(pa => pa.PageUrl == pageAccess.PageUrl))
                {
                    _context.PageAccesses.Add(pageAccess);
                    _logger.LogInformation("Added page access: {PageName}", pageAccess.PageName);
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task SeedDefaultUserAsync()
        {
            if (!await _context.Users.AnyAsync(u => u.Username == "admin"))
            {
                var adminUser = new User
                {
                    FirstName = "System",
                    LastName = "Administrator",
                    Username = "admin",
                    Email = "admin@avyyan.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), // Change this in production!
                    IsEmailVerified = true,
                    IsActive = true
                };

                _context.Users.Add(adminUser);
                await _context.SaveChangesAsync();

                // Assign Admin role
                var adminRole = await _context.Roles.FirstAsync(r => r.Name == "Admin");
                var userRole = new UserRole
                {
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id,
                    AssignedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.UserRoles.Add(userRole);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created default admin user: admin@avyyan.com / admin123");
            }
        }

        private async Task SeedRolePageAccessesAsync()
        {
            var adminRole = await _context.Roles.FirstAsync(r => r.Name == "Admin");
            var managerRole = await _context.Roles.FirstAsync(r => r.Name == "Manager");
            var employeeRole = await _context.Roles.FirstAsync(r => r.Name == "Employee");
            var userRole = await _context.Roles.FirstAsync(r => r.Name == "User");

            var allPageAccesses = await _context.PageAccesses.ToListAsync();

            // Admin - Full access to everything
            foreach (var pageAccess in allPageAccesses)
            {
                if (!await _context.RolePageAccesses.AnyAsync(rpa => rpa.RoleId == adminRole.Id && rpa.PageAccessId == pageAccess.Id))
                {
                    _context.RolePageAccesses.Add(new RolePageAccess
                    {
                        RoleId = adminRole.Id,
                        PageAccessId = pageAccess.Id,
                        CanView = true,
                        CanCreate = true,
                        CanEdit = true,
                        CanDelete = true,
                        CanExport = true,
                        GrantedAt = DateTime.UtcNow
                    });
                }
            }

            // Manager - Most access except user management
            var managerPages = allPageAccesses.Where(pa => 
                pa.Category != "Administration" || pa.PageUrl == "/settings").ToList();
            
            foreach (var pageAccess in managerPages)
            {
                if (!await _context.RolePageAccesses.AnyAsync(rpa => rpa.RoleId == managerRole.Id && rpa.PageAccessId == pageAccess.Id))
                {
                    _context.RolePageAccesses.Add(new RolePageAccess
                    {
                        RoleId = managerRole.Id,
                        PageAccessId = pageAccess.Id,
                        CanView = true,
                        CanCreate = pageAccess.Category == "Operations",
                        CanEdit = pageAccess.Category == "Operations",
                        CanDelete = false,
                        CanExport = true,
                        GrantedAt = DateTime.UtcNow
                    });
                }
            }

            // Employee - Basic operational access
            var employeePages = allPageAccesses.Where(pa => 
                pa.Category == "Main" || 
                pa.Category == "Operations" || 
                pa.Category == "Communication" || 
                pa.Category == "Personal").ToList();
            
            foreach (var pageAccess in employeePages)
            {
                if (!await _context.RolePageAccesses.AnyAsync(rpa => rpa.RoleId == employeeRole.Id && rpa.PageAccessId == pageAccess.Id))
                {
                    _context.RolePageAccesses.Add(new RolePageAccess
                    {
                        RoleId = employeeRole.Id,
                        PageAccessId = pageAccess.Id,
                        CanView = true,
                        CanCreate = false,
                        CanEdit = false,
                        CanDelete = false,
                        CanExport = false,
                        GrantedAt = DateTime.UtcNow
                    });
                }
            }

            // User - Basic access
            var userPages = allPageAccesses.Where(pa => 
                pa.Category == "Main" || 
                pa.Category == "Communication" || 
                pa.Category == "Personal").ToList();
            
            foreach (var pageAccess in userPages)
            {
                if (!await _context.RolePageAccesses.AnyAsync(rpa => rpa.RoleId == userRole.Id && rpa.PageAccessId == pageAccess.Id))
                {
                    _context.RolePageAccesses.Add(new RolePageAccess
                    {
                        RoleId = userRole.Id,
                        PageAccessId = pageAccess.Id,
                        CanView = true,
                        CanCreate = false,
                        CanEdit = false,
                        CanDelete = false,
                        CanExport = false,
                        GrantedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded role page accesses");
        }
    }
}
