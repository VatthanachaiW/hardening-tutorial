using System;
using Microsoft.AspNetCore.Mvc;

namespace Hardening.API.Models.Requests
{
  public class GetAllRequest
  {
    [FromQuery(Name = "search")] public string Search { get; set; }
    [FromQuery(Name = "due_date")] public DateTime? DueDate { get; set; }
    [FromQuery(Name = "end_due_date")] public DateTime? EndDueDate { get; set; }
    [FromQuery(Name = "is_active")] public bool IsActive { get; set; }
  }
}