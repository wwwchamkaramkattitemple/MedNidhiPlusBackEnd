using MedNidhiPlusBackEnd.API.Data;
using MedNidhiPlusBackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedNidhiPlusBackEnd.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MedicineCategoryController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public MedicineCategoryController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MedicineCategory>>> Get()
    {
        return await _context.MedicineCategories.OrderBy(c => c.CategoryName).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MedicineCategory>> Get(int id)
    {
        var category = await _context.MedicineCategories.FindAsync(id);
        if (category == null)
            return NotFound();

        return category;
    }

    [HttpPost]
    public async Task<ActionResult<MedicineCategory>> Create(MedicineCategory category)
    {
        category.CreatedAt = DateTime.UtcNow;
        _context.MedicineCategories.Add(category);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = category.Id }, category);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, MedicineCategory updated)
    {
        if (id != updated.Id) return BadRequest();

        var category = await _context.MedicineCategories.FindAsync(id);
        if (category == null) return NotFound();

        category.CategoryName = updated.CategoryName;
        category.Description = updated.Description;
        category.IsActive = updated.IsActive;
        category.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.MedicineCategories.FindAsync(id);
        if (category == null) return NotFound();

        _context.MedicineCategories.Remove(category);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}