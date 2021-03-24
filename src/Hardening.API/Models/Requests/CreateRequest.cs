using System;

namespace Hardening.API.Models.Requests
{
  public class CreateRequest
  {
    public string Detail { get; set; }
    public DateTime DueDate { get; set; }
  }
}