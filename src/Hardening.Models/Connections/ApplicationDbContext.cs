using System;
using System.Threading;
using System.Threading.Tasks;
using Hardening.Models.Domains;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hardening.Models.Connections
{
  public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>, IApplicationDbContext
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

      base.OnModelCreating(builder);
    }


    private void TodoModelConfiguration(EntityTypeBuilder<Todo> builder)
    {
      builder.HasKey(k => k.Id);
      builder.Property(p => p.Id).HasColumnName("id").ValueGeneratedOnAdd();
      builder.Property(p => p.Detail).HasColumnName("detail");
    }
  }
}