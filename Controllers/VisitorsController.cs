using CastleGuardian.Api.Data;
using CastleGuardian.Api.DTOs;
using CastleGuardian.Api.Models;
using CastleGuardian.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CastleGuardian.Api.Controllers;

[ApiController]
[Route("api/visitors")]
public class VisitorsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly GuardianService _guardianService;

    public VisitorsController(AppDbContext db, GuardianService guardianService)
    {
        _db = db;
        _guardianService = guardianService;
    }

    [HttpPost]
    public async Task<IActionResult> RegisterVisitor()
    {
        var visitor = new Visitor { TrustLevel = 0 };
        _db.Visitors.Add(visitor);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetVisitor), new { id = visitor.Id }, visitor);
    }

    [HttpGet]
    public async Task<IActionResult> GetVisitors()
    {
        var visitors = await _db.Visitors.Include(v => v.ActionHistory).ToListAsync();
        return Ok(visitors);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetVisitor(int id)
    {
        var visitor = await _db.Visitors
            .Include(v => v.ActionHistory)
            .FirstOrDefaultAsync(v => v.Id == id);

        return visitor is null ? NotFound() : Ok(visitor);
    }

    [HttpPost("{id:int}/actions")]
    public async Task<IActionResult> PerformAction(int id, [FromBody] ActionRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Action))
            return BadRequest("Действие не может быть пустым");

        var (visitor, error, newTrust) = await _guardianService.ExecuteActionAsync(id, request.Action);

        if (visitor is null)
            return NotFound(error);

        if (error is not null)
            return BadRequest(error);

        return Ok(new { visitorId = id, action = request.Action, trustLevel = newTrust });
    }

    [HttpGet("{id:int}/access")]
    public async Task<IActionResult> CheckAccess(int id)
    {
        var visitor = await _db.Visitors.FirstOrDefaultAsync(v => v.Id == id);

        if (visitor is null)
            return NotFound();

        return Ok(new { accessGranted = visitor.TrustLevel >= 50 });
    }
}