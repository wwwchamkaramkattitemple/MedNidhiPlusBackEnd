using MedNidhiPlusBackEnd.API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MedNidhiPlusBackEnd.Models;

public class Doctor
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string DoctorName { get; set; } = string.Empty;

    [Required]
    public int DepartmentId { get; set; }

    [ForeignKey(nameof(DepartmentId))]
    [JsonIgnore] 
    public Department? Department { get; set; } 
    [MaxLength(100)]
    public string? Qualification { get; set; }

    [MaxLength(100)]
    public string? Specialization { get; set; }

    public decimal ConsultationFee { get; set; }

    public int? RevisitDays { get; set; }

    [MaxLength(20)]
    public string? MobileNumber { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Appointment>? Appointments { get; set; }
}

