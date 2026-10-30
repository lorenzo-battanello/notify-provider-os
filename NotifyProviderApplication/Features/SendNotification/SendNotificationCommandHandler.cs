using MediatR;
using NotifyProviderApplication.Abstractions;
using NotifyProviderApplication.Models;

namespace NotifyProviderApplication.Features.SendNotification;

public sealed class SendNotificationCommandHandler(
    INotificationProviderRepository repository,
    IEmailProviderFactory providerFactory) : IRequestHandler<SendNotificationCommand, SendNotificationResult>
{
    private readonly INotificationProviderRepository _repository = repository;
    private readonly IEmailProviderFactory _providerFactory = providerFactory;

    public async Task<SendNotificationResult> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        var siteHost = ExtractHost(request.SiteUrl);
        var isAllowed = await _repository.IsSiteAllowedAsync(request.NotificationKey, siteHost, cancellationToken);

        if (!isAllowed)
        {
            throw new UnauthorizedAccessException($"Site '{siteHost}' is not allowed for notification key '{request.NotificationKey}'.");
        }

        var configuration = await _repository.GetByNotificationKeyAsync(request.NotificationKey, cancellationToken);

        if (configuration is null)
        {
            throw new InvalidOperationException($"No provider configuration found for notification key '{request.NotificationKey}'.");
        }

        if (request.ToAddresses.Count == 0 && configuration.ToAddresses.Count == 0) 
        {
            throw new InvalidOperationException($"No recipients configured for notification key '{request.NotificationKey}'.");
        }

        var message = new EmailDispatchMessage
        {
            Subject = request.Subject,
            Body = request.Body,
            IsBodyHtml = request.IsBodyHtml
        };

        var providerSender = _providerFactory.Resolve(configuration.ProviderType);
        await providerSender.SendAsync(message, request.ToAddresses, configuration, cancellationToken);

        return new SendNotificationResult
        {
            NotificationKey = request.NotificationKey,
            ProviderType = configuration.ProviderType,
            RecipientCount = request.ToAddresses.Count,
            SentAtUtc = DateTime.UtcNow
        };
    }

    private static string ExtractHost(string siteUrl)
    {
        if (!Uri.TryCreate(siteUrl, UriKind.Absolute, out var uri) || string.IsNullOrWhiteSpace(uri.Host))
        {
            throw new InvalidOperationException("SiteUrl must be a valid absolute URL.");
        }

        return uri.Host.ToLowerInvariant();
    }
}
