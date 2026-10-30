using NotifyProviderApplication.Models;

namespace NotifyProviderApplication.Providers;

public sealed class GoogleEmailProviderSender : SmtpEmailProviderSenderBase
{
    public override EmailProviderType ProviderType => EmailProviderType.Google;
}
