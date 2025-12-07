using MedNidhiPlusBackEnd.API.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class Invoice
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int PatientId { get; set; }

    [ForeignKey("PatientId")]
    [JsonIgnore]
    public Patient? Patient { get; set; }

    public string? InvoiceNumber { get; set; }  // Backend will generate

    [Required]
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

    public DateTime? DueDate { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Pending";

    [Column(TypeName = "decimal(18,2)")]
    public decimal SubTotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PaidAmount { get; set; } = 0;

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime? PaymentDate { get; set; }

    [MaxLength(100)]
    public string? PaymentMethod { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    [JsonIgnore]
    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
}