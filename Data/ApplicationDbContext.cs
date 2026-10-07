using AcxiomCRM.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Lead> Leads => Set<Lead>();
    public DbSet<Opportunity> Opportunities => Set<Opportunity>();
    public DbSet<FollowUp> FollowUps => Set<FollowUp>();
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Customer>().HasIndex(x => x.Email).IsUnique();
        builder.Entity<Customer>().HasIndex(x => x.Phone).IsUnique();
        builder.Entity<Customer>().Property(x => x.Status).HasDefaultValue("Active");
        builder.Entity<Lead>().Property(x => x.Status).HasDefaultValue("New");
        builder.Entity<Opportunity>().Property(x => x.Probability).HasDefaultValue(0);
        builder.Entity<FollowUp>().Property(x => x.Status).HasDefaultValue("Planned");
        builder.Entity<Activity>().Property(x => x.Status).HasDefaultValue("Open");

        builder.Entity<Customer>().HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Lead>().HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.AssignedTo).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Opportunity>().HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.AssignedTo).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<FollowUp>().HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.AssignedTo).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Activity>().HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.AssignedTo).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<AuditLog>().HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}
