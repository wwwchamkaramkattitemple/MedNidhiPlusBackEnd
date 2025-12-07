using MedNidhiPlusBackEnd.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedNidhiPlusBackEnd.API.Models;

public class InvoiceItem
{
    [Key]
    public int Id { get; set; } // EF will auto-generate

    public int InvoiceId { get; set; }

    [JsonIgnore]
    public Invoice? Invoice { get; set; }

    public string Description { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal Discount { get; set; }

    public decimal TaxRate { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public int? AppointmentId { get; set; }

    [JsonIgnore]
    public Appointment? Appointment { get; set; }
}





