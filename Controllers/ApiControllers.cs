using AcxiomCRM.Data;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers.Api;

[ApiController, Route("api/customers"), Authorize]
public class CustomersApiController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _users;
    public CustomersApiController(ApplicationDbContext db, UserManager<ApplicationUser> users) { _db = db; _users = users; }
    private IQueryable<Customer> Scoped() => User.IsInRole("SalesExecutive") ? _db.Customers.Where(x => x.CreatedBy == _users.GetUserId(User)) : _db.Customers;

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await Scoped().Select(x => new { x.CustomerId, x.CustomerCode, x.CustomerName, x.Email, x.Phone, x.CompanyName, x.Status, x.CreatedDate }).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var x = await Scoped().FirstOrDefaultAsync(x => x.CustomerId == id);
        return x == null ? NotFound() : Ok(new { x.CustomerId, x.CustomerCode, x.CustomerName, x.Email, x.Phone, x.CompanyName, x.Status, x.CreatedDate });
    }

    [HttpPost]
    public async Task<IActionResult> Post(Customer model)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        if (await _db.Customers.AnyAsync(x => x.Email == model.Email || x.Phone == model.Phone)) return Conflict(new { message = "Email or phone already exists." });
        model.CreatedBy = _users.GetUserId(User) ?? "";
        model.CreatedDate = DateTime.UtcNow;
        _db.Customers.Add(model);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = model.CustomerId }, new { model.CustomerId, model.CustomerCode, model.CustomerName, model.Email, model.Phone, model.CompanyName, model.Status });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, Customer model)
    {
        var x = await Scoped().FirstOrDefaultAsync(x => x.CustomerId == id);
        if (x == null) return NotFound();
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        if (await _db.Customers.AnyAsync(c => c.CustomerId != id && (c.Email == model.Email || c.Phone == model.Phone))) return Conflict(new { message = "Email or phone already exists." });
        x.CustomerName = model.CustomerName; x.Email = model.Email; x.Phone = model.Phone; x.CompanyName = model.CompanyName; x.Address = model.Address; x.City = model.City; x.State = model.State; x.Status = model.Status;
        await _db.SaveChangesAsync();
        return Ok(new { message = "Customer updated." });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var x = await Scoped().FirstOrDefaultAsync(x => x.CustomerId == id);
        if (x == null) return NotFound();
        _db.Customers.Remove(x); await _db.SaveChangesAsync(); return NoContent();
    }
}

[ApiController, Route("api/leads"), Authorize]
public class LeadsApiController : ControllerBase
{
    private readonly ApplicationDbContext _db; private readonly UserManager<ApplicationUser> _users;
    public LeadsApiController(ApplicationDbContext db, UserManager<ApplicationUser> users) { _db = db; _users = users; }
    private IQueryable<Lead> Scoped() => User.IsInRole("SalesExecutive") ? _db.Leads.Where(x => x.AssignedTo == _users.GetUserId(User)) : _db.Leads;

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await Scoped().Select(x => new { x.LeadId, x.LeadCode, x.LeadName, x.Email, x.Phone, x.Source, x.Status, x.Priority, x.ExpectedValue, x.AssignedTo }).ToListAsync());

    [HttpPost]
    public async Task<IActionResult> Post(Lead model)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        model.AssignedTo = _users.GetUserId(User) ?? ""; model.CreatedDate = DateTime.UtcNow;
        _db.Leads.Add(model); await _db.SaveChangesAsync();
        return Created($"/api/leads/{model.LeadId}", new { model.LeadId, model.LeadCode, model.LeadName, model.Status });
    }
}

[ApiController, Route("api/opportunities"), Authorize]
public class OpportunitiesApiController : ControllerBase
{
    private readonly ApplicationDbContext _db; private readonly UserManager<ApplicationUser> _users;
    public OpportunitiesApiController(ApplicationDbContext db, UserManager<ApplicationUser> users) { _db = db; _users = users; }
    private IQueryable<Opportunity> Scoped() => User.IsInRole("SalesExecutive") ? _db.Opportunities.Where(x => x.AssignedTo == _users.GetUserId(User)) : _db.Opportunities;

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await Scoped().Select(x => new { x.OpportunityId, x.OpportunityName, x.CustomerId, x.Amount, x.Stage, x.Probability, x.ExpectedCloseDate, x.Status, x.AssignedTo }).ToListAsync());

    [HttpPost]
    public async Task<IActionResult> Post(Opportunity model)
    {
        if (model.Amount <= 0) return BadRequest(new { message = "Amount must be greater than 0." });
        if (model.Probability is < 0 or > 100) return BadRequest(new { message = "Probability must be between 0 and 100." });
        if (model.Status == "Open" && model.ExpectedCloseDate.Date < DateTime.Today) return BadRequest(new { message = "Expected close date cannot be in the past." });
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        model.AssignedTo = _users.GetUserId(User) ?? ""; model.CreatedDate = DateTime.UtcNow;
        _db.Opportunities.Add(model); await _db.SaveChangesAsync();
        return Created($"/api/opportunities/{model.OpportunityId}", new { model.OpportunityId, model.OpportunityName, model.Amount, model.Stage, model.Probability });
    }
}
