using AcxiomCRM.Data; using AcxiomCRM.Models; using AcxiomCRM.Services; using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Identity; using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore;
namespace AcxiomCRM.Controllers;
[Authorize] public class LeadsController : BaseController
{
 private readonly ApplicationDbContext _db; private readonly IAuditService _audit;
 public LeadsController(ApplicationDbContext db, UserManager<ApplicationUser> users, IAuditService audit):base(users){_db=db;_audit=audit;}
 public async Task<IActionResult> Index(string? search,string? status){var q=_db.Leads.AsQueryable();if(IsSales)q=q.Where(x=>x.AssignedTo==CurrentUserId);if(!string.IsNullOrWhiteSpace(search))q=q.Where(x=>x.LeadName.Contains(search)||(x.CompanyName??"").Contains(search));if(!string.IsNullOrWhiteSpace(status))q=q.Where(x=>x.Status==status);return View(await q.OrderByDescending(x=>x.CreatedDate).ToListAsync());}
 public IActionResult Create()=>View(new Lead{LeadCode=$"LED-{DateTime.UtcNow:yyyyMMddHHmmss}"});
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult>Create(Lead model){if(!ModelState.IsValid)return View(model);model.AssignedTo=CurrentUserId;model.CreatedDate=DateTime.UtcNow;_db.Leads.Add(model);await _db.SaveChangesAsync();await _audit.LogAsync("Create","Lead",model.LeadId.ToString(),newValue:model);return RedirectToAction(nameof(Index));}
 public async Task<IActionResult>Edit(int id){var x=await _db.Leads.FindAsync(id);if(x==null||(IsSales&&x.AssignedTo!=CurrentUserId))return NotFound();return View(x);}
 [HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult>Edit(int id,Lead model){var x=await _db.Leads.FindAsync(id);if(x==null||(IsSales&&x.AssignedTo!=CurrentUserId))return NotFound();if(!ModelState.IsValid)return View(model);var old=new{x.LeadName,x.Status,x.Priority,x.ExpectedValue};x.LeadName=model.LeadName;x.Email=model.Email;x.Phone=model.Phone;x.CompanyName=model.CompanyName;x.Source=model.Source;x.Status=model.Status;x.Priority=model.Priority;x.ExpectedValue=model.ExpectedValue;await _db.SaveChangesAsync();await _audit.LogAsync("Update","Lead",id.ToString(),old,x);return RedirectToAction(nameof(Index));}
 [Authorize(Roles="Admin,Manager")][HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult>Delete(int id){var x=await _db.Leads.FindAsync(id);if(x==null)return NotFound();_db.Leads.Remove(x);await _db.SaveChangesAsync();await _audit.LogAsync("Delete","Lead",id.ToString(),oldValue:x);return RedirectToAction(nameof(Index));}
}
