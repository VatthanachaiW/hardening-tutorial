using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Hardening.API.Dtos;
using Hardening.API.Services;
using Hardening.Models.Connections;
using Hardening.Models.Domains;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Hardening.API
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.Configure<JWTConfig>(options => Configuration.GetSection(nameof(JWTConfig)).Bind(options));

      services.AddDbContext<ApplicationDbContext>(options => { options.UseSqlite("Data Source=TodoDb.sqlite", sqlOptions => { sqlOptions.MigrationsHistoryTable("_migrations", "mg"); }); });
      services.AddControllers();
      services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "Hardening.API", Version = "v1"}); });
      services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
          options.Password.RequiredLength = 8;
          options.Password.RequireNonAlphanumeric = false;
          options.Password.RequireUppercase = true;
          options.Password.RequireDigit = true;
          options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(1);
          options.Lockout.MaxFailedAccessAttempts = 5;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

      var jwtConfig = Configuration.GetSection(nameof(JWTConfig)).Get<JWTConfig>();
      services.AddAuthentication(options =>
        {
          options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
          options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
          options.RequireHttpsMetadata = false;
          options.SaveToken = true;

          options.TokenValidationParameters = new TokenValidationParameters
          {
            ValidateIssuer = jwtConfig.ValidateIssuer,
            ValidateAudience = jwtConfig.ValidateAudience,
            ValidateLifetime = jwtConfig.ValidateLifetime,
            ValidateIssuerSigningKey = jwtConfig.ValidateIssuerSigningKey,
            ValidIssuer = jwtConfig.Issuer,
            ValidAudience = jwtConfig.Issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret)),
            ClockSkew = TimeSpan.Zero
          };

          options.Events = new JwtBearerEvents
          {
            OnAuthenticationFailed = context =>
            {
              if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
              {
                context.Response.Headers.Add("token-expired", "true");
              }

              return Task.CompletedTask;
            }
          };
        });
      services.AddHostedService<Worker>();
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
      builder.RegisterType<ApplicationDbContext>().As<IApplicationDbContext>();
      builder.RegisterType<JwtHandler>().As<IJwtHandler>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hardening.API v1"));
      }

      app.UseHttpsRedirection();

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
  }

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
      RoleManager<ApplicationRole> _roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
      await context.Database.EnsureCreatedAsync(cancellationToken);

      var existing = await _roleManager.FindByNameAsync("Admin");
      if (existing == null)
      {
        var nRole = new ApplicationRole
        {
          Name = "Admin"
        };
        await _roleManager.CreateAsync(nRole);
      }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
  }
}