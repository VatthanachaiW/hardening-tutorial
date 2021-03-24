using System;

namespace Hardening.API.Models.Requests
{
  public class UpdateRequest
  {
    public string Detail { get; set; }
    public DateTime? DueDate { get; set; }
    public bool? IsActive { get; set; }
  }
}