using MedNidhiPlusBackEnd.API.Models;
using MedNidhiPlusBackEnd.Models;
using QuestPDF.Infrastructure;

namespace MedNidhiPlusBackEnd.Services;

public static class ReportPdfFactory
{
    // ---------------- DAILY COLLECTION ----------------
    public static IDocument CreateDailyCollectionReport(
        List<DailyCollectionReportDto> data,
        DateTime fromDate,
        DateTime toDate,
        SystemSetting settings)
    {
        return settings.DefaultReportDesign switch
        {
            "Classic" => new DailyCollectionClassicReportDocument(data, fromDate, toDate, settings),
            "Modern" => new DailyCollectionModernReportDocument(data, fromDate, toDate, settings),
            "Minimal" => new DailyCollectionMinimalReportDocument(data, fromDate, toDate, settings),
            _ => throw new ArgumentException("Invalid report design")
        };
    }

    // ---------------- DOCTOR REVENUE ----------------
    public static IDocument CreateDoctorRevenueReport(
        List<DoctorRevenueReportDto> data,
        DateTime fromDate,
        DateTime toDate,
        SystemSetting settings)
    {
        return settings.DefaultReportDesign switch
        {
           // "Classic" => new DoctorRevenueClassicReportDocument(data, fromDate, toDate, settings),
           // "Modern" => new DoctorRevenueModernReportDocument(data, fromDate, toDate, settings),
           // "Minimal" => new DoctorRevenueMinimalReportDocument(data, fromDate, toDate, settings),
            _ => throw new ArgumentException("Invalid report design")
        };
    }
}

