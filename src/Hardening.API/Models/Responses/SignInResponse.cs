using System;
using Newtonsoft.Json;

namespace Hardening.API.Models.Responses
{
  [JsonObject]
  public class SignInResponse
  {
    [JsonProperty] public string Username { get; set; }
    [JsonProperty] public string RoleName { get; set; }
    [JsonProperty] public string AccessToken { get; set; }
    [JsonProperty] public DateTime ExpireOn { get; set; }
  }
}