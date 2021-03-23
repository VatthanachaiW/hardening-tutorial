using System;
using Newtonsoft.Json;

namespace Hardening.API.Dtos
{
  [JsonObject]
  public class JsonTokenDto
  {
    /// <summary>
    /// Token
    /// </summary>
    [JsonProperty]
    public string Token { get; set; }

    /// <summary>
    /// Expires
    /// </summary>
    [JsonProperty]
    public long Expires { get; set; }

    /// <summary>
    /// Token expire on
    /// </summary>
    [JsonProperty]
    public DateTime ExpireOn { get; set; }
  }
}