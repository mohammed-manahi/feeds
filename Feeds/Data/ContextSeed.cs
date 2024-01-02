using Feeds.Models;
using Feeds.Utilities;
using Microsoft.AspNetCore.Identity;

namespace Feeds.Data;

public  static class ContextSeed
{
    public static async Task SeedUserRoles(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Seed application user roles to database
        foreach (var role in Enum.GetNames(typeof(ApplicationUserRoles)))
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(ApplicationUserRoles.Admin.ToString()));
                await roleManager.CreateAsync(new IdentityRole(ApplicationUserRoles.Moderator.ToString()));
                await roleManager.CreateAsync(new IdentityRole(ApplicationUserRoles.User.ToString()));
            }
        }
        
    }
    
    public static async Task SeedSuperAdmin(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Seed super user (admin)
        var defaultUser = new ApplicationUser 
        { 
            UserName = "admin@email.com", 
            Email = "admin@email.com",
            FirstName = "Admin",
            LastName = "User",
            EmailConfirmed = true, 
            PhoneNumberConfirmed = true 
        };
        if (userManager.Users.All(u => u.Id != defaultUser.Id))
        {
            var user = await userManager.FindByEmailAsync(defaultUser.Email);
            if(user==null)
            {
                await userManager.CreateAsync(defaultUser, "Admin@123");
                await userManager.AddToRoleAsync(defaultUser, ApplicationUserRoles.Admin.ToString());
            }
               
        }
    }
}