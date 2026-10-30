using NotifyProviderApplication.Models;

namespace NotifyProviderApplication.Abstractions;

public interface IEmailProviderFactory
{
    IEmailProviderSender Resolve(EmailProviderType providerType);
}
