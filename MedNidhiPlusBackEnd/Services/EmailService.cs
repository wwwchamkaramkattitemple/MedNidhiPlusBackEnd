using System.Net;
using System.Net.Mail;

namespace MedNidhiPlusBackEnd.Services;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendInvoiceAsync(
        string toEmail,
        string subject,
        string body,
        byte[] pdfBytes,
        string fileName)
    {
        var smtp = _config.GetSection("Smtp");

        var message = new MailMessage
        {
            From = new MailAddress(smtp["From"]),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        message.To.Add(toEmail);
        message.Attachments.Add(new Attachment(
            new MemoryStream(pdfBytes), fileName));

        var client = new SmtpClient(smtp["Host"], int.Parse(smtp["Port"]))
        {
            Credentials = new NetworkCredential(
                smtp["Username"], smtp["Password"]),
            EnableSsl = true
        };

        await client.SendMailAsync(message);
    }
}