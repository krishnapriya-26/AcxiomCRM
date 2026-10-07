using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AcxiomCRM.Models;

public class ApplicationUser : IdentityUser
{
    [Required, StringLength(120)] public string FullName { get; set; } = "";
    public bool IsActive { get; set; } = true;
}

public class Customer
{
    public int CustomerId { get; set; }
    [Required, StringLength(30)] public string CustomerCode { get; set; } = "";
    [Required, StringLength(120)] public string CustomerName { get; set; } = "";
    [Required, EmailAddress, StringLength(180)] public string Email { get; set; } = "";
    [Required, RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Enter a valid 10-digit Indian mobile number."), StringLength(10)]
public string Phone { get; set; } = "";
    [StringLength(150)] public string? CompanyName { get; set; }
    [StringLength(250)] public string? Address { get; set; }
    [StringLength(80)] public string? City { get; set; }
    [StringLength(80)] public string? State { get; set; }
    [Required] public string Status { get; set; } = "Active";
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = "";
}

public class Lead
{
    public int LeadId { get; set; }
    [Required, StringLength(30)] public string LeadCode { get; set; } = "";
    [Required, StringLength(120)] public string LeadName { get; set; } = "";
    [EmailAddress, StringLength(180)] public string? Email { get; set; }
    [Phone, StringLength(20)] public string? Phone { get; set; }
    [StringLength(150)] public string? CompanyName { get; set; }
    [StringLength(80)] public string Source { get; set; } = "Website";
    [Required] public string Status { get; set; } = "New";
    [StringLength(30)] public string Priority { get; set; } = "Medium";
    [Range(0, double.MaxValue)] public decimal ExpectedValue { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string AssignedTo { get; set; } = "";
}

public class Opportunity
{
    public int OpportunityId { get; set; }
    [Required, StringLength(150)] public string OpportunityName { get; set; } = "";
    public int CustomerId { get; set; }
    public int? LeadId { get; set; }
    [Range(0.01, double.MaxValue)] public decimal Amount { get; set; }
    [Required] public string Stage { get; set; } = "Qualification";
    [Range(0, 100)] public int Probability { get; set; }
    [Required, DataType(DataType.Date)] public DateTime ExpectedCloseDate { get; set; } = DateTime.Today.AddDays(30);
    [Required] public string Status { get; set; } = "Open";
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string AssignedTo { get; set; } = "";

    public decimal WeightedPipeline => Amount * Probability / 100m;
}

public class FollowUp
{
    public int FollowUpId { get; set; }
    public int? CustomerId { get; set; }
    public int? LeadId { get; set; }
    [Required, DataType(DataType.Date)] public DateTime FollowUpDate { get; set; } = DateTime.Today;
    [Required, StringLength(60)] public string FollowUpType { get; set; } = "Call";
    [StringLength(500)] public string? Remarks { get; set; }
    [Required] public string Status { get; set; } = "Planned";
    public string AssignedTo { get; set; } = "";
}

public class Activity
{
    public int ActivityId { get; set; }
    [Required] public string ActivityType { get; set; } = "Call";
    [Required, StringLength(150)] public string Subject { get; set; } = "";
    [StringLength(1000)] public string? Description { get; set; }
    [Required, DataType(DataType.Date)] public DateTime ActivityDate { get; set; } = DateTime.Today;
    public int? CustomerId { get; set; }
    public int? LeadId { get; set; }
    public string AssignedTo { get; set; } = "";
    public string Status { get; set; } = "Open";
}

public class AuditLog
{
    public long AuditLogId { get; set; }
    public string UserId { get; set; } = "";
    [Required] public string Action { get; set; } = "";
    [Required] public string EntityName { get; set; } = "";
    public string? RecordId { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string? IpAddress { get; set; }
    public string? Result { get; set; }
}
