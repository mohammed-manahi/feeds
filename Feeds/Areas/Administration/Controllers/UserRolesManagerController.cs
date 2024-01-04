using Feeds.Models;
using Feeds.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Feeds.Areas.Administration.Controllers;

[Area("Administration")]
[Authorize(Roles = nameof(ApplicationUserRoles.Admin))]
public class UserRolesManagerController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserRolesManagerController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Inject role manager and user manager 
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        // Get all users 
        var users = await _userManager.Users.ToListAsync();
        var userRolesViewModel = new List<UserRolesViewModel>();
        foreach (ApplicationUser user in users)
        {
            // Get each user details and assigned roles
            var thisViewModel = new UserRolesViewModel();
            thisViewModel.UserId = user.Id;
            thisViewModel.Email = user.Email;
            thisViewModel.FirstName = user.FirstName;
            thisViewModel.LastName = user.LastName;
            // Invoke GetUserRoles method to get user roles
            thisViewModel.Roles = await GetUserRoles(user);
            userRolesViewModel.Add(thisViewModel);
        }

        return View(userRolesViewModel);
    }

    public async Task<IActionResult> Manage(string userId)
    {
        // Get user id
        ViewBag.userId = userId;
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
            return NotFound();
        }

        ViewBag.UserName = user.UserName;
        var model = new List<ManageUserRolesViewModel>();
        foreach (var role in _roleManager.Roles)
        {
            var userRolesViewModel = new ManageUserRolesViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name
            };
            // Get selected user roles
            if ( _userManager.IsInRoleAsync(user, role.Name).GetAwaiter().GetResult())
            {
                userRolesViewModel.Selected = true;
            }
            else
            {
                userRolesViewModel.Selected = false;
            }

            model.Add(userRolesViewModel);
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Manage(List<ManageUserRolesViewModel> model, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return View();
        }
        
        var roles = await _userManager.GetRolesAsync(user);
        // Remove user roles
        var result = await _userManager.RemoveFromRolesAsync(user, roles);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Cannot remove user existing roles");
            return View(model);
        }
        // Add user roles
        result = await _userManager.AddToRolesAsync(user, model.Where(x => x.Selected).Select(y => y.RoleName));
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Cannot add selected roles to user");
            return View(model);
        }

        return RedirectToAction("Index");
    }


    private async Task<List<string>> GetUserRoles(ApplicationUser user)
    {
        return new List<string>(await _userManager.GetRolesAsync(user));
    }
}