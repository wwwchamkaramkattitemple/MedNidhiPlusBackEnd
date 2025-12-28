namespace MedNidhiPlusBackEnd.Models;

public class SystemSetting
{
    public int Id { get; set; }

    // ───────── Clinic Identity ─────────
    public string ClinicName { get; set; } = "Clinic Billing System";
    public string ClinicAddress { get; set; } = "";
    public string ClinicPhone { get; set; } = "";
    public string ClinicEmail { get; set; } = "";
    public string ClinicGstNumber { get; set; } = "";
    public string SpecialityName { get; set; } = "Dental & Dermatology";

    // ───────── Billing Defaults ─────────
    public decimal DefaultFee { get; set; } = 200;
    public int DefaultRevisitDays { get; set; } = 15;
    public string FeePriority { get; set; } = "Default";

    // ───────── Invoice Print Defaults ─────────
    public string DefaultInvoicePrintMode { get; set; } = "Normal";
    // Normal | Thermal

    public string DefaultInvoiceDesign { get; set; } = "Classic";
    // Classic | Modern | Minimal

    public string DefaultReportDesign { get; set; } = "Classic";
    // Classic | Modern | Minimal

    // ───────── PDF Messages ─────────
    public string PdfHeaderMessage { get; set; } = "";
    public string PdfFooterMessage { get; set; } = "";
    public string PdfThankYouMessage { get; set; } = "Thank you for choosing us";
}
