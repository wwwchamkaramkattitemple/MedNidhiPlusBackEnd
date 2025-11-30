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
public class DoctorController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DoctorController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/doctor
    [HttpGet]
    public async Task<ActionResult> GetDoctors()
    {
        var doctors = await _context.Doctors
            .Include(d => d.Department)
            .Select(d => new
            {
                d.Id,
                d.DoctorName,
                DepartmentName = d.Department.DepartmentName,
                d.Qualification,
                d.ConsultationFee,
                d.RevisitDays,
                d.Specialization,
                d.IsActive,
                DepartmentDefaultConsultationFee = d.Department.DefaultConsultationFee,
                DepartmentDefaultRevisitDays = d.Department.DefaultRevisitDays
            })
            .ToListAsync();

        return Ok(doctors);
    }

    //// GET: api/doctor/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Doctor>> GetDoctor(int id)
    {
        var doctor = await _context.Doctors
            .Include(d => d.Department)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (doctor == null)
            return NotFound();

        return doctor;
    }

    // GET: api/Doctor/search?query=Sudan
    [HttpGet("search")]
    public async Task<ActionResult> SearchDoctors(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            var allDoctors = await _context.Doctors
                .Include(d => d.Department)
                .Select(d => new
                {
                    d.Id,
                    d.DoctorName,
                    DepartmentName = d.Department.DepartmentName,
                    d.Qualification,
                    d.ConsultationFee,
                    d.RevisitDays,
                    d.IsActive
                })
                .Take(20) 
                .ToListAsync();

            return Ok(allDoctors);
        }

        var doctors = await _context.Doctors
            .Include(d => d.Department)
            .Where(d =>
                d.DoctorName.Contains(query) ||
                d.Qualification.Contains(query) ||
                d.Specialization.Contains(query) ||
                d.Department.DepartmentName.Contains(query)||
                d.ConsultationFee.ToString().Contains(query)
            )
            .Select(d => new
            {
                d.Id,
                d.DoctorName,
                DepartmentName = d.Department.DepartmentName,
                d.Qualification,
                d.ConsultationFee,
                d.RevisitDays,
                d.IsActive
            })
            .ToListAsync();

        return Ok(doctors);
    }




    [HttpPost]
    public async Task<IActionResult> CreateDoctor(Doctor doctor)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync();
        return Ok(doctor);
    }

    // PUT: api/doctor/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDoctor(int id,Doctor updatedDoctor)
    {
        var doctor = await _context.Doctors.FindAsync(id);
        if (doctor == null)
            return NotFound();

        doctor.DoctorName = updatedDoctor.DoctorName;
        doctor.DepartmentId = updatedDoctor.DepartmentId;
        doctor.Qualification = updatedDoctor.Qualification;
        doctor.Specialization = updatedDoctor.Specialization;
        doctor.ConsultationFee = updatedDoctor.ConsultationFee;
        doctor.RevisitDays = updatedDoctor.RevisitDays;
        doctor.MobileNumber = updatedDoctor.MobileNumber;
        doctor.Email = updatedDoctor.Email;
        doctor.IsActive = updatedDoctor.IsActive;
        doctor.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/doctor/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteDoctor(int id)
    {
        var doctor = await _context.Doctors.FindAsync(id);
        if (doctor == null)
            return NotFound();

        _context.Doctors.Remove(doctor);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
