using AcxiomCRM.Data;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;

[Authorize]
public class ReportsController : BaseController
{
    private readonly ApplicationDbContext _db;

    public ReportsController(ApplicationDbContext db, UserManager<ApplicationUser> users)
        : base(users)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var sales = IsSales;
        var leadsQuery = sales
            ? _db.Leads.Where(x => x.AssignedTo == CurrentUserId)
            : _db.Leads;
        var oppsQuery = sales
            ? _db.Opportunities.Where(x => x.AssignedTo == CurrentUserId)
            : _db.Opportunities;

        // SQLite cannot translate Sum(decimal). Grouping/counting can stay in SQL,
        // but the decimal totals are calculated in .NET after loading the needed rows.
        var leads = await leadsQuery
            .Select(x => new { x.Status, x.ExpectedValue })
            .ToListAsync();

        ViewBag.Leads = leads
            .GroupBy(x => x.Status)
            .Select(g => new
            {
                Status = g.Key,
                Count = g.Count(),
                Value = g.Sum(x => x.ExpectedValue)
            })
            .ToList();

        var opportunities = await oppsQuery
            .Select(x => new { x.Stage, x.Amount, x.Probability })
            .ToListAsync();

        ViewBag.Opportunities = opportunities
            .GroupBy(x => x.Stage)
            .Select(g => new
            {
                Stage = g.Key,
                Count = g.Count(),
                Amount = g.Sum(x => x.Amount),
                Weighted = g.Sum(x => x.Amount * x.Probability / 100m)
            })
            .ToList();

        return View();
    }
}
