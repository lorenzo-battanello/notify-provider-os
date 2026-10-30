namespace NotifyProviderAPI.Contracts;

public sealed class ValidateSiteRequest
{
    public string NotificationKey { get; init; } = string.Empty;

    public string SiteUrl { get; init; } = string.Empty;
}
