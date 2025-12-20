using MedNidhiPlusBackEnd.API.Controllers;
using MedNidhiPlusBackEnd.API.Models;

namespace MedNidhiPlusBackEnd.Models;

public class InvoiceDetailDto
{
    public int Id { get; set; }
    public string? InvoiceNumber { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime? DueDate { get; set; }

    public string? Status { get; set; }

    // Patient
    public PatientDto? Patient { get; set; }

    // Items
    public List<InvoiceItemDto>? Items { get; set; }

    // Amounts
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }

    // Payments
    public decimal PaidAmount { get; set; }
    public decimal BalanceAmount { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string? PaymentMethod { get; set; }

    public bool IsLocked { get; set; } = false;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}



