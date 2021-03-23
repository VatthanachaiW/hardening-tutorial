using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Hardening.API.Models.Requests
{
  [JsonObject]
  public class SignInRequest
  {
    [Required]
    [JsonProperty]
    [JsonRequired]
    public string Username { get; set; }

    [Required]
    [JsonProperty]
    [JsonRequired]
    [MinLength(8, ErrorMessage = "Password  must be at least 8 characters.")]
    public string Password { get; set; }
  }
}