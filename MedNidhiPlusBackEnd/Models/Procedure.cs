using System.ComponentModel.DataAnnotations;

namespace MedNidhiPlusBackEnd.Models;

public class Procedure
{
    public int Id { get; set; }
    [Required]
    public string ProcedureName { get; set; } = string.Empty;
    public string? Description { get; set; }
    [Required]
    public decimal Fee { get; set; }
    public decimal TaxRate { get; set; } = 0;   
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
