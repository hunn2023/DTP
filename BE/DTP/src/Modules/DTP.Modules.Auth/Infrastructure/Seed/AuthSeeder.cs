using DTP.Modules.Auth.Domain.Entities;
using DTP.Modules.Auth.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DTP.Modules.Auth.Infrastructure.Seed
{
    public static class AuthSeeder
    {
        public static async Task SeedAdminAsync(AuthDbContext context)
        {
            var adminRoleName = "Admin";
            var adminEmail = "admin@dtp.com";
            var adminPassword = "Admin@123456";

            var role = await context.Roles
                .FirstOrDefaultAsync(x => x.Name == adminRoleName);

            if (role == null)
            {
                role = new Role
                {
                    Code = adminRoleName.ToUpper(),
                    Name = adminRoleName,
                    Description = "System administrator",
                    IsActive = true,
                    IsSystem = true
                };

                context.Roles.Add(role);
                await context.SaveChangesAsync();
            }

            var user = await context.Users
                .FirstOrDefaultAsync(x => x.Email == adminEmail);

            if (user == null)
            {
                user = new User
                {
                    Email = adminEmail,
                    FullName = "Administrator",
                    EmailConfirmed = true,
                    IsActive = true,
                    PasswordResetVerifyFailedCount = 0
                };

                var passwordHasher = new PasswordHasher<User>();
                user.PasswordHash = passwordHasher.HashPassword(user, adminPassword);

                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            var hasRole = await context.UserRoles
                .AnyAsync(x => x.UserId == user.Id && x.RoleId == role.Id);

            if (!hasRole)
            {
                context.UserRoles.Add(new UserRole
                {
                    UserId = user.Id,
                    RoleId = role.Id
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
