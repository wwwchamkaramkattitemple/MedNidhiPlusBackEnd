using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using MedNidhiPlusBackEnd.Models;

namespace MedNidhiPlusBackEnd.Services;

public class ThermalInvoiceDocument : IDocument
{
    private readonly InvoiceDetailDto _invoice;
    private readonly SystemSetting _settings;

    public ThermalInvoiceDocument(InvoiceDetailDto invoice, SystemSetting settings)
    {
        _invoice = invoice;
        _settings = settings;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            // 80mm ≈ 226 points
            page.Size(new PageSize(226, PageSizes.A4.Height));
            page.Margin(8);
            page.DefaultTextStyle(x => x.FontSize(9));

            page.Content().Column(col =>
            {
                col.Spacing(4);

                col.Item().Element(ComposeHeader);
                col.Item().Element(Separator);

                col.Item().Element(ComposeInvoiceMeta);
                col.Item().Element(Separator);

                col.Item().Element(ComposePatient);
                col.Item().Element(Separator);

                col.Item().Element(ComposeItems);
                col.Item().Element(Separator);

                col.Item().Element(ComposeTotals);
                col.Item().Element(Separator);

                col.Item().Element(ComposeFooter);
            });
        });
    }

    void ComposeHeader(IContainer container)
    {
        container.Border(1).Padding(10).Column(col =>
        {
            col.Item().AlignCenter()
                .Text(_settings.ClinicName)
                .FontSize(11)
                .Bold();

            if (!string.IsNullOrWhiteSpace(_settings.ClinicAddress))
                col.Item().AlignCenter().Text(_settings.ClinicAddress);

            col.Item().AlignCenter()
                .Text($"Phone: {_settings.ClinicPhone}");
        });
    }

    void ComposeInvoiceMeta(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Text($"Invoice: {_invoice.InvoiceNumber}");
            col.Item().Text($"Date: {_invoice.InvoiceDate:dd/MM/yyyy}");
        });
    }

    void ComposePatient(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Text($"Patient: {_invoice.Patient.FullName}");
        });
    }

    void ComposeItems(IContainer container)
    {
        container.Column(col =>
        {
            foreach (var item in _invoice.Items)
            {
                col.Item().Text(item.Description).Bold();

                col.Item().Row(row =>
                {
                    row.RelativeItem().Text($"{item.Quantity} x {item.UnitPrice:N0}");
                    row.ConstantItem(50)
                        .AlignRight()
                        .Text(item.TotalAmount.ToString("N0"));
                });
            }
        });
    }

    void ComposeTotals(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Row(row =>
            {
                row.RelativeItem().Text("Subtotal");
                row.ConstantItem(50).AlignRight()
                    .Text(_invoice.SubTotal.ToString("N0"));
            });

            col.Item().Row(row =>
            {
                row.RelativeItem().Text("Tax");
                row.ConstantItem(50).AlignRight()
                    .Text(_invoice.TaxAmount.ToString("N0"));
            });

            col.Item().PaddingTop(2).Row(row =>
            {
                row.RelativeItem().Text("TOTAL").Bold();
                row.ConstantItem(50).AlignRight()
                    .Text(_invoice.TotalAmount.ToString("N0"))
                    .Bold();
            });
        });
    }

    void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Column(col =>
        {
            col.Item().Text("PAID").Bold();
            col.Item().PaddingTop(2).Text(_settings.PdfThankYouMessage);
        });
    }

    static void Separator(IContainer container)
    {
        container.LineHorizontal(1);
    }






}


