using MedNidhiPlusBackEnd.API.Data;
using MedNidhiPlusBackEnd.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedNidhiPlusBackEnd.API.Models;

namespace MedNidhiPlusBackEnd.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class InvoiceController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public InvoiceController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Invoice
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoices()
    {
        return await _context.Invoices
            .Include(i => i.Patient)
            .Include(i => i.Items)
            .ToListAsync();
    }

    // GET: api/Invoice/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Invoice>> GetInvoice(int id)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Patient)
            .Include(i => i.Items)
            .ThenInclude(item => item.Appointment)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null)
        {
            return NotFound();
        }

        return invoice;
    }

    // GET: api/Invoice/patient/5
    [HttpGet("patient/{patientId}")]
    public async Task<ActionResult<IEnumerable<Invoice>>> GetPatientInvoices(int patientId)
    {
        return await _context.Invoices
            .Where(i => i.PatientId == patientId)
            .Include(i => i.Patient)
            .Include(i => i.Items)
            .ToListAsync();
    }

    // GET: api/Invoice/status/Pending
    [HttpGet("status/{status}")]
    public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoicesByStatus(string status)
    {
        return await _context.Invoices
            .Where(i => i.Status == status)
            .Include(i => i.Patient)
            .Include(i => i.Items)
            .ToListAsync();
    }

    // POST: api/Invoice
    [HttpPost]
    public async Task<ActionResult<Invoice>> CreateInvoice(Invoice invoice)
    {
        // Check if patient exists
        var patient = await _context.Patients.FindAsync(invoice.PatientId);
        if (patient == null)
        {
            return BadRequest("Patient not found");
        }

        // Generate invoice number
        invoice.InvoiceNumber = GenerateInvoiceNumber();

        // Calculate totals
        CalculateInvoiceTotals(invoice);

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        // If invoice has items with appointments, mark those appointments as billed
        foreach (var item in invoice.Items.Where(i => i.AppointmentId.HasValue))
        {
            var appointment = await _context.Appointments.FindAsync(item.AppointmentId.Value);
            if (appointment != null)
            {
                appointment.IsBilled = true;
                appointment.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetInvoice), new { id = invoice.Id }, invoice);
    }

    // PUT: api/Invoice/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateInvoice(int id, Invoice invoice)
    {
        if (id != invoice.Id)
        {
            return BadRequest();
        }

        // Recalculate totals
        CalculateInvoiceTotals(invoice);

        invoice.UpdatedAt = DateTime.UtcNow;
        _context.Entry(invoice).State = EntityState.Modified;

        // Handle invoice items
        var existingItems = await _context.InvoiceItems.Where(i => i.InvoiceId == id).ToListAsync();
        var itemsToRemove = existingItems.Where(existingItem => 
            !invoice.Items.Any(i => i.Id == existingItem.Id)).ToList();

        foreach (var item in itemsToRemove)
        {
            _context.InvoiceItems.Remove(item);
        }

        foreach (var item in invoice.Items)
        {
            if (item.Id == 0)
            {
                // New item
                item.InvoiceId = id;
                _context.InvoiceItems.Add(item);
            }
            else
            {
                // Existing item
                _context.Entry(item).State = EntityState.Modified;
            }
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!InvoiceExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // PUT: api/Invoice/5/status
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateInvoiceStatus(int id, [FromBody] string status)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice == null)
        {
            return NotFound();
        }

        // Validate status
        if (!new[] { "Pending", "Paid", "Overdue", "Cancelled" }.Contains(status))
        {
            return BadRequest("Invalid status value");
        }

        invoice.Status = status;
        invoice.UpdatedAt = DateTime.UtcNow;

        // If status is Paid, update payment date
        if (status == "Paid" && invoice.PaymentDate == null)
        {
            invoice.PaymentDate = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // PUT: api/Invoice/5/payment
    [HttpPut("{id}/payment")]
    public async Task<IActionResult> RecordPayment(int id, [FromBody] PaymentDto payment)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice == null)
        {
            return NotFound();
        }

        invoice.PaidAmount += payment.Amount;
        invoice.PaymentMethod = payment.PaymentMethod;
        invoice.PaymentDate = DateTime.UtcNow;
        invoice.UpdatedAt = DateTime.UtcNow;

        // Update status if fully paid
        if (invoice.PaidAmount >= invoice.TotalAmount)
        {
            invoice.Status = "Paid";
        }
        else if (invoice.PaidAmount > 0)
        {
            invoice.Status = "Partial";
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/Invoice/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteInvoice(int id)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null)
        {
            return NotFound();
        }

        // Remove all invoice items first
        _context.InvoiceItems.RemoveRange(invoice.Items);
        
        // Then remove the invoice
        _context.Invoices.Remove(invoice);
        
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool InvoiceExists(int id)
    {
        return _context.Invoices.Any(e => e.Id == id);
    }

    private string GenerateInvoiceNumber()
    {
        // Format: INV-YYYYMMDD-XXXX where XXXX is a sequential number
        string datePrefix = DateTime.UtcNow.ToString("yyyyMMdd");
        
        // Get the last invoice number with this date prefix
        var lastInvoice = _context.Invoices
            .Where(i => i.InvoiceNumber.StartsWith($"INV-{datePrefix}"))
            .OrderByDescending(i => i.InvoiceNumber)
            .FirstOrDefault();

        int sequence = 1;
        if (lastInvoice != null)
        {
            // Extract the sequence number from the last invoice number
            string[] parts = lastInvoice.InvoiceNumber.Split('-');
            if (parts.Length == 3 && int.TryParse(parts[2], out int lastSequence))
            {
                sequence = lastSequence + 1;
            }
        }

        return $"INV-{datePrefix}-{sequence:D4}";
    }

    private void CalculateInvoiceTotals(Invoice invoice)
    {
        decimal subtotal = 0;
        decimal taxAmount = 0;

        foreach (var item in invoice.Items)
        {
            // Calculate item total
            item.TotalAmount = (item.UnitPrice * item.Quantity) - item.Discount;
            
            // Calculate tax if applicable
            if (item.TaxRate > 0)
            {
                item.TaxAmount = item.TotalAmount * (item.TaxRate / 100);
                item.TotalAmount += item.TaxAmount;
            }

            subtotal += (item.UnitPrice * item.Quantity) - item.Discount;
            taxAmount += item.TaxAmount;
        }

        invoice.SubTotal = subtotal;
        invoice.TaxAmount = taxAmount;
        invoice.TotalAmount = subtotal + taxAmount;
    }
}

public class PaymentDto
{
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
}