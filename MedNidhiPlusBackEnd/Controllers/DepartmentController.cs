using MedNidhiPlusBackEnd.API.Data;
using MedNidhiPlusBackEnd.API.Models;
using MedNidhiPlusBackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedNidhiPlusBackEnd.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DepartmentController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DepartmentController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Department
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
    {
        return await _context.Departments.ToListAsync();
    }

    // GET: api/Department/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Department>> GetDepartment(int id)
    {
        var department = await _context.Departments.FindAsync(id);

        if (department == null)
        {
            return NotFound();
        }

        return department;
    }

    // GET: api/Department/search?query=cardiology
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Department>>> SearchDepartments(string query)
    {
        if (string.IsNullOrEmpty(query))
            return await _context.Departments.Take(20).ToListAsync();

        return await _context.Departments
            .Where(p => p.DepartmentName.Contains(query))
            .ToListAsync();
    }

    // POST: api/Department
    [HttpPost]
    public async Task<ActionResult<Patient>> CreateDepartment(Department department)
    {
        _context.Departments.Add(department);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetDepartment), new { id = department.Id }, department);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDepartment(int id, Department updatedDepartment)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null)
            return NotFound();

        department.DepartmentName = updatedDepartment.DepartmentName;
        department.Description = updatedDepartment.Description;
        department.DefaultRevisitDays = updatedDepartment.DefaultRevisitDays;
        department.DefaultConsultationFee = updatedDepartment.DefaultConsultationFee;
        department.IsActive = updatedDepartment.IsActive;
        department.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw;
        }
        return NoContent();
    }


    // DELETE: api/Department/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null)
        {
            return NotFound();
        }

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();

        return NoContent();
    }

}