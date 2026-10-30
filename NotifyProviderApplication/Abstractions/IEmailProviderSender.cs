using NotifyProviderApplication.Models;

namespace NotifyProviderApplication.Abstractions;

public interface IEmailProviderSender
{
    EmailProviderType ProviderType { get; }

    Task SendAsync(EmailDispatchMessage message, List<string> toAddresses, ProviderConfiguration configuration, CancellationToken cancellationToken);
}
