namespace MedNidhiPlusBackEnd.Models;

public class InvoiceUpdateDto
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Status { get; set; }
    public decimal PaidAmount { get; set; }
    public string? Notes { get; set; }
    public string? PaymentMethod { get; set; }

    public List<InvoiceItemDto>? Items { get; set; }
}

