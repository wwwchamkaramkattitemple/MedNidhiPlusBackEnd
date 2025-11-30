using MedNidhiPlusBackEnd.API.Data;
using MedNidhiPlusBackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace MedNidhiPlusBackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SystemSettingsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public SystemSettingsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // ✅ GET: api/SystemSettings
    [HttpGet]
    public async Task<ActionResult<SystemSetting>> GetSystemSettings()
    {
        var setting = await _context.SystemSettings.FirstOrDefaultAsync();

        if (setting == null)
        {
            // Optional: create default setting if not found
            setting = new SystemSetting
            {
                ClinicName = "Clinic Billing System",
                FeePriority = "Default",
                DefaultFee = 200,
                DefaultRevisitDays = 15
            };

            _context.SystemSettings.Add(setting);
            await _context.SaveChangesAsync();
        }

        return Ok(setting);
    }

    // ✅ POST: api/SystemSettings
    [HttpPost]
    public async Task<IActionResult> SaveSystemSettings(SystemSetting settings)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existing = await _context.SystemSettings.FirstOrDefaultAsync();

        if (existing == null)
        {
            // Create new
            _context.SystemSettings.Add(settings);
        }
        else
        {
            // Update existing
            existing.ClinicName = settings.ClinicName;
            existing.FeePriority = settings.FeePriority;
            existing.DefaultFee = settings.DefaultFee;
            existing.DefaultRevisitDays = settings.DefaultRevisitDays;
        }

        await _context.SaveChangesAsync();

        return Ok(new { message = "Settings saved successfully" });
    }
}
