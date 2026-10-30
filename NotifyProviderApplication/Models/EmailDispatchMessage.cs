namespace NotifyProviderApplication.Models;

public sealed class EmailDispatchMessage
{
    public string Subject { get; init; } = string.Empty;

    public string Body { get; init; } = string.Empty;

    public bool IsBodyHtml { get; init; }
}
