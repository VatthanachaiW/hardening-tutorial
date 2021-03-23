using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Hardening.API.Models.Requests
{
  [JsonObject]
  public class SignUpRequest
  {
    [Required] [JsonProperty] public string Username { get; set; }
    [Required] [JsonProperty] public string Email { get; set; }
    [Required] [JsonProperty] public string Firstname { get; set; }
    [Required] [JsonProperty] public string Lastname { get; set; }
    [Required] [JsonProperty] public string Password { get; set; }
    [Required] [JsonProperty] public string ConfirmPassword { get; set; }
  }
}