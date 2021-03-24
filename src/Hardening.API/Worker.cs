using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hardening.Models.Connections;
using Hardening.Models.Domains;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hardening.API
{
  public class Worker : IHostedService
  {
    private readonly IServiceProvider _serviceProvider;


    public Worker(IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
      using var scope = _serviceProvider.CreateScope();

      var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

      await context.Database.EnsureCreatedAsync(cancellationToken);
    }


    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
  }
}