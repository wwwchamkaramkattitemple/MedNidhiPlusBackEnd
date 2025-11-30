using MedNidhiPlusBackEnd.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedNidhiPlusBackEnd.API.Models;


public class Appointment
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int PatientId { get; set; }
    public Patient? Patient { get; set; }   // ← MAKE NULLABLE

    [Required]
    public int DoctorId { get; set; }
    public Doctor? Doctor { get; set; }     // ← MAKE NULLABLE

    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }

    [Required]
    public DateTime AppointmentDate { get; set; }

    [Required]
    public TimeSpan AppointmentTime { get; set; }

    [Required]
    public int AppointmentTypeId { get; set; }
    public AppointmentType? AppointmentType { get; set; }   // ← MAKE NULLABLE

    [Required]
    public int StatusId { get; set; }
    public AppointmentStatus? Status { get; set; }          // ← MAKE NULLABLE

    public string? Notes { get; set; }

    public decimal? Fee { get; set; }

    public bool IsRevisit { get; set; } = false;
    public bool IsBilled { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
