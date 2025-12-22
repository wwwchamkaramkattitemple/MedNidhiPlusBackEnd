using System.ComponentModel.DataAnnotations;

namespace MedNidhiPlusBackEnd.Models;

public class MedicineCategory
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string CategoryName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
