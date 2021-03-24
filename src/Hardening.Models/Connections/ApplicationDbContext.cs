using System;
using System.Threading;
using System.Threading.Tasks;
using Hardening.Models.Domains;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hardening.Models.Connections
{
  public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid, ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken>, IApplicationDbContext
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public bool SaveChange() => base.SaveChanges() > 0;

    public bool SaveChange(bool acceptAllChangesOnSuccess) => base.SaveChanges(acceptAllChangesOnSuccess) > 0;

    public async Task<bool> SaveChangeAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken)) => await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken) > 0;

    public async Task<bool> SaveChangeAsync(CancellationToken cancellationToken = default(CancellationToken)) => await base.SaveChangesAsync(cancellationToken) > 0;

    protected override void OnModelCreating(ModelBuilder builder)
    {
      TodoModelConfiguration(builder.Entity<Todo>());
      /*
      string adminId = "8e6dd84f-51e2-4c1f-bbee-dc85606bd00d";
      string adminRoleId = "537d7e10-27ba-4bb4-9b80-1da23d3ca66d";
      string memberRoleId = "dab748ec-8873-4d25-9942-6cc45e47b720";
      builder.Entity<ApplicationRole>().HasData(new ApplicationRole
      {
        Name = "Admin",
        NormalizedName = "Admin",
        Id = Guid.Parse(adminRoleId),
        ConcurrencyStamp = adminRoleId
      });

      builder.Entity<ApplicationRole>().HasData(new ApplicationRole
      {
        Name = "Member",
        NormalizedName = "Member",
        Id = Guid.Parse(memberRoleId),
        ConcurrencyStamp = memberRoleId
      });

      var adminUser = new ApplicationUser
      {
        UserName = "admin",
        Email = "admin@todo.xyz",
        EmailConfirmed = true,
        Firstname = "System",
        Lastname = "Admin",
        Id = Guid.Parse(adminId)
      };

      var passwordHasher = new PasswordHasher<ApplicationUser>();
      adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "P@ssw0rd!");
      builder.Entity<ApplicationUser>().HasData(adminUser);

      builder.Entity<ApplicationUserRole>().HasData(new ApplicationUserRole
      {
        RoleId = Guid.Parse(adminRoleId),
        UserId = Guid.Parse(adminId)
      });
      */
      base.OnModelCreating(builder);
    }


    private void TodoModelConfiguration(EntityTypeBuilder<Todo> builder)
    {
      builder.HasKey(k => k.Id);
      builder.Property(p => p.Id).HasColumnName("id").ValueGeneratedOnAdd();
      builder.Property(p => p.DueDate).HasColumnName("due_date");
      builder.Property(p => p.Detail).HasColumnName("detail");
      builder.Property(p => p.CreatedById).HasColumnName("created_by");
      builder.Property(p => p.CreatedOn).HasColumnName("created_on");
      builder.Property(p => p.ModifiedById).HasColumnName("modified_by");
      builder.Property(p => p.ModifiedOn).HasColumnName("modified_on");
      builder.Property(p => p.IsActive).HasColumnName("is_active");
    }
  }
}