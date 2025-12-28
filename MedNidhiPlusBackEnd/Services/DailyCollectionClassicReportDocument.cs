using MedNidhiPlusBackEnd.API.Models;
using MedNidhiPlusBackEnd.Models;
using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;



public class DailyCollectionClassicReportDocument : IDocument
{
    private readonly List<DailyCollectionReportDto> _data;
    private readonly DateTime _fromDate;
    private readonly DateTime _toDate;
    private readonly SystemSetting _settings;

    public DailyCollectionClassicReportDocument(
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
            page.DefaultTextStyle(x => x.FontSize(10));

            page.Content().Column(col =>
            {
                col.Item().Element(ComposeHeader);
                col.Item().PaddingVertical(10).Element(ComposeTable);
                col.Item().PaddingTop(10).AlignRight().Element(ComposeTotals);
                col.Item().PaddingTop(15).Element(ComposeFooter);
            });
        });
    }

    void ComposeHeader(IContainer container)
    {
        container.Border(1).Padding(10).AlignCenter().Column(col =>
        {
            col.Item().Text(_settings.ClinicName).FontSize(16).Bold();
            col.Item().Text("Daily Collection Report");
            col.Item().Text(
                $"From {_fromDate.ToLocalTime():dd/MM/yyyy} to {_toDate.ToLocalTime():dd/MM/yyyy}"
            );
        });
    }

    void ComposeTable(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(c =>
            {
                c.RelativeColumn(); // Date
                c.RelativeColumn(); // Cash
                c.RelativeColumn(); // Card
                c.RelativeColumn(); // UPI
                c.RelativeColumn(); // Other
                c.RelativeColumn(); // Total
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

            foreach (var r in _data)
            {
                DateCell(table.Cell(), r.Date);
                AmountCell(table.Cell(), r.CashCollection);
                AmountCell(table.Cell(), r.CardCollection);
                AmountCell(table.Cell(), r.UpiCollection);
                AmountCell(table.Cell(), r.OtherCollection);
                AmountCell(table.Cell(), r.TotalCollection);
            }
        });
    }

    static void DateCell(IContainer c, DateTime date) =>
    c.Border(1)
     .Padding(6)
     .Text(date.ToString("dd/MM/yyyy"));

    static void AmountCell(IContainer c, decimal amount) =>
    c.Border(1)
     .Padding(6)
     .AlignRight()
     .Text($"₹ {amount:N2}");



    static void Header(IContainer c, string text) =>
        c.Border(1).Padding(6).Background(Colors.Grey.Lighten3).Text(text).Bold();

    static void Row(IContainer c, object val) =>
        c.Border(1).Padding(6).AlignRight().Text($"₹ {val:N2}");

    void ComposeTotals(IContainer container)
    {
        container.Text(
            $"Grand Total: ₹ {_data.Sum(x => x.TotalCollection):N2}"
        ).Bold();
    }

    void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(_settings.PdfFooterMessage).FontSize(9);
    }
}
