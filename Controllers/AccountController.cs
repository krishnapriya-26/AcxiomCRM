using AcxiomCRM.Models;
using AcxiomCRM.Services;
using AcxiomCRM.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcxiomCRM.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signIn;
    private readonly UserManager<ApplicationUser> _users;
    private readonly IAuditService _audit;
    public AccountController(SignInManager<ApplicationUser> signIn, UserManager<ApplicationUser> users, IAuditService audit) { _signIn = signIn; _users = users; _audit = audit; }

    [AllowAnonymous] public IActionResult Login(string? returnUrl = null) => View(new LoginViewModel { });

    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await _users.FindByEmailAsync(model.Email);
        if (user == null || !user.IsActive)
        {
            ModelState.AddModelError("", "Invalid credentials or inactive account.");
            return View(model);
        }
        var result = await _signIn.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);
        if (result.Succeeded)
        {
            await _audit.LogAsync("Login", "Authentication", user.Id);
            return LocalRedirect(returnUrl ?? "/Dashboard");
        }
        await _audit.LogAsync("Failed Login", "Authentication", user.Id, result: result.IsLockedOut ? "LockedOut" : "Rejected");
        ModelState.AddModelError("", result.IsLockedOut ? "Account is temporarily locked." : "Invalid credentials.");
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        var id = _users.GetUserId(User);
        await _signIn.SignOutAsync();
        if (id != null) await _audit.LogAsync("Logout", "Authentication", id);
        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous] public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FullName = model.FullName, EmailConfirmed = true, IsActive = true };
        var result = await _users.CreateAsync(user, model.Password);
        if (!result.Succeeded) { foreach (var e in result.Errors) ModelState.AddModelError("", e.Description); return View(model); }
        await _users.AddToRoleAsync(user, "SalesExecutive");
        await _audit.LogAsync("Register", "User", user.Id);
        await _signIn.SignInAsync(user, false);
        return RedirectToAction("Index", "Dashboard");
    }

    public IActionResult AccessDenied() => View();
}
