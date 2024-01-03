using Feeds.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Feeds.Areas.Administration.Controllers;

[Area("Administration")]
[Authorize(Roles = nameof(ApplicationUserRoles.Admin))]
public class DashboardController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public DashboardController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        return View(roles);
    }

    [HttpPost]
    public async Task<IActionResult> AddRole(string roleName)
    {
        if (roleName != null)
        {
            await _roleManager.CreateAsync(new IdentityRole(roleName.Trim()));
        }

        return RedirectToAction("Index");
    }
}