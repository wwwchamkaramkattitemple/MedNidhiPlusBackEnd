using MedNidhiPlusBackEnd.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MedNidhiPlusBackEnd.Services;

public class ClassicInvoiceDocument : IDocument
{
    private readonly InvoiceDetailDto _invoice;
    private readonly SystemSetting _settings;

    public ClassicInvoiceDocument(InvoiceDetailDto invoice, SystemSetting settings)
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
            page.Margin(30);
            page.DefaultTextStyle(x => x.FontSize(10));

            page.Content().Column(col =>
            {
                col.Item().Element(ComposeHeader);
                col.Item().PaddingVertical(10).Element(ComposeMeta);
                col.Item().Element(ComposeItemsTable);
                col.Item().PaddingTop(10).AlignRight().Element(ComposeTotals);
                col.Item().PaddingTop(15).Element(ComposePaymentInfo);
                col.Item().PaddingTop(20).Element(ComposeFooter);
            });
        });
    }

    // ================= HEADER =================
    void ComposeHeader(IContainer container)
    {
        container.Border(1).Padding(10).Column(col =>
        {
            col.Item().AlignCenter()
                .Text(_settings.ClinicName)
                .FontSize(16)
                .Bold();

            if (!string.IsNullOrWhiteSpace(_settings.ClinicAddress))
                col.Item().AlignCenter().Text(_settings.ClinicAddress);

            col.Item().AlignCenter()
                .Text($"Phone: {_settings.ClinicPhone}");
        });
    }


    // ================= META =================
    void ComposeMeta(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(c =>
            {
                c.RelativeColumn();
                c.RelativeColumn();
            });

            table.Cell().Element(MetaCell).Text($"Invoice No: {_invoice.InvoiceNumber}");
            table.Cell().Element(MetaCell).Text($"Date: {_invoice.InvoiceDate:dd/MM/yyyy}");

            table.Cell().Element(MetaCell).Text($"Patient: {_invoice.Patient.FullName}");
            table.Cell().Element(MetaCell).Text($"Status: {_invoice.Status}");

            table.Cell().Element(MetaCell).Text($"Phone: {_invoice.Patient.Phone}");
            table.Cell().Element(MetaCell).Text($"Payment: {_invoice.PaymentMethod ?? "-"}");
        });
    }

    static IContainer MetaCell(IContainer c) =>
        c.Border(1).Padding(6);

    // ================= ITEMS TABLE =================
    void ComposeItemsTable(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(30);
                columns.RelativeColumn();
                columns.ConstantColumn(50);
                columns.ConstantColumn(70);
                columns.ConstantColumn(80);
            });

            table.Header(header =>
            {
                header.Cell().Element(HeaderCell).Text("Sl");
                header.Cell().Element(HeaderCell).Text("Description");
                header.Cell().Element(HeaderCell).AlignRight().Text("Qty");
                header.Cell().Element(HeaderCell).AlignRight().Text("Rate");
                header.Cell().Element(HeaderCell).AlignRight().Text("Amount");
            });

            int index = 1;
            foreach (var item in _invoice.Items)
            {
                table.Cell().Element(Cell).Text(index++.ToString());
                table.Cell().Element(Cell).Text(item.Description);
                table.Cell().Element(Cell).AlignRight().Text(item.Quantity.ToString());
                table.Cell().Element(Cell).AlignRight().Text(item.UnitPrice.ToString("N2"));
                table.Cell().Element(Cell).AlignRight().Text(item.TotalAmount.ToString("N2"));
            }
        });
    }



    static IContainer HeaderCell(IContainer c) =>
        c.Border(1).Padding(6).Background(Colors.Grey.Lighten3);

    static IContainer Cell(IContainer c) =>
        c.Border(1).Padding(6);

    void ComposeTotals(IContainer container)
    {
        var cgst = _invoice.TaxAmount / 2;
        var sgst = _invoice.TaxAmount / 2;

        container.Table(table =>
        {
            table.ColumnsDefinition(c =>
            {
                c.RelativeColumn();
                c.ConstantColumn(120);
            });

            table.Cell().Text("Subtotal:");
            table.Cell().AlignRight().Text(_invoice.SubTotal.ToString("N2"));

            table.Cell().Text("CGST:");
            table.Cell().AlignRight().Text(cgst.ToString("N2"));

            table.Cell().Text("SGST:");
            table.Cell().AlignRight().Text(sgst.ToString("N2"));

            table.Cell().Text("TOTAL:");
            table.Cell().AlignRight().Text(_invoice.TotalAmount.ToString("N2"));
        });
    }

    void ComposePaymentInfo(IContainer container)
    {
        container.Border(1).Padding(8).Column(col =>
        {
            col.Item().Text($"Paid Amount: ₹ {_invoice.PaidAmount:N2}");
            col.Item().Text($"Balance: ₹ {_invoice.BalanceAmount:N2}");
            col.Item().Text($"Payment Date: {_invoice.PaymentDate:dd/MM/yyyy}");
        });
    }

    void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(text =>
        {
            text.Span(_settings.PdfFooterMessage)
                .FontSize(9);
        });
    }




}

