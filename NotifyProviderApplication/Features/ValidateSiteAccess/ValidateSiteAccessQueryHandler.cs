using MediatR;
using NotifyProviderApplication.Abstractions;

namespace NotifyProviderApplication.Features.ValidateSiteAccess;

public sealed class ValidateSiteAccessQueryHandler(INotificationProviderRepository repository) : IRequestHandler<ValidateSiteAccessQuery, bool>
{
    private readonly INotificationProviderRepository _repository = repository;

    public async Task<bool> Handle(ValidateSiteAccessQuery request, CancellationToken cancellationToken)
    {
        if (!Uri.TryCreate(request.SiteUrl, UriKind.Absolute, out var uri) || string.IsNullOrWhiteSpace(uri.Host))
        {
            throw new InvalidOperationException("SiteUrl must be a valid absolute URL.");
        }

        var host = uri.Host.ToLowerInvariant();
        return await _repository.IsSiteAllowedAsync(request.NotificationKey, host, cancellationToken);
    }
}
