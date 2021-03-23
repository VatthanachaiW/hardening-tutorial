using Newtonsoft.Json;

namespace Hardening.API.Models.Responses
{
  [JsonObject]
  public class SignUpResponse
  {
    [JsonProperty] public string Email { get; set; }
    [JsonProperty] public bool IsSuccess { get; set; }
    [JsonProperty] public string Message { get; set; }
  }
}