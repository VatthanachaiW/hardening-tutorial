using System;
using Newtonsoft.Json;

namespace Hardening.API.Models.Responses
{
  [JsonObject]
  public class GetAllResponse
  {
    [JsonProperty] public Guid Id { get; set; }
    [JsonProperty] public DateTime DueDate { get; set; }
    [JsonProperty] public string Detail { get; set; }
    [JsonProperty] public bool IsActive { get; set; }
  }
}