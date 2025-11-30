using System.ComponentModel.DataAnnotations;

namespace MedNidhiPlusBackEnd.Models;

public class AppointmentStatus
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string StatusName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public int? OrderNo { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
