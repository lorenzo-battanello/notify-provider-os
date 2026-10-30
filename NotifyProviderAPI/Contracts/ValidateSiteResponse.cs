namespace NotifyProviderAPI.Contracts;

public sealed class ValidateSiteResponse
{
    public string NotificationKey { get; init; } = string.Empty;

    public string SiteUrl { get; init; } = string.Empty;

    public bool IsAllowed { get; init; }
}
