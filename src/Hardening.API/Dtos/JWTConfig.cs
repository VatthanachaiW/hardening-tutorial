using Hardening.API.Services;

namespace Hardening.API.Dtos
{
  public class JWTConfig
  {
    public string Issuer { get; set; }
    public string Secret { get; set; }
    public bool ValidateIssuer { get; set; }
    public bool ValidateAudience { get; set; }
    public bool ValidateLifetime { get; set; }
    public bool ValidateIssuerSigningKey { get; set; }
    public int TokenExpireTime { get; set; }
    public TokenExpireType TokenExpireType { get; set; }
  }
}