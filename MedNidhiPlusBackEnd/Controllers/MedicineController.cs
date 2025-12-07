using MedNidhiPlusBackEnd.API.Data;
using MedNidhiPlusBackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedNidhiPlusBackEnd.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MedicineController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public MedicineController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetMedicines()
    {
        var medicines = await _context.Medicines
            .Include(m => m.Category)
            .Select(m => new
            {
                m.Id,
                m.MedicineName,
                m.GenericName,
                CategoryId = m.CategoryId,
                CategoryName = m.Category != null ? m.Category.CategoryName : "",
                m.UnitPrice,
                m.StockQuantity,
                m.ReorderLevel,
                m.Discount,
                m.TaxRate,
                m.IsActive
            })
            .ToListAsync();

        return Ok(medicines);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<Medicine>> GetMedicine(int id)
    {
        var med = await _context.Medicines.FindAsync(id);
        if (med == null) return NotFound();
        return med;
    }

    [HttpPost]
    public async Task<ActionResult<Medicine>> CreateMedicine(Medicine med)
    {
        _context.Medicines.Add(med);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetMedicine), new { id = med.Id }, med);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMedicine(int id, Medicine med)
    {
        if (id != med.Id) return BadRequest();

        med.UpdatedAt = DateTime.UtcNow;
        _context.Entry(med).State = EntityState.Modified;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMedicine(int id)
    {
        var med = await _context.Medicines.FindAsync(id);
        if (med == null) return NotFound();

        _context.Medicines.Remove(med);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
