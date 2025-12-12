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
public class AppointmentController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AppointmentController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("types")]
    public async Task<ActionResult<IEnumerable<AppointmentType>>> GetAppointmentTypes()
    {
        var types = await _context.AppointmentTypes
            .Where(t => t.IsActive)
            .OrderBy(t => t.TypeName)
            .ToListAsync();

        return Ok(types);
    }

    [HttpGet("statuses")]
    public async Task<ActionResult<IEnumerable<AppointmentStatus>>> GetAppointmentStatuses()
    {
        var statuses = await _context.AppointmentStatuses
            .Where(s => s.IsActive)
            .OrderBy(s => s.StatusName)
            .ToListAsync();

        return Ok(statuses);
    }

    // ✅ GET: api/Appointment
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAppointments()
    {
        var appointments = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Include(a => a.Department)
            .Include(a => a.Status)
            .Include(a => a.AppointmentType)
            .Select(a => new
            {
                a.Id,
                PatientName = a.Patient.FirstName + " " + a.Patient.LastName,
                DoctorName = a.Doctor.DoctorName,
                DepartmentName = a.Department != null ? a.Department.DepartmentName : null,
                a.AppointmentDate,
                a.AppointmentTime,
                StatusId = a.StatusId,
                StatusName = a.Status.StatusName,
                AppointmentTypeId = a.AppointmentTypeId,
                AppointmentTypeName = a.AppointmentType.TypeName,
                a.Fee,
                a.IsRevisit,
                a.IsBilled,
                a.Notes,
                a.CreatedAt,
                a.UpdatedAt
            })
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();

        return Ok(appointments);
    }


    // ✅ GET: api/Appointment/5
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetAppointment(int id)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Include(a => a.Department)
            .Where(a => a.Id == id)
            .Select(a => new
            {
                a.Id,
                a.PatientId,
                a.DoctorId,
                a.DepartmentId,
                a.AppointmentDate,
                a.AppointmentTime,
                a.Status,
                a.AppointmentType,
                a.Notes,
                a.Fee,
                a.IsRevisit,
                a.IsBilled,
                a.CreatedAt,
                a.UpdatedAt,
                PatientName = a.Patient.FirstName + " " + a.Patient.LastName,
                DoctorName = a.Doctor.DoctorName,
                DepartmentName = a.Department != null ? a.Department.DepartmentName : null
            })
            .FirstOrDefaultAsync();

        if (appointment == null)
            return NotFound();

        return Ok(appointment);
    }

    // ✅ GET: api/Appointment/search?query=ram
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<object>>> SearchAppointments(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return await GetAppointments();
        }

        var results = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Include(a => a.Department)
            .Where(a =>
                a.Patient.FirstName.Contains(query) ||
                a.Patient.LastName.Contains(query) ||
                a.Doctor.DoctorName.Contains(query))
            .Select(a => new
            {
                a.Id,
                PatientName = a.Patient.FirstName + " " + a.Patient.LastName,
                DoctorName = a.Doctor.DoctorName,
                DepartmentName = a.Department != null ? a.Department.DepartmentName : null,
                a.AppointmentDate,
                a.AppointmentTime,
                a.Status,
                a.Fee,
                a.IsRevisit,
                a.IsBilled
            })
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();

        return Ok(results);
    }

    // ✅ GET: api/Appointment/by-patient/5
    [HttpGet("patient/{patientId}")]
    public async Task<ActionResult<IEnumerable<object>>> GetAppointmentsByPatientId(int patientId)
    {
        var appointments = await _context.Appointments
            .Include(a => a.Doctor)
            .Include(a => a.AppointmentType)
            .Include(a => a.Status)
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.AppointmentDate)
            //.Select(a => new
            //{
            //    a.Id,
            //    a.PatientId,

            //    AppointmentDate = a.AppointmentDate,
            //    AppointmentTime = a.AppointmentTime.ToString(@"hh\:mm"),
            //    DateString = a.AppointmentDate.ToString("dd/MM/yyyy"),

            //    DoctorName = a.Doctor != null ? a.Doctor.DoctorName : null,

            //    AppointmentTypeId = a.AppointmentTypeId,
            //    AppointmentType = a.AppointmentType != null ? a.AppointmentType.TypeName : null,

            //    Fee = a.Fee ?? 0,
            //    TaxRate = 0,     // <—— Appointment does NOT have tax, so return 0

            //    StatusId = a.StatusId,
            //    StatusName = a.Status != null ? a.Status.StatusName : null,

            //    a.IsRevisit,
            //    a.IsBilled
            //})
            .Select(a => new
            {
                a.Id,
                a.PatientId,

                AppointmentDate = a.AppointmentDate,
                AppointmentTime = a.AppointmentTime.ToString(@"hh\:mm"),
                DateString = a.AppointmentDate.ToString("dd/MM/yyyy"),

                DoctorName = a.Doctor != null ? a.Doctor.DoctorName : null,

                AppointmentTypeId = a.AppointmentTypeId,
                AppointmentType = a.AppointmentType != null ? a.AppointmentType.TypeName : null,

                Fee = a.Fee ?? 0,
                TaxRate = 0,

                StatusId = a.StatusId,
                StatusName = a.Status != null ? a.Status.StatusName : null,

                a.IsRevisit,
                a.IsBilled,

                InvoiceId = _context.InvoiceItems
                .Where(ii => ii.AppointmentId == a.Id)
                .Select(ii => ii.InvoiceId)
                .FirstOrDefault()
            })
            .ToListAsync();

        return Ok(appointments);
    }

   
    [HttpPost]
    public async Task<ActionResult<Appointment>> CreateAppointment(Appointment appointment)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Validate Patient
        var patientExists = await _context.Patients.AnyAsync(p => p.Id == appointment.PatientId);
        if (!patientExists)
            return BadRequest("Patient not found");

        // Validate Doctor and fetch department
        var doctor = await _context.Doctors
            .Where(d => d.Id == appointment.DoctorId)
            .Select(d => new { d.Id, d.DepartmentId })
            .FirstOrDefaultAsync();

        if (doctor == null)
            return BadRequest("Doctor not found");

        // Auto-assign DepartmentId based on selected Doctor
        appointment.DepartmentId = doctor.DepartmentId;

        // Get Cancelled StatusId (safer than hardcoding)
        var cancelledStatusId = await _context.AppointmentStatuses
            .Where(x => x.StatusName == "Cancelled")
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

        // Prevent multiple active appointments at the same time for same doctor
        var conflict = await _context.Appointments.AnyAsync(a =>
            a.AppointmentDate.Date == appointment.AppointmentDate.Date &&
            a.AppointmentTime == appointment.AppointmentTime &&
            a.DoctorId == appointment.DoctorId &&
            a.StatusId != cancelledStatusId
        );

        if (conflict)
            return BadRequest("Doctor already has an appointment scheduled at this time.");

        appointment.CreatedAt = DateTime.UtcNow;
        appointment.UpdatedAt = DateTime.UtcNow;

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAppointment(int id, Appointment updated)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
            return NotFound();

        appointment.PatientId = updated.PatientId;
        appointment.DoctorId = updated.DoctorId;
        appointment.DepartmentId = updated.DepartmentId;
        appointment.AppointmentDate = updated.AppointmentDate;
        appointment.AppointmentTime = updated.AppointmentTime;
        appointment.AppointmentTypeId = updated.AppointmentTypeId;
        appointment.StatusId = updated.StatusId;
        appointment.Notes = updated.Notes;
        appointment.Fee = updated.Fee;
        appointment.IsRevisit = updated.IsRevisit;
        appointment.IsBilled = updated.IsBilled;
        appointment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAppointment(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
            return NotFound();

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();

        return NoContent();
    }


    private bool AppointmentExists(int id)
    {
        return _context.Appointments.Any(e => e.Id == id);
    }
}
