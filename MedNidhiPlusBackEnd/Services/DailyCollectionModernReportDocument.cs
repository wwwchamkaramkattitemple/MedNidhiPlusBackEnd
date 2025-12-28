using MedNidhiPlusBackEnd.API.Models;
using MedNidhiPlusBackEnd.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MedNidhiPlusBackEnd.Services;

public class DailyCollectionModernReportDocument : IDocument
{
    private readonly List<DailyCollectionReportDto> _data;
    private readonly DateTime _fromDate;
    private readonly DateTime _toDate;
    private readonly SystemSetting _settings;

    public DailyCollectionModernReportDocument(
        List<DailyCollectionReportDto> data,
        DateTime fromDate,
        DateTime toDate,
        SystemSetting settings)
    {
        _data = data;
        _fromDate = fromDate;
        _toDate = toDate;
        _settings = settings;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(30);
            page.DefaultTextStyle(x => x.FontSize(11));

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeTable);
            page.Footer().Element(ComposeFooter);
        });
    }

    // -------- HEADER --------
    void ComposeHeader(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Text(_settings.ClinicName)
                .FontSize(18)
                .Bold()
                .FontColor(Colors.Blue.Medium);

            col.Item().Text("Daily Collection Report").Bold();
            col.Item().Text(
                $"From {_fromDate.ToLocalTime():dd/MM/yyyy} to {_toDate.ToLocalTime():dd/MM/yyyy}"
            );

            col.Item().PaddingVertical(8).LineHorizontal(1);
        });
    }

    // -------- TABLE --------
    void ComposeTable(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(c =>
            {
                c.RelativeColumn();
                c.RelativeColumn();
                c.RelativeColumn();
                c.RelativeColumn();
                c.RelativeColumn();
                c.RelativeColumn();
            });

            table.Header(h =>
            {
                Header(h.Cell(), "Date");
                Header(h.Cell(), "Cash");
                Header(h.Cell(), "Card");
                Header(h.Cell(), "UPI");
                Header(h.Cell(), "Other");
                Header(h.Cell(), "Total");
            });

            foreach (var row in _data)
            {
                Cell(table.Cell(), row.Date.ToString("dd/MM/yyyy"));
                Cell(table.Cell(), row.CashCollection);
                Cell(table.Cell(), row.CardCollection);
                Cell(table.Cell(), row.UpiCollection);
                Cell(table.Cell(), row.OtherCollection);
                Cell(table.Cell(), row.TotalCollection, true);
            }
        });
    }

    static void Header(IContainer c, string text) =>
        c.Padding(6).Background(Colors.Grey.Lighten3).Text(text).Bold();

    static void Cell(IContainer c, object value, bool bold = false)
    {
        var t = c.Padding(6).AlignRight();
        if (bold) t.Text($"₹ {value:N2}").Bold();
        else t.Text($"₹ {value:N2}");
    }

    // -------- FOOTER --------
    void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(_settings.PdfFooterMessage)
            .FontSize(9)
            .Italic();
    }
}
