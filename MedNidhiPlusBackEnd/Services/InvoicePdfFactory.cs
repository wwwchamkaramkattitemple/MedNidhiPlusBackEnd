using MedNidhiPlusBackEnd.Models;
using QuestPDF.Infrastructure;

namespace MedNidhiPlusBackEnd.Services;

public static class InvoicePdfFactory
{
    public static IDocument Create( InvoiceDetailDto invoice, SystemSetting settings, string mode,string? design)
    {
        return mode switch
        {
            "Thermal" => new ThermalInvoiceDocument(invoice, settings),

            "Normal" => design switch
            {
                "Classic" => new ClassicInvoiceDocument(invoice,settings),
                "Modern" => new InvoiceDocumentStyleB(invoice, settings),
                "Minimal" => new MinimalInvoiceDocument(invoice, settings),
                _ => throw new ArgumentException("Invalid invoice design")
            },

            _ => throw new ArgumentException("Invalid print mode")
        };
    }
}
