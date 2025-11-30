using System.ComponentModel.DataAnnotations;


namespace MedNidhiPlusBackEnd.Models;

public class Department
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string DepartmentName { get; set; } = string.Empty;

    [MaxLength(250)]
    public string? Description { get; set; }  

    public int DefaultRevisitDays { get; set; } = 15; 

    public decimal? DefaultConsultationFee { get; set; }  

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}


