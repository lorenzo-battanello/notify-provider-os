using System.Net;
using System.Net.Mail;
using System.Text;
using NotifyProviderApplication.Abstractions;
using NotifyProviderApplication.Models;

namespace NotifyProviderApplication.Providers;

public abstract class SmtpEmailProviderSenderBase : IEmailProviderSender
{
    public abstract EmailProviderType ProviderType { get; }

    public virtual async Task SendAsync(EmailDispatchMessage message, List<string> toAddresses, ProviderConfiguration configuration, CancellationToken cancellationToken)
    {
        using var mail = new MailMessage
        {
            Subject = message.Subject,
            Body = message.Body,
            IsBodyHtml = message.IsBodyHtml,
            From = new MailAddress(configuration.FromAddress, configuration.FromName)
        };

        var rec = toAddresses is null || toAddresses.Count == 0 ? configuration.ToAddresses : toAddresses;

        foreach (var recipient in rec)
        {
            mail.To.Add(recipient);
        }

        var smtpPassword = DecodeBase64IfNeeded(configuration.Password);

        using var client = new SmtpClient(configuration.SmtpHost, configuration.SmtpPort)
        {
            EnableSsl = configuration.UseSsl,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(configuration.Username, smtpPassword)
        };

        using var registration = cancellationToken.Register(client.SendAsyncCancel);
        try
        {
            await client.SendMailAsync(mail, cancellationToken);
        }
        catch (SmtpException ex)
        {
            throw new InvalidOperationException(
                $"SMTP authentication failed for provider '{ProviderType}'. Check Username, password/credential requirements for the provider, SSL and port for host '{configuration.SmtpHost}:{configuration.SmtpPort}'.",
                ex);
        }
    }

    private static string DecodeBase64IfNeeded(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        try
        {
            var decodedBytes = Convert.FromBase64String(value);
            var decoded = Encoding.UTF8.GetString(decodedBytes);
            return string.IsNullOrWhiteSpace(decoded) ? value : decoded;
        }
        catch (FormatException)
        {
            return value;
        }
    }
}
