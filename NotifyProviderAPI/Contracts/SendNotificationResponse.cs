namespace NotifyProviderAPI.Contracts;

public sealed class SendNotificationResponse
{
    public string NotificationKey { get; init; } = string.Empty;

    public string Provider { get; init; } = string.Empty;

    public int RecipientCount { get; init; }

    public DateTime SentAtUtc { get; init; }
}
