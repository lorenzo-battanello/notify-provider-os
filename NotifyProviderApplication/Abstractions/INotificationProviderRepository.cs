using NotifyProviderApplication.Models;

namespace NotifyProviderApplication.Abstractions;

public interface INotificationProviderRepository
{
    Task<ProviderConfiguration?> GetByNotificationKeyAsync(string notificationKey, CancellationToken cancellationToken);

    Task<bool> IsSiteAllowedAsync(string notificationKey, string siteHost, CancellationToken cancellationToken);
}
