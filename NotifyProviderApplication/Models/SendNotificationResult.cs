namespace NotifyProviderApplication.Models;

public sealed class SendNotificationResult
{
    public string NotificationKey { get; init; } = string.Empty;

    public EmailProviderType ProviderType { get; init; }

    public int RecipientCount { get; init; }

    public DateTime SentAtUtc { get; init; }
}
