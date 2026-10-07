using AcxiomCRM.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AcxiomCRM.Controllers;

public abstract class BaseController : Controller
{
    protected readonly UserManager<ApplicationUser> UserManager;
    protected BaseController(UserManager<ApplicationUser> userManager) => UserManager = userManager;
    protected string CurrentUserId => UserManager.GetUserId(User) ?? "";
    protected bool IsAdmin => User.IsInRole("Admin");
    protected bool IsManager => User.IsInRole("Manager");
    protected bool IsSales => User.IsInRole("SalesExecutive");
}
