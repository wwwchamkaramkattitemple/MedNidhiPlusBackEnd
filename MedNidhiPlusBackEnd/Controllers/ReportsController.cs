using MedNidhiPlusBackEnd.API.Data;
using MedNidhiPlusBackEnd.API.Models;
using MedNidhiPlusBackEnd.Models;
using MedNidhiPlusBackEnd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using ClosedXML.Excel;

namespace MedNidhiPlusBackEnd.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ReportsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/reports/daily-collection
    [HttpGet("daily-collection")]
    public async Task<ActionResult<List<DailyCollectionReportDto>>> GetDailyCollectionReport([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
    {
        var result = await GetDailyCollectionInternal(fromDate, toDate);
        return Ok(result);
    }

    private async Task<List<DailyCollectionReportDto>> GetDailyCollectionInternal(DateTime fromDate, DateTime toDate)
    {
        var result = await _context.Invoices
               .Where(i =>
                   i.PaymentDate != null &&
                   i.PaymentDate >= fromDate &&
                   i.PaymentDate <= toDate &&
                   i.PaidAmount > 0
               )
               .GroupBy(i => i.PaymentDate!.Value.Date)
               .Select(g => new DailyCollectionReportDto
               {
                   Date = g.Key,
                   TotalCollection = g.Sum(x => x.PaidAmount),
                   CashCollection = g.Where(x => x.PaymentMethod == "Cash").Sum(x => x.PaidAmount),
                   CardCollection = g.Where(x => x.PaymentMethod == "Card").Sum(x => x.PaidAmount),
                   UpiCollection = g.Where(x => x.PaymentMethod == "UPI").Sum(x => x.PaidAmount),
                   OtherCollection = g.Where(x =>
                       x.PaymentMethod != "Cash" &&
                       x.PaymentMethod != "Card" &&
                       x.PaymentMethod != "UPI"
                   ).Sum(x => x.PaidAmount)
               })
               .OrderByDescending(x => x.Date)
               .ToListAsync();

        return result;
    }

    [HttpGet("daily-collection/pdf")]
    public async Task<IActionResult> DailyCollectionPdf([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
    {
        var data = await GetDailyCollectionInternal(fromDate, toDate);
        if (!data.Any())
            return BadRequest("No data available");

        var settings = await _context.SystemSettings.FirstAsync();

        var document = ReportPdfFactory.CreateDailyCollectionReport(data, fromDate, toDate, settings);

        using var stream = new MemoryStream();
        document.GeneratePdf(stream);

        return File(
            stream.ToArray(),
            "application/pdf",
            $"DailyCollection_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.pdf"
        );
    }

    [HttpGet("daily-collection/excel")]
    public async Task<IActionResult> DailyCollectionExcel(
    [FromQuery] DateTime fromDate,
    [FromQuery] DateTime toDate)
    {
        var data = await GetDailyCollectionInternal(fromDate, toDate);

        if (data == null || !data.Any())
            return BadRequest("No data available for selected date range");

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Daily Collection");

        // ---------------- TITLE ----------------
        worksheet.Cell(1, 1).Value = "Daily Collection Report";
        worksheet.Range(1, 1, 1, 6)
            .Merge()
            .Style.Font.SetBold()
            .Font.SetFontSize(14)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        worksheet.Cell(2, 1).Value =
            $"From {fromDate:dd/MM/yyyy} To {toDate:dd/MM/yyyy}";
        worksheet.Range(2, 1, 2, 6)
            .Merge()
            .Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        // ---------------- HEADER ----------------
        int row = 4;

        worksheet.Cell(row, 1).Value = "Date";
        worksheet.Cell(row, 2).Value = "Cash";
        worksheet.Cell(row, 3).Value = "Card";
        worksheet.Cell(row, 4).Value = "UPI";
        worksheet.Cell(row, 5).Value = "Other";
        worksheet.Cell(row, 6).Value = "Total";

        worksheet.Range(row, 1, row, 6)
            .Style
            .Font.SetBold()
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
            .Border.SetOutsideBorder(XLBorderStyleValues.Thin);

        // ---------------- DATA ----------------
        row++;

        foreach (var item in data)
        {
            worksheet.Cell(row, 1).Value = item.Date.ToString("dd/MM/yyyy");
            worksheet.Cell(row, 2).Value = item.CashCollection;
            worksheet.Cell(row, 3).Value = item.CardCollection;
            worksheet.Cell(row, 4).Value = item.UpiCollection;
            worksheet.Cell(row, 5).Value = item.OtherCollection;
            worksheet.Cell(row, 6).Value = item.TotalCollection;

            worksheet.Range(row, 1, row, 6)
                .Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

            row++;
        }

        // ---------------- GRAND TOTAL ----------------
        worksheet.Cell(row, 5).Value = "Grand Total";
        worksheet.Cell(row, 6).FormulaA1 = $"SUM(F5:F{row - 1})";

        worksheet.Range(row, 5, row, 6)
            .Style
            .Font.SetBold()
            .Border.SetTopBorder(XLBorderStyleValues.Thick);

        worksheet.Columns().AdjustToContents();

        // ---------------- SAVE ----------------
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"DailyCollection_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.xlsx"
        );
    }

    //Doctor Revenue Report

    private async Task<List<DoctorRevenueReportDto>> GetDoctorRevenueInternal(DateTime fromDate, DateTime toDate)
    {
        var result = await _context.InvoiceItems
     .Where(ii =>
         ii.AppointmentId != null &&
         ii.Appointment!.DoctorId != null &&
         ii.Invoice != null &&
         ii.Invoice.PaymentDate != null &&
         ii.Invoice.PaymentDate >= fromDate &&
         ii.Invoice.PaymentDate <= toDate &&
         ii.Invoice.Status == "Paid"
     )
     .GroupBy(ii => new
     {
         ii.Appointment!.DoctorId,
         ii.Appointment.Doctor!.DoctorName
     })
     .Select(g => new DoctorRevenueReportDto
     {
         DoctorId = g.Key.DoctorId,
         DoctorName = g.Key.DoctorName,
         InvoiceCount = g.Select(x => x.InvoiceId).Distinct().Count(),
         TotalRevenue = g.Sum(x => x.TotalAmount)
     })
     .OrderByDescending(x => x.TotalRevenue)
     .ToListAsync();
        return result;

    }

    [HttpGet("doctor-revenue")]
    public async Task<ActionResult<List<DoctorRevenueReportDto>>> GetDoctorRevenueReport([FromQuery] DateTime fromDate,[FromQuery] DateTime toDate)
    {
        var data = await GetDoctorRevenueInternal(fromDate, toDate);
        if (!data.Any())
            return BadRequest("No data available");

        var settings = await _context.SystemSettings.FirstAsync();

        var document = ReportPdfFactory.CreateDoctorRevenueReport(data, fromDate, toDate, settings);

        using var stream = new MemoryStream();
        document.GeneratePdf(stream);

        return File(
            stream.ToArray(),
            "application/pdf",
            $"DoctorRevenue_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.pdf"
        );

    }

    [HttpGet("doctor-revenue/pdf")]
    public async Task<IActionResult> DoctorRevenuePdf([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
    {
        var data = await GetDoctorRevenueInternal(fromDate, toDate);
        if (!data.Any())
            return BadRequest("No data available");

        var settings = await _context.SystemSettings.FirstAsync();

        var document = ReportPdfFactory.CreateDoctorRevenueReport(data, fromDate, toDate, settings);

        using var stream = new MemoryStream();
        document.GeneratePdf(stream);

        return File(
            stream.ToArray(),
            "application/pdf",
            $"DoctorRevenue_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.pdf"
        );
    }

    [HttpGet("doctor-revenue/excel")]
    public async Task<IActionResult> ExportDoctorRevenueExcel([FromQuery] DateTime fromDate,[FromQuery] DateTime toDate)
    {
        var data = await GetDoctorRevenueInternal(fromDate, toDate);

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Doctor Revenue");

        int row = 1;

        // Header
        ws.Cell(row, 1).Value = "Doctor Wise Revenue Report";
        ws.Range(row, 1, row, 3).Merge().Style
            .Font.SetBold()
            .Font.SetFontSize(14)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        row++;

        ws.Cell(row, 1).Value = $"From {fromDate:dd/MM/yyyy} To {toDate:dd/MM/yyyy}";
        ws.Range(row, 1, row, 3).Merge().Style
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        row += 2;

        // Table header
        ws.Cell(row, 1).Value = "Doctor";
        ws.Cell(row, 2).Value = "Invoices";
        ws.Cell(row, 3).Value = "Revenue";

        ws.Range(row, 1, row, 3).Style
            .Font.SetBold()
            .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
            .Fill.SetBackgroundColor(XLColor.LightGray);
        row++;

        // Rows
        foreach (var r in data)
        {
            ws.Cell(row, 1).Value = r.DoctorName;
            ws.Cell(row, 2).Value = r.InvoiceCount;
            ws.Cell(row, 3).Value = r.TotalRevenue;

            ws.Range(row, 1, row, 3).Style
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin);

            ws.Cell(row, 3).Style.NumberFormat.Format = "₹ #,##0.00";
            row++;
        }

        // Grand total
        ws.Cell(row, 2).Value = "Grand Total";
        ws.Cell(row, 3).Value = data.Sum(x => x.TotalRevenue);

        ws.Range(row, 2, row, 3).Style
            .Font.SetBold()
            .Border.SetTopBorder(XLBorderStyleValues.Thin);

        ws.Cell(row, 3).Style.NumberFormat.Format = "₹ #,##0.00";

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Doctor_Revenue_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.xlsx"
        );
    }





}
