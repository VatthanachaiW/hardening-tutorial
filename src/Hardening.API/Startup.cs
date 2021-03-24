using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Hardening.API.Dtos;
using Hardening.API.Services;
using Hardening.Models.Connections;
using Hardening.Models.Domains;
using Hardening.Models.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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

      services.AddDbContext<ApplicationDbContext>(options =>
      {
        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), sqlOptions =>
        {
          sqlOptions.CommandTimeout(5000);
          sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Namespace);
          sqlOptions.EnableRetryOnFailure(5);
        });
      });
      services.AddControllers()
        .AddNewtonsoftJson();

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

    private static NewtonsoftJsonPatchInputFormatter getJsonPatchInputFormatter()
    {
      var builder = new ServiceCollection()
        .AddMvc()
        .AddNewtonsoftJson()
        .Services.BuildServiceProvider();

      return builder
        .GetRequiredService<IOptions<MvcOptions>>()
        .Value
        .InputFormatters
        .OfType<NewtonsoftJsonPatchInputFormatter>()
        .First();
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
      builder.RegisterType<ApplicationDbContext>().As<IApplicationDbContext>();
      builder.RegisterType<JwtHandler>().As<IJwtHandler>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hardening.API v1"));
      }

      app.UseHttpsRedirection();

      app.UseRouting();
      app.UseAuthentication();
      app.UseAuthorization();

      DatabaseInitializer.SeedDataAsync(userManager, roleManager).GetAwaiter().GetResult();

      app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
  }
}