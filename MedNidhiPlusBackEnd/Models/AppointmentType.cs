using System.ComponentModel.DataAnnotations;

namespace MedNidhiPlusBackEnd.Models;

public class AppointmentType
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string TypeName { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
