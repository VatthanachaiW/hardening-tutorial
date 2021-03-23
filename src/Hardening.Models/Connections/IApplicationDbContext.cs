using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Hardening.Models.Connections
{
  public interface IApplicationDbContext : IDisposable
  {
    DatabaseFacade Database { get; }
    DbSet<T> Set<T>() where T : class;
    EntityEntry Entry(object entity);
    ChangeTracker ChangeTracker { get; }
    bool SaveChange();
    bool SaveChange(bool acceptAllChangesOnSuccess);

    Task<bool> SaveChangeAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken));

    Task<bool> SaveChangeAsync(CancellationToken cancellationToken = default(CancellationToken));
  }
}