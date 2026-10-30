using NotifyProviderApplication.Abstractions;
using NotifyProviderApplication.Models;

namespace NotifyProviderApplication.Providers;

public sealed class EmailProviderFactory(IEnumerable<IEmailProviderSender> senders) : IEmailProviderFactory
{
    private readonly IReadOnlyDictionary<EmailProviderType, IEmailProviderSender> _senders = senders.ToDictionary(sender => sender.ProviderType);

    public IEmailProviderSender Resolve(EmailProviderType providerType)
    {
        if (_senders.TryGetValue(providerType, out var sender))
        {
            return sender;
        }

        throw new NotSupportedException($"Provider '{providerType}' is not registered.");
    }
}
