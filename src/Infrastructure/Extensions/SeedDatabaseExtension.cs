using Domain.Models;
using Infrastructure.Database;
using Infrastructure.Database.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Extensions
{
    public static class SeedDatabaseExtension
    {
        public static void CreateDbIfNotExists(this IHost host)
        {
            using var scope = host.Services.CreateScope();

            // Obtén la configuración y crea un TenantService manualmente
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var baseConnectionString = configuration.GetConnectionString("baseDatabase");

            // Crea un TenantService con valores fijos para el seed
            var tenantService = new TenantService(configuration);
            tenantService.SetTenantId("base"); // O el tenant que corresponda
            // Si tu TenantService requiere el connection string, asígnalo
            tenantService.SetConnectionString(baseConnectionString);

            // Crea las opciones del DbContext
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql(baseConnectionString);

            // Crea el contexto directamente, sin factory ni DI
            using var context = new ApplicationDbContext(optionsBuilder.Options, tenantService);

            try
            {
                context.Database.EnsureCreated();
                DbInitializer.SeedDatabase(context);
            }
            catch (Exception ex)
            {
                // Consider adding proper logging here
            }
        }
    }

    public static class DbInitializer
    {
        public static void SeedDatabase(ApplicationDbContext context)
        {
            DateTime now = DateTime.UtcNow;
            // First, seed Tenants
            if (!context.Tenants.Any())
            {
                var tenants = new List<Tenant>
                {
                    new Tenant
                    {
                        TenantId = "tenant1",
                        Name = "Company One",
                        ConnectionString = "Host=localhost;Database=tenant1_db;Username=user;Password=pass",
                        CreatedAt = now,
                        UpdatedAt = now
                    },
                    new Tenant
                    {
                        TenantId = "tenant2",
                        Name = "Company Two",
                        ConnectionString = "Host=localhost;Database=tenant2_db;Username=user;Password=pass",
                        CreatedAt = now,
                        UpdatedAt = now
                    }
                };
                context.Tenants.AddRange(tenants);
                context.SaveChanges();
            }

            // Then seed data for each tenant
            var tenantIds = new[] { "tenant1", "tenant2" };
            foreach (var tenantId in tenantIds)
            {
                // Seed Roles for each tenant
                if (!context.Roles.Any(r => r.TenantId == tenantId))
                {
                    var roles = new List<Role>
                    {
                        new Role { Name = "Admin", TenantId = tenantId, UserRoles = [], RolePermissions = [] },
                        new Role { Name = "Manager", TenantId = tenantId, UserRoles = [], RolePermissions = [] },
                        new Role { Name = "User", TenantId = tenantId, UserRoles = [], RolePermissions = [] }
                    };
                    context.Roles.AddRange(roles);
                    context.SaveChanges();
                }

                // Seed Permissions for each tenant
                if (!context.Permissions.Any(p => p.TenantId == tenantId))
                {
                    var seedPermissions = new List<Permission>
                    {
                        new Permission { Screen = "Dashboard", Action = "View", TenantId = tenantId },
                        new Permission { Screen = "ManageUsers", Action = "Delete", TenantId = tenantId },
                        new Permission { Screen = "Reports", Action = "View", TenantId = tenantId },
                        new Permission { Screen = "Settings", Action = "Edit", TenantId = tenantId }
                    };
                    context.Permissions.AddRange(seedPermissions);
                    context.SaveChanges();
                }

                // Get the roles for this tenant for reference
                var adminRole = context.Roles.First(r => r.Name == "Admin" && r.TenantId == tenantId);
                var managerRole = context.Roles.First(r => r.Name == "Manager" && r.TenantId == tenantId);
                var userRole = context.Roles.First(r => r.Name == "User" && r.TenantId == tenantId);

                // Seed Users for each tenant
                if (!context.Users.Any(u => u.TenantId == tenantId))
                {
                    List<User> users;
                    if (tenantId == "tenant1")
                    {
                        users = new List<User>
                        {
                            new User
                            {
                                Idn = Guid.NewGuid().ToString(),
                                FirstName = $"Alice_{tenantId}",
                                LastName = "Admin",
                                Email = $"admin@{tenantId}.com",
                                PasswordHash = "hashed_password_admin",
                                IsActive = true,
                                CreatedAt = now,
                                UpdatedAt = now,
                                TenantId = tenantId,
                                UserRoles = new List<UserRole>()
                            },
                            new User
                            {
                                Idn = Guid.NewGuid().ToString(),
                                FirstName = $"Bob_{tenantId}",
                                LastName = "Manager",
                                Email = $"manager@{tenantId}.com",
                                PasswordHash = "hashed_password_manager",
                                IsActive = true,
                                CreatedAt = now,
                                UpdatedAt = now,
                                TenantId = tenantId,
                                UserRoles = new List<UserRole>()
                            },
                            new User
                            {
                                Idn = Guid.NewGuid().ToString(),
                                FirstName = $"Charlie_{tenantId}",
                                LastName = "User",
                                Email = $"user@{tenantId}.com",
                                PasswordHash = "hashed_password_user",
                                IsActive = true,
                                CreatedAt = now,
                                UpdatedAt = now,
                                TenantId = tenantId,
                                UserRoles = new List<UserRole>()
                            }
                        };
                    }
                    else // tenant2
                    {
                        users = new List<User>
                        {
                            new User
                            {
                                Idn = Guid.NewGuid().ToString(),
                                FirstName = $"Dana_{tenantId}",
                                LastName = "Admin",
                                Email = $"admin@{tenantId}.com",
                                PasswordHash = "hashed_password_admin",
                                IsActive = true,
                                CreatedAt = now,
                                UpdatedAt = now,
                                TenantId = tenantId,
                                UserRoles = new List<UserRole>()
                            },
                            new User
                            {
                                Idn = Guid.NewGuid().ToString(),
                                FirstName = $"Evan_{tenantId}",
                                LastName = "Manager",
                                Email = $"manager@{tenantId}.com",
                                PasswordHash = "hashed_password_manager",
                                IsActive = true,
                                CreatedAt = now,
                                UpdatedAt = now,
                                TenantId = tenantId,
                                UserRoles = new List<UserRole>()
                            },
                            new User
                            {
                                Idn = Guid.NewGuid().ToString(),
                                FirstName = $"Fiona_{tenantId}",
                                LastName = "User",
                                Email = $"user@{tenantId}.com",
                                PasswordHash = "hashed_password_user",
                                IsActive = true,
                                CreatedAt = now,
                                UpdatedAt = now,
                                TenantId = tenantId,
                                UserRoles = new List<UserRole>()
                            }
                        };
                    }
                    context.Users.AddRange(users);
                    context.SaveChanges();
                }

                // Always ensure UserRoles are present for each user/role/tenant
                var admin = context.Users.First(u => u.Email == $"admin@{tenantId}.com");
                var manager = context.Users.First(u => u.Email == $"manager@{tenantId}.com");
                var user = context.Users.First(u => u.Email == $"user@{tenantId}.com");

                var userRolesToAdd = new List<UserRole>();
                if (!context.UserRoles.Any(ur => ur.UserId == admin.Id && ur.RoleId == adminRole.Id && ur.TenantId == tenantId))
                    userRolesToAdd.Add(new UserRole { UserId = admin.Id, RoleId = adminRole.Id, TenantId = tenantId });
                if (!context.UserRoles.Any(ur => ur.UserId == manager.Id && ur.RoleId == managerRole.Id && ur.TenantId == tenantId))
                    userRolesToAdd.Add(new UserRole { UserId = manager.Id, RoleId = managerRole.Id, TenantId = tenantId });
                if (!context.UserRoles.Any(ur => ur.UserId == user.Id && ur.RoleId == userRole.Id && ur.TenantId == tenantId))
                    userRolesToAdd.Add(new UserRole { UserId = user.Id, RoleId = userRole.Id, TenantId = tenantId });

                if (userRolesToAdd.Count > 0)
                {
                    context.UserRoles.AddRange(userRolesToAdd);
                    context.SaveChanges();
                }

                if (!context.RolePermissions.Any())
                {
                    // Get the permissions for this tenant
                    var permissions = context.Permissions.Where(p => p.TenantId == tenantId).ToList();

                    // Always ensure RolePermissions are present for each role/permission/tenant
                    var rolePermissionsToAdd = new List<RolePermission>();

                    // Admin gets all permissions
                    foreach (var permission in permissions)
                    {
                        if (!context.RolePermissions.Any(rp => rp.RoleId == adminRole.Id && rp.PermissionId == permission.Id && rp.TenantId == tenantId))
                            rolePermissionsToAdd.Add(new RolePermission { RoleId = adminRole.Id, PermissionId = permission.Id, TenantId = tenantId });
                    }

                    // Manager gets Dashboard, ManageUsers, Reports
                    foreach (var permission in permissions.Where(p =>
                        p.Screen == "Dashboard" ||
                        p.Screen == "ManageUsers" ||
                        p.Screen == "Reports"))
                    {
                        if (!context.RolePermissions.Any(rp => rp.RoleId == managerRole.Id && rp.PermissionId == permission.Id && rp.TenantId == tenantId))
                            rolePermissionsToAdd.Add(new RolePermission { RoleId = managerRole.Id, PermissionId = permission.Id, TenantId = tenantId });
                    }

                    // User gets Dashboard and Reports only
                    foreach (var permission in permissions.Where(p =>
                        p.Screen == "Dashboard" ||
                        p.Screen == "Reports"))
                    {
                        if (!context.RolePermissions.Any(rp => rp.RoleId == userRole.Id && rp.PermissionId == permission.Id && rp.TenantId == tenantId))
                            rolePermissionsToAdd.Add(new RolePermission { RoleId = userRole.Id, PermissionId = permission.Id, TenantId = tenantId });
                    }

                    if (rolePermissionsToAdd.Count > 0)
                    {
                        context.RolePermissions.AddRange(rolePermissionsToAdd);
                        context.SaveChanges();
                    }
                }
            }
        }
    }
}
