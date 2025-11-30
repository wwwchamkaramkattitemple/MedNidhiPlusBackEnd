using MedNidhiPlusBackEnd.API.Data;
using MedNidhiPlusBackEnd.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedNidhiPlusBackEnd.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PatientController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PatientController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Patient
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Patient>>> GetPatients()
    {
        return await _context.Patients.ToListAsync();
    }

    // GET: api/Patient/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Patient>> GetPatient(int id)
    {
        var patient = await _context.Patients.FindAsync(id);

        if (patient == null)
        {
            return NotFound();
        }

        return patient;
    }

    // GET: api/Patient/search?query=john
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Patient>>> SearchPatients(string query)
    {
        if (string.IsNullOrEmpty(query))
            return await _context.Patients.Take(20).ToListAsync();

        return await _context.Patients
            .Where(p => p.FirstName.Contains(query) ||
                       p.LastName.Contains(query) ||
                       p.PhoneNumber!.Contains(query) ||
                       p.Email!.Contains(query))
            .ToListAsync();
    }

    // POST: api/Patient
    [HttpPost]
    public async Task<ActionResult<Patient>> CreatePatient(Patient patient)
    {
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePatient(int id, Patient updatedPatient)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null)
            return NotFound();

        patient.FirstName = updatedPatient.FirstName;
        patient.LastName = updatedPatient.LastName;
        patient.PhoneNumber = updatedPatient.PhoneNumber;
        patient.Email = updatedPatient.Email;
        patient.DateOfBirth = updatedPatient.DateOfBirth;
        patient.Address = updatedPatient.Address;
        patient.City = updatedPatient.City;
        patient.State = updatedPatient.State;
        patient.ZipCode = updatedPatient.ZipCode;
        patient.MedicalHistory = updatedPatient.MedicalHistory;
        patient.InsuranceProvider = updatedPatient.InsuranceProvider;
        patient.InsurancePolicyNumber = updatedPatient.InsurancePolicyNumber;
        patient.UpdatedAt = DateTime.UtcNow;

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


    // DELETE: api/Patient/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePatient(int id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null)
        {
            return NotFound();
        }

        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();

        return NoContent();
    }

}