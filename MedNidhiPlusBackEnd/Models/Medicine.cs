using System.ComponentModel.DataAnnotations;

namespace MedNidhiPlusBackEnd.Models;

public class Medicine
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string MedicineName { get; set; } = string.Empty;

    public string? GenericName { get; set; }

    public int? CategoryId { get; set; }
    public MedicineCategory? Category { get; set; }

    [Required]
    public decimal UnitPrice { get; set; }

    public decimal TaxRate { get; set; } = 0;

    public int StockQuantity { get; set; } = 0;
    public int ReorderLevel { get; set; } = 10;
    public decimal Discount { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

