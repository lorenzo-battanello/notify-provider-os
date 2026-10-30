namespace NotifyProviderApplication.Models;

public sealed class ProviderConfiguration
{
    public int Id { get; init; }

    public string NotificationKey { get; init; } = string.Empty;

    public EmailProviderType ProviderType { get; init; }

    public string SmtpHost { get; init; } = string.Empty;

    public int SmtpPort { get; init; }

    public bool UseSsl { get; init; }

    public string Username { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;

    public string FromAddress { get; init; } = string.Empty;

    public string FromName { get; init; } = string.Empty;

    public IReadOnlyCollection<string> ToAddresses { get; init; } = Array.Empty<string>();
}
