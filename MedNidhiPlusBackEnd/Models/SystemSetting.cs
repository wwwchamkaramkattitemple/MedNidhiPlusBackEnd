namespace MedNidhiPlusBackEnd.Models;

public class SystemSetting
{
    public int Id { get; set; }
    public decimal DefaultFee { get; set; }
    public int DefaultRevisitDays { get; set; }
    public string FeePriority { get; set; } = "Default"; // e.g., "Doctor>Department>Default"
    public string ClinicName { get; set;} = "Clinic Billing System";
}
