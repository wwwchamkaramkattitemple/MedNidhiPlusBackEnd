using MedNidhiPlusBackEnd.API.Data;
using MedNidhiPlusBackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedNidhiPlusBackEnd.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProcedureController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProcedureController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Procedure
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Procedure>>> GetProcedures()
    {
        return await _context.Procedures.ToListAsync();
    }

    // GET: api/Procedure/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Procedure>> GetProcedure(int id)
    {
        var procedure = await _context.Procedures.FindAsync(id);
        if (procedure == null) return NotFound();
        return procedure;
    }

    // POST: api/Procedure
    [HttpPost]
    public async Task<ActionResult<Procedure>> CreateProcedure(Procedure procedure)
    {
        _context.Procedures.Add(procedure);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetProcedure), new { id = procedure.Id }, procedure);
    }

    // PUT: api/Procedure/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProcedure(int id, Procedure procedure)
    {
        if (id != procedure.Id) return BadRequest();

        procedure.UpdatedAt = DateTime.UtcNow;
        _context.Entry(procedure).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Procedure/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProcedure(int id)
    {
        var procedure = await _context.Procedures.FindAsync(id);
        if (procedure == null) return NotFound();

        _context.Procedures.Remove(procedure);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Procedure>>> Search(string query)
    {
        return await _context.Procedures
            .Where(p => string.IsNullOrEmpty(query)
                     || p.ProcedureName.Contains(query)
                     || p.Description.Contains(query))
            .Take(20)                   
            .ToListAsync();
    }

}