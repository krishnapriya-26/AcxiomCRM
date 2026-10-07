using AcxiomCRM.Data;
using AcxiomCRM.Models;
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;

[Authorize]
public class CustomersController : BaseController
{
    private readonly ApplicationDbContext _db; private readonly IAuditService _audit;
    public CustomersController(ApplicationDbContext db, UserManager<ApplicationUser> users, IAuditService audit) : base(users) { _db = db; _audit = audit; }
    public async Task<IActionResult> Index(string? search)
    {
        var q = _db.Customers.AsQueryable();
        if (IsSales) q = q.Where(x => x.CreatedBy == CurrentUserId);
        if (!string.IsNullOrWhiteSpace(search)) q = q.Where(x => x.CustomerName.Contains(search) || x.Email.Contains(search) || x.Phone.Contains(search) || (x.CompanyName ?? "").Contains(search));
        return View(await q.OrderByDescending(x => x.CreatedDate).ToListAsync());
    }
    public IActionResult Create() => View(new Customer { CustomerCode = $"CUS-{DateTime.UtcNow:yyyyMMddHHmmss}" });
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Customer model)
    {
        if (await _db.Customers.AnyAsync(x => x.Email == model.Email)) ModelState.AddModelError(nameof(model.Email), "Email already exists.");
        if (await _db.Customers.AnyAsync(x => x.Phone == model.Phone)) ModelState.AddModelError(nameof(model.Phone), "Phone already exists.");
        if (!ModelState.IsValid) return View(model);
        model.CreatedBy = CurrentUserId; model.CreatedDate = DateTime.UtcNow;
        _db.Customers.Add(model); await _db.SaveChangesAsync(); await _audit.LogAsync("Create", "Customer", model.CustomerId.ToString(), newValue: model); return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Edit(int id)
    { var x = await _db.Customers.FindAsync(id); if (x == null || (IsSales && x.CreatedBy != CurrentUserId)) return NotFound(); return View(x); }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Customer model)
    { var x = await _db.Customers.FindAsync(id); if (x == null || (IsSales && x.CreatedBy != CurrentUserId)) return NotFound();
      if (await _db.Customers.AnyAsync(c => c.CustomerId != id && c.Email == model.Email)) ModelState.AddModelError(nameof(model.Email), "Email already exists.");
      if (await _db.Customers.AnyAsync(c => c.CustomerId != id && c.Phone == model.Phone)) ModelState.AddModelError(nameof(model.Phone), "Phone already exists.");
      if (!ModelState.IsValid) return View(model); var old = new { x.CustomerName, x.Email, x.Phone, x.Status };
      x.CustomerName=model.CustomerName; x.Email=model.Email; x.Phone=model.Phone; x.CompanyName=model.CompanyName; x.Address=model.Address; x.City=model.City; x.State=model.State; x.Status=model.Status;
      await _db.SaveChangesAsync(); await _audit.LogAsync("Update", "Customer", id.ToString(), old, x); return RedirectToAction(nameof(Index)); }
    [Authorize(Roles="Admin,Manager")]
    public async Task<IActionResult> Delete(int id) { var x=await _db.Customers.FindAsync(id); if(x==null)return NotFound(); return View(x); }
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, Authorize(Roles="Admin,Manager")]
    public async Task<IActionResult> DeleteConfirmed(int id) { var x=await _db.Customers.FindAsync(id); if(x==null)return NotFound(); _db.Customers.Remove(x); await _db.SaveChangesAsync(); await _audit.LogAsync("Delete","Customer",id.ToString(),oldValue:x); return RedirectToAction(nameof(Index)); }
}
