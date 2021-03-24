using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Hardening.Models.Domains
{
  public partial class ApplicationUser : IdentityUser<Guid>
  {
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public ICollection<Todo> Todos { get; set; }
  }

  public partial class ApplicationUser
  {
    public string Fullname => $"{Firstname} {Lastname}";
  }
}