using System;
using System.Linq;
using System.Threading.Tasks;
using Hardening.API.Models.Requests;
using Hardening.API.Models.Responses;
using Hardening.Models.Connections;
using Hardening.Models.Domains;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hardening.API.Controllers
{
  [Route("api/todo")]
  [ApiController]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  public class TodoController : ControllerBase
  {
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly UserManager<ApplicationUser> _userManager;


    public TodoController(IApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager)
    {
      _applicationDbContext = applicationDbContext;
      _userManager = userManager;
    }

    [HttpGet("get_all")]
    public async Task<IActionResult> GetAllAsync([FromQuery] GetAllRequest request)
    {
      var currentUser = await _userManager.FindByNameAsync(User?.Identity?.Name);
      var query = _applicationDbContext.Set<Todo>().Where(s => s.IsActive == request.IsActive && s.CreatedById == currentUser.Id);

      if (!string.IsNullOrWhiteSpace(request.Search))
      {
        query = query.Where(s => s.Detail.ToLower().Contains(request.Search));
      }

      if (request.DueDate.HasValue)
      {
        query = query.Where(s => s.DueDate >= request.DueDate);
      }

      if (request.EndDueDate.HasValue)
      {
        query = query.Where(s => s.DueDate <= request.EndDueDate);
      }

      var response = await query.Select(s => new GetAllResponse
      {
        Detail = s.Detail,
        DueDate = s.DueDate,
        Id = s.Id,
        IsActive = s.IsActive
      }).ToListAsync();

      return response.Any() ? Ok(response) : NoContent();
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateRequest request)
    {
      if (ModelState.IsValid)
      {
        var currentUser = await _userManager.FindByNameAsync(User?.Identity?.Name);

        var newTask = new Todo
        {
          Detail = request.Detail,
          DueDate = request.DueDate,
          CreatedById = currentUser.Id,
          CreatedOn = DateTime.UtcNow,
          ModifiedById = currentUser.Id,
          ModifiedOn = DateTime.UtcNow,
          IsActive = true
        };

        await _applicationDbContext.Set<Todo>().AddAsync(newTask);
        if (await _applicationDbContext.SaveChangeAsync())
        {
          var response = new CreateResponse
          {
            Id = newTask.Id,
            Detail = newTask.Detail,
            DueDate = newTask.DueDate,
            IsActive = newTask.IsActive
          };

          return Ok(response);
        }
      }

      return BadRequest(ModelState);
    }

    [HttpPatch("update/{id}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateRequest request)
    {
      if (ModelState.IsValid)
      {
        var existing = await _applicationDbContext.Set<Todo>().FirstOrDefaultAsync(s => s.Id == id);
        if (existing == null) return NotFound(new Exception("Current content not found."));

        if (request.IsActive.HasValue) existing.IsActive = request.IsActive.Value;
        if (!string.IsNullOrWhiteSpace(request.Detail)) existing.Detail = request.Detail;
        if (request.DueDate.HasValue) existing.DueDate = request.DueDate.Value;

        var currentUser = await _userManager.FindByNameAsync(User?.Identity?.Name);
        existing.ModifiedById = currentUser.Id;
        existing.ModifiedOn = DateTime.UtcNow;

        _applicationDbContext.Set<Todo>().Update(existing);
        if (await _applicationDbContext.SaveChangeAsync())
        {
          var response = new UpdateResponse
          {
            Id = existing.Id,
            Detail = existing.Detail,
            DueDate = existing.DueDate,
            IsActive = existing.IsActive
          };

          return Ok(response);
        }
      }

      return BadRequest(ModelState);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DisableAsync(Guid id)
    {
      if (ModelState.IsValid)
      {
        var existing = await _applicationDbContext.Set<Todo>().FirstOrDefaultAsync(s => s.Id == id);
        if (existing == null) return NotFound(new Exception("Current content not found."));
        existing.IsActive = false;

        _applicationDbContext.Set<Todo>().Update(existing);
        if (await _applicationDbContext.SaveChangeAsync())
        {
          return Ok();
        }
      }

      return BadRequest(ModelState);
    }
  }
}