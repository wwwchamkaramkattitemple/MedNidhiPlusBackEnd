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


    [HttpGet]
    public async Task<ActionResult<SystemSetting>> GetSystemSettings()
    {
        var setting = await _context.SystemSettings.FirstOrDefaultAsync();

        if (setting == null)
        {
            setting = new SystemSetting(); 
            _context.SystemSettings.Add(setting);
            await _context.SaveChangesAsync();
        }

        return Ok(setting);
    }

    [HttpPost]
    public async Task<IActionResult> SaveSystemSettings(SystemSetting settings)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existing = await _context.SystemSettings.FirstOrDefaultAsync();

        if (existing == null)
        {
            _context.SystemSettings.Add(settings);
        }
        else
        {
            settings.Id = existing.Id;
            _context.Entry(existing).CurrentValues.SetValues(settings);
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "System settings saved successfully" });
    }

}
