using System;
using Microsoft.AspNetCore.Identity;

namespace Hardening.Models.Domains
{
  public class ApplicationUserToken: IdentityUserToken<Guid> { }
}