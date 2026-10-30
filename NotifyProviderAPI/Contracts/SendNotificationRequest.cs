namespace NotifyProviderAPI.Contracts;

public sealed class SendNotificationRequest
{
    public string NotificationKey { get; init; } = string.Empty;

    public string SiteUrl { get; init; } = string.Empty;

    public string Subject { get; init; } = string.Empty;

    public string Body { get; init; } = string.Empty;

    public bool IsBodyHtml { get; init; }

    public List<string> ToAddresses { get; init; } = [];
}
