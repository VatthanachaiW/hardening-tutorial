using System;
using Microsoft.AspNetCore.Identity;

namespace Hardening.Models.Domains
{
  public class ApplicationUserClaim : IdentityUserClaim<Guid> { }
}