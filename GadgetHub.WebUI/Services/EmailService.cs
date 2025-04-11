using System.Text;
using GadgetHub.WebUI.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace GadgetHub.WebUI.Services;

public interface IEmailService
{
    void SendOrderConfirmationEmail(CheckoutViewModel model);
}

public class EmailService : IEmailService
{
    private readonly string fromEmail = "";
    private readonly string fromName = "";
    private readonly string smtpPassword = "";

    private readonly int smtpPort = 5555;

    // For production, these settings should come from configuration
    private readonly string smtpServer = "";
    private readonly string smtpUsername = "";

    public void SendOrderConfirmationEmail(CheckoutViewModel model)
    {
        // Create the email message
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, fromEmail));
        message.To.Add(new MailboxAddress("", model.Email));
        message.Subject = "Order Confirmation";

        // Build the email body with order and shipping details
        var builder = new BodyBuilder();
        var sb = new StringBuilder();
        sb.AppendLine("Thank you for your order!");
        sb.AppendLine();
        sb.AppendLine("Order Summary:");
        foreach (var item in model.CartItems)
            sb.AppendLine($"{item.ProductName} x {item.Quantity} - {(item.UnitPrice * item.Quantity).ToString("C")}");
        sb.AppendLine();
        sb.AppendLine($"Total: {model.Total.ToString("C")}");
        sb.AppendLine();
        sb.AppendLine("Shipping Details:");
        sb.AppendLine($"Name: {model.FullName}");
        sb.AppendLine($"Address: {model.Address}");
        sb.AppendLine($"City: {model.City}");
        if (!string.IsNullOrEmpty(model.State))
            sb.AppendLine($"State: {model.State}");
        sb.AppendLine($"Zip Code: {model.ZipCode}");
        sb.AppendLine($"Country: {model.Country}");
        builder.TextBody = sb.ToString();
        message.Body = builder.ToMessageBody();

        // Send the email using MailKit
        using (var client = new SmtpClient())
        {
            // Accept all SSL certificates for demo purposes (remove in production)
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            client.Connect(smtpServer, smtpPort, SecureSocketOptions.StartTls);
            client.Authenticate(smtpUsername, smtpPassword);
            client.Send(message);
            client.Disconnect(true);
        }
    }
}