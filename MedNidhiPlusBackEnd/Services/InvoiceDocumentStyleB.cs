using MedNidhiPlusBackEnd.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MedNidhiPlusBackEnd.Services;

public class InvoiceDocumentStyleB : IDocument
{
    private readonly InvoiceDetailDto _invoice;
    private readonly SystemSetting _settings;

    public InvoiceDocumentStyleB(InvoiceDetailDto invoice, SystemSetting settings)
    {
        _invoice = invoice;
        _settings = settings;
    }

    public DocumentMetadata GetMetadata() =>
        DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(30);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(11));

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeBody);
            page.Footer().Element(ComposeFooter);
        });
    }

    // ================= HEADER =================
    

    void ComposeHeader(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Row(row =>
            {
                // LOGO
                row.ConstantItem(80)
                   .Image("Assets/mednidhi-logo.png");

                // CLINIC DETAILS
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("MEDNIDHI PLUS")
                        .FontSize(18)
                        .Bold()
                        .FontColor(Colors.Blue.Medium);

                    c.Item().Text(_settings.ClinicName);
                    c.Item().Text("GSTIN: "+_settings.ClinicGstNumber);
                    c.Item().Text("Phone: "+ _settings.ClinicPhone);
                    c.Item().Text("Address: "+ _settings.ClinicAddress);
                });

                // INVOICE META
                row.ConstantItem(200).Column(c =>
                {
                    c.Item().Text($"Invoice #: {_invoice.InvoiceNumber}").Bold();
                    c.Item().Text($"Invoice Date: {_invoice.InvoiceDate:dd/MM/yyyy}");
                    c.Item().Text($"Due Date: {_invoice.DueDate:dd/MM/yyyy}");

                    c.Item().Text($"Status: {_invoice.Status}")
                        .Bold()
                        .FontColor(_invoice.Status == "Paid"
                            ? Colors.Green.Medium
                            : Colors.Orange.Medium);
                });
            });

            // Separator line
            col.Item()
               .PaddingVertical(10)
               .LineHorizontal(1);
        });
    }


    // ================= BODY =================
    void ComposeBody(IContainer container)
    {
        container.PaddingVertical(15).Column(col =>
        {
            // Patient + Invoice info
            col.Item().Row(row =>
            {
                row.RelativeColumn().Element(ComposePatientInfo);
                row.RelativeColumn().Element(ComposeInvoiceInfo);
            });

            col.Item().PaddingTop(15).Element(ComposeItemsTable);

            col.Item().PaddingTop(15).AlignRight().Element(ComposeTotals);
        });
    }

    void ComposePatientInfo(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Text("BILL TO").Bold().FontSize(12);
            col.Item().Text(_invoice.Patient.FullName);
            col.Item().Text(_invoice.Patient.Phone);
            col.Item().Text(_invoice.Patient.Email);
            col.Item().Text(_invoice.Patient.Address);
        });
    }

    void ComposeInvoiceInfo(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Text("INVOICE DETAILS").Bold().FontSize(12);
            col.Item().Text($"Invoice Date: {_invoice.InvoiceDate:dd/MM/yyyy}");
            col.Item().Text($"Due Date: {_invoice.DueDate:dd/MM/yyyy}");
            col.Item().Text($"Payment Method: {_invoice.PaymentMethod ?? "—"}");
        });
    }

    // ================= ITEMS TABLE =================
    void ComposeItemsTable(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(30);
                columns.RelativeColumn();
                columns.ConstantColumn(60);
                columns.ConstantColumn(80);
                columns.ConstantColumn(80);
            });

            // Header
            table.Header(header =>
            {
                header.Cell().Element(HeaderCell).Text("#");
                header.Cell().Element(HeaderCell).Text("Description");
                header.Cell().Element(HeaderCell).AlignRight().Text("Qty");
                header.Cell().Element(HeaderCell).AlignRight().Text("Rate");
                header.Cell().Element(HeaderCell).AlignRight().Text("Amount");
            });

            int i = 1;
            foreach (var item in _invoice.Items)
            {
                table.Cell().Element(Cell).Text($"{i++}");
                table.Cell().Element(Cell).Text(item.Description);
                table.Cell().Element(Cell).AlignRight().Text(item.Quantity.ToString());
                table.Cell().Element(Cell).AlignRight().Text($"₹ {item.UnitPrice:N2}");
                table.Cell().Element(Cell).AlignRight().Text($"₹ {item.TotalAmount:N2}");
            }
        });

        static IContainer HeaderCell(IContainer c) =>
            c.Padding(6).Background(Colors.Grey.Lighten3);

        static IContainer Cell(IContainer c) =>
            c.Padding(6).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
    }

    void ComposeTotals(IContainer container)
    {
        var cgst = _invoice.TaxAmount / 2;
        var sgst = _invoice.TaxAmount / 2;

        container.Column(col =>
        {
            col.Item().Text($"Subtotal: ₹{_invoice.SubTotal:F2}");
            col.Item().Text($"CGST (9%): ₹{cgst:F2}");
            col.Item().Text($"SGST (9%): ₹{sgst:F2}");

            col.Item().PaddingTop(5).Text($"Total: ₹{_invoice.TotalAmount:F2}")
                .Bold()
                .FontSize(12);
        });
    }


    // ================= FOOTER =================
    void ComposeFooter(IContainer container)
    {
        container.AlignCenter().PaddingTop(10).Text(text =>
        {
            text.Span(_settings.PdfFooterMessage)
                .FontSize(9)
                .Italic();
        });
    }
}
