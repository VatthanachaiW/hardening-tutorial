using System.Collections.Generic;
using Hardening.API.Dtos;
using Hardening.Models.Domains;

namespace Hardening.API.Services
{
  public interface IJwtHandler
  {
    JsonTokenDto CreateWithJwtPayload(ApplicationUser user, IList<string> roles);
  }
}