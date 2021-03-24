using System.Collections.Generic;
using System.Threading.Tasks;
using Hardening.Models.Domains;
using Microsoft.AspNetCore.Identity;

namespace Hardening.Models.Helpers
{
  public static class DatabaseInitializer
  {
    public static async Task SeedDataAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
      await SeedRoleAsync(roleManager);
      await SeedUserAsync(userManager);
    }

    private static async Task SeedRoleAsync(RoleManager<ApplicationRole> roleManager)
    {
      var roles = new List<ApplicationRole>
      {
        new ApplicationRole
        {
          Name = "Admin",
          NormalizedName = "Admin"
        },
        new ApplicationRole
        {
          Name = "Member",
          NormalizedName = "Member"
        }
      };

      foreach (var role in roles)
      {
        if (!await roleManager.RoleExistsAsync(role.Name))
        {
          await roleManager.CreateAsync(role);
        }
      }
    }

    private static async Task SeedUserAsync(UserManager<ApplicationUser> userManager)
    {
      var adminUser = new ApplicationUser
      {
        UserName = "admin",
        Firstname = "System",
        Lastname = "Admin",
        Email = "admin@todo.xyz",
        EmailConfirmed = true
      };

      if (await userManager.FindByNameAsync(adminUser.UserName) == null)
      {
        var passwordHash = new PasswordHasher<ApplicationUser>();
        var result = await userManager.CreateAsync(adminUser, "P@ssw0rd!");
        if (result.Succeeded)
        {
          await userManager.AddToRoleAsync(adminUser, "admin");
        }
      }
    }
  }
}