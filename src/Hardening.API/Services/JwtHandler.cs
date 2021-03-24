using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Hardening.API.Dtos;
using Hardening.Models.Domains;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Hardening.API.Services
{
  public class JwtHandler : IJwtHandler
  {
    private readonly JWTConfig _options;
    private readonly SecurityKey _issuerSigningKey;
    private readonly SigningCredentials _signingCredentials;
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

    public JwtHandler(IOptions<JWTConfig> options)
    {
      _options = options.Value;
      _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
      _issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
      _signingCredentials = new SigningCredentials(_issuerSigningKey, SecurityAlgorithms.HmacSha256);
    }

    private static DateTime CheckTokenExpireType(TokenExpireType tokenExpireType, int tokenExpireTime)
    {
      DateTime expire;
      switch (tokenExpireType)
      {
        case TokenExpireType.Minutes:
          expire = DateTime.Now.AddMinutes(Convert.ToDouble(tokenExpireTime));
          break;
        case TokenExpireType.Hours:
          expire = DateTime.Now.AddHours(Convert.ToDouble(tokenExpireTime));
          break;
        case TokenExpireType.Days:
          expire = DateTime.Now.AddDays(Convert.ToDouble(tokenExpireTime));
          break;
        default:
          expire = DateTime.Now.AddMinutes(Convert.ToDouble(tokenExpireTime));
          break;
      }

      return expire;
    }

    public JsonTokenDto CreateWithJwtPayload(ApplicationUser user, IList<string> roles)
    {
      var nowUtc = DateTime.UtcNow;
      var expires = CheckTokenExpireType(_options.TokenExpireType, _options.TokenExpireTime);
      var centuryBegin = new DateTime(1970, 1, 1).ToUniversalTime();
      var exp = (long) (new TimeSpan(expires.Ticks - centuryBegin.Ticks).TotalSeconds);
      var now = (long) (new TimeSpan(nowUtc.Ticks - centuryBegin.Ticks).TotalSeconds);

      var claims = new List<Claim>()
      {
        new Claim(JwtRegisteredClaimNames.Sub, $"{user.Id}"),
        new Claim(JwtRegisteredClaimNames.Jti, $"{Guid.NewGuid()}"),
        new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
        new Claim(JwtRegisteredClaimNames.Iss, _options.Issuer),
        new Claim(JwtRegisteredClaimNames.Iat, $"{now}"),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.NameIdentifier, $"{user.Id}"),
        new Claim(ClaimTypes.Expired, $"{exp}"),
        new Claim(ClaimTypes.Role, string.Join(',', roles)),
      };


      var jwt = new JwtSecurityToken(issuer: _options.Issuer, audience: _options.Issuer, claims: claims, expires: expires, signingCredentials: _signingCredentials);

      var token = _jwtSecurityTokenHandler.WriteToken(jwt);

      return new JsonTokenDto
      {
        Token = token,
        Expires = exp,
        ExpireOn = expires
      };
    }
  }
}