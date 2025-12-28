using MedNidhiPlusBackEnd.API.Models;
using MedNidhiPlusBackEnd.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public class DailyCollectionMinimalReportDocument : IDocument
{
    private readonly List<DailyCollectionReportDto> _data;
    private readonly DateTime _fromDate;
    private readonly DateTime _toDate;
    private readonly SystemSetting _settings;

    public DailyCollectionMinimalReportDocument(
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
                col.Spacing(6);

                col.Item().AlignCenter().Text(_settings.ClinicName).Bold();
                col.Item().AlignCenter().Text("Daily Collection Report");
                col.Item().AlignCenter().Text(
                    $"{_fromDate.ToLocalTime():dd/MM/yyyy} - {_toDate.ToLocalTime():dd/MM/yyyy}"
                );

                col.Item().LineHorizontal(1);

                foreach (var r in _data)
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text(r.Date.ToString("dd/MM/yyyy"));
                        row.ConstantItem(80).AlignRight().Text(r.TotalCollection.ToString("N2"));
                    });
                }

                col.Item().LineHorizontal(1);
                col.Item().AlignRight()
                    .Text($"TOTAL: {_data.Sum(x => x.TotalCollection):N2}")
                    .Bold();

                col.Item().PaddingTop(8).AlignCenter()
                    .Text(_settings.PdfFooterMessage)
                    .FontSize(9);
            });
        });
    }
}
