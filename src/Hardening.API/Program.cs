using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;

namespace Hardening.API
{
  public class Program
  {
    public static void Main(string[] args)
    {
      CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
        .UseServiceProviderFactory(new AutofacServiceProviderFactory())
        .ConfigureAppConfiguration((context, config) =>
        {
          config.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
          config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        })
        .ConfigureWebHostDefaults(builder =>
        {
          builder.UseContentRoot(AppContext.BaseDirectory);
          builder.UseStartup<Startup>();
          builder.UseWebRoot("AccessToken");
        });
  }
}