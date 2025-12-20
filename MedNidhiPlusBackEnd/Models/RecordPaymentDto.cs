namespace MedNidhiPlusBackEnd.Models;

public class RecordPaymentDto
{
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string? PaymentMethod { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
}



