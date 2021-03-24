using System;

namespace Hardening.Models.Domains
{
  public class Todo
  {
    public Guid Id { get; set; }
    public DateTime DueDate { get; set; }
    public string Detail { get; set; }
    public Guid CreatedById { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid ModifiedById { get; set; }
    public DateTime ModifiedOn { get; set; }
    public bool IsActive { get; set; }
  }
}