namespace MedNidhiPlusBackEnd.Models;

public class InvoiceItemDto
{
    public int Id { get; set; } // 0 = new item
    public string Description { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxRate { get; set; }
    public int? AppointmentId { get; set; }
}

