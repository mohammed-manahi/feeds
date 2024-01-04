using Feeds.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Feeds.Areas.Administration.Controllers;

[Area("Administration")]
[Authorize(Roles = nameof(ApplicationUserRoles.Admin))]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}