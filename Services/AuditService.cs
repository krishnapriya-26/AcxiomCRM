using System.Text.Json;
using AcxiomCRM.Data;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Identity;

namespace AcxiomCRM.Services;

public interface IAuditService
{
    Task LogAsync(string action, string entity, string? recordId = null, object? oldValue = null, object? newValue = null, string result = "Success");
}

public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _db;
    private readonly IHttpContextAccessor _http;
    private readonly UserManager<ApplicationUser> _users;
    public AuditService(ApplicationDbContext db, IHttpContextAccessor http, UserManager<ApplicationUser> users) { _db = db; _http = http; _users = users; }

    public async Task LogAsync(string action, string entity, string? recordId = null, object? oldValue = null, object? newValue = null, string result = "Success")
    {
        var principal = _http.HttpContext?.User;
        var id = principal?.Identity?.IsAuthenticated == true
            ? _users.GetUserId(principal) ?? "system"
            : "system";
        _db.AuditLogs.Add(new AuditLog
        {
            UserId = id, Action = action, EntityName = entity, RecordId = recordId,
            OldValue = oldValue == null ? null : JsonSerializer.Serialize(oldValue),
            NewValue = newValue == null ? null : JsonSerializer.Serialize(newValue),
            IpAddress = _http.HttpContext?.Connection.RemoteIpAddress?.ToString(), Result = result
        });
        await _db.SaveChangesAsync();
    }
}
