using Entities.IdentityIntities;
using Microsoft.AspNetCore.Identity;

namespace Entities
{
    public class IdentityInitializer
    {
        public static async Task Initialize(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            if (!userManager.Users.Any())
            {
                //Adding Roles
                var roles = new List<ApplicationRole>
                {
                    new ApplicationRole
                    {
                        Id = Guid.NewGuid(),
                        Name = "Admin",
                        NormalizedName = "ADMIN"
                    },
                    new ApplicationRole
                    {
                        Id = Guid.NewGuid(),
                        Name = "Moderator",
                        NormalizedName = "MODERATOR"
                    },
                    new ApplicationRole
                    {
                        Id = Guid.NewGuid(),
                        Name = "User",
                        NormalizedName = "USER"
                    }
                };

                for (int i = 0; i < roles.Count; i++)
                {
                    await roleManager.CreateAsync(roles[i]);
                }

                // Add admin user
                var adminUser = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    PersonName = "Ahmed",
                    UserName = "admin@gmail.com",
                    NormalizedUserName = "ADMIN",
                    Email = "admin@gmail.com",
                    NormalizedEmail = "ADMIN@GMAIL.COM",
                    PhoneNumber = "01092532838",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, "897453alo");

                if (result.Succeeded)
                {
                    await userManager.AddToRolesAsync(adminUser, new[] { "User", "Admin" });
                }
            }
        }
    }
}