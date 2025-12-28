using MedNidhiPlusBackEnd.API.Models;
using MedNidhiPlusBackEnd.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MedNidhiPlusBackEnd.Services;

public class DailyCollectionReportDocument : IDocument
{
    private readonly List<DailyCollectionReportDto> _data;
    private readonly DateTime _fromDate;
    private readonly DateTime _toDate;
    private readonly SystemSetting _settings;

    public DailyCollectionReportDocument(
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
            page.Margin(25);
            page.DefaultTextStyle(x => x.FontSize(10));

            page.Content().Column(col =>
            {
                col.Spacing(10);

                col.Item().Element(ComposeHeader);
                col.Item().Element(ComposeDateRange);
                col.Item().Element(ComposeTable);
                col.Item().PaddingTop(10).Element(ComposeTotals);
                col.Item().PaddingTop(15).Element(ComposeFooter);
            });
        });
    }

    void ComposeHeader(IContainer container)
    {
        container.AlignCenter().Column(col =>
        {
            col.Item().Text(_settings.ClinicName)
                .FontSize(16)
                .Bold();

            if (!string.IsNullOrWhiteSpace(_settings.ClinicAddress))
                col.Item().Text(_settings.ClinicAddress);

            col.Item().Text($"Daily Collection Report")
                .FontSize(13)
                .Bold();
        });
    }

    void ComposeDateRange(IContainer container)
    {
        container.AlignCenter().Text(
            $"From {_fromDate:dd/MM/yyyy} to {_toDate:dd/MM/yyyy}"
        );
    }

    void ComposeTable(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(c =>
            {
                c.ConstantColumn(80);
                c.RelativeColumn();
                c.RelativeColumn();
                c.RelativeColumn();
                c.RelativeColumn();
                c.RelativeColumn();
            });

            table.Header(h =>
            {
                HeaderCell(h.Cell(), "Date");
                HeaderCell(h.Cell(), "Cash");
                HeaderCell(h.Cell(), "Card");
                HeaderCell(h.Cell(), "UPI");
                HeaderCell(h.Cell(), "Other");
                HeaderCell(h.Cell(), "Total");
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

    void HeaderCell(IContainer c, string text) =>
    c.Border(1).Padding(5).Background(Colors.Grey.Lighten3)
     .Text(text).Bold();

    void Cell(IContainer c, object value, bool bold = false)
    {
        var cell = c.Border(1).Padding(5).AlignRight();
        if (bold)
            cell.Text($"₹ {value:N2}").Bold();
        else
            cell.Text($"₹ {value:N2}");
    }

    void ComposeTotals(IContainer container)
    {
        var grandTotal = _data.Sum(x => x.TotalCollection);

        container.AlignRight().Text(
            $"Grand Total : ₹ {grandTotal:N2}"
        ).Bold();
    }


    void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(_settings.PdfFooterMessage
        ).FontSize(9);
    }



}
