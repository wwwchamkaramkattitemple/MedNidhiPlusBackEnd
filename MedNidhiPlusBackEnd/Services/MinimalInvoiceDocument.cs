using MedNidhiPlusBackEnd.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MedNidhiPlusBackEnd.Services;

public class MinimalInvoiceDocument : IDocument
{
    private readonly InvoiceDetailDto _invoice;
    private readonly SystemSetting _settings;

    public MinimalInvoiceDocument(InvoiceDetailDto invoice, SystemSetting settings)
    {
        _invoice = invoice;
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

                col.Item().Element(ComposeHeader);
                col.Item().Element(ComposeInvoiceMeta);
                col.Item().LineHorizontal(1);

                col.Item().Element(ComposeItems);
                col.Item().LineHorizontal(1);

                col.Item().Element(ComposeTotals);
                col.Item().Element(ComposeFooter);
            });
        });
    }

    // ================= HEADER =================
    //void ComposeHeader(IContainer container)
    //{
    //    container.AlignCenter().Column(col =>
    //    {
    //        col.Item().Text(_settings.ClinicName)
    //            .Bold()
    //            .FontSize(14);

    //        col.Item().Text("Multi-Speciality Healthcare");
    //        col.Item().Text("Phone: +91 98765 43210");
    //    });
    //}

    void ComposeHeader(IContainer container)
    {
        container.Border(1).Padding(10).Column(col =>
        {
            col.Item().AlignCenter()
                .Text(_settings.ClinicName)
                .FontSize(14)
                .Bold();

            if (!string.IsNullOrWhiteSpace(_settings.ClinicAddress))
                col.Item().AlignCenter().Text(_settings.ClinicAddress);

            col.Item().AlignCenter()
                .Text($"Phone: {_settings.ClinicPhone}");
        });
    }

    // ================= INVOICE META =================
    void ComposeInvoiceMeta(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Text($"Invoice #: {_invoice.InvoiceNumber}");
            col.Item().Text($"Date: {_invoice.InvoiceDate:dd/MM/yyyy}");
            col.Item().Text($"Status: {_invoice.Status}");
            col.Item().Text($"Patient: {_invoice.Patient.FullName}");
        });
    }


    // ================= ITEMS =================
    void ComposeItems(IContainer container)
    {
        container.Column(col =>
        {
            foreach (var item in _invoice.Items)
            {
                col.Item().Row(row =>
                {
                    row.RelativeItem().Text(item.Description);
                    row.ConstantItem(60).AlignRight()
                        .Text($"{item.Quantity} x {item.UnitPrice:N0}");
                    row.ConstantItem(60).AlignRight()
                        .Text(item.TotalAmount.ToString("N0"));
                });
            }
        });
    }


    // ================= TOTALS =================
    void ComposeTotals(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().AlignRight().Text($"Subtotal: {_invoice.SubTotal:N2}");
            col.Item().AlignRight().Text($"Tax: {_invoice.TaxAmount:N2}");

            col.Item().PaddingTop(4)
                .AlignRight()
                .Text($"TOTAL: {_invoice.TotalAmount:N2}")
                .Bold();
        });
    }

    void ComposeFooter(IContainer container)
    {
        container.PaddingTop(10).AlignCenter().Text(text =>
        {
            text.Span(_settings.PdfFooterMessage)
                .FontSize(9)
                .Italic();
        });
    }


}

