using AcxiomCRM.Data;
using AcxiomCRM.Models;
using AcxiomCRM.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;

[Microsoft.AspNetCore.Authorization.Authorize]
public class DashboardController : BaseController
{
    private readonly ApplicationDbContext _db;
    public DashboardController(ApplicationDbContext db, UserManager<ApplicationUser> users) : base(users) => _db = db;

    public async Task<IActionResult> Index()
    {
        var vm = new DashboardViewModel();
        var sales = IsSales;

        vm.TotalCustomers = sales
            ? await _db.Customers.CountAsync(x => x.CreatedBy == CurrentUserId)
            : await _db.Customers.CountAsync();

        vm.TotalLeads = sales
            ? await _db.Leads.CountAsync(x => x.AssignedTo == CurrentUserId)
            : await _db.Leads.CountAsync();

        vm.OpenLeads = sales
            ? await _db.Leads.CountAsync(x => x.AssignedTo == CurrentUserId && x.Status != "Lost" && x.Status != "Converted")
            : await _db.Leads.CountAsync(x => x.Status != "Lost" && x.Status != "Converted");

        vm.TotalOpportunities = sales
            ? await _db.Opportunities.CountAsync(x => x.AssignedTo == CurrentUserId)
            : await _db.Opportunities.CountAsync();

        vm.OpenOpportunities = sales
            ? await _db.Opportunities.CountAsync(x => x.AssignedTo == CurrentUserId && x.Status == "Open")
            : await _db.Opportunities.CountAsync(x => x.Status == "Open");

        vm.WonOpportunities = sales
            ? await _db.Opportunities.CountAsync(x => x.AssignedTo == CurrentUserId && x.Stage == "Won")
            : await _db.Opportunities.CountAsync(x => x.Stage == "Won");

        vm.LostOpportunities = sales
            ? await _db.Opportunities.CountAsync(x => x.AssignedTo == CurrentUserId && x.Stage == "Lost")
            : await _db.Opportunities.CountAsync(x => x.Stage == "Lost");

        // SQLite cannot translate Sum(decimal). Load only the Amount column and
        // perform the decimal aggregation in .NET instead.
        var pipelineQuery = sales
            ? _db.Opportunities.Where(x => x.AssignedTo == CurrentUserId && x.Status == "Open")
            : _db.Opportunities.Where(x => x.Status == "Open");
        var pipelineAmounts = await pipelineQuery.Select(x => x.Amount).ToListAsync();
        vm.TotalPipelineValue = pipelineAmounts.Sum();

        var leads = sales ? _db.Leads.Where(x => x.AssignedTo == CurrentUserId) : _db.Leads;
        vm.LeadStatuses = await leads
            .GroupBy(x => x.Status)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count);

        var opps = sales ? _db.Opportunities.Where(x => x.AssignedTo == CurrentUserId) : _db.Opportunities;
        vm.OpportunityStages = await opps
            .GroupBy(x => x.Stage)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count);

        return View(vm);
    }
}
