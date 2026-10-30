using NotifyProviderApplication.Models;

namespace NotifyProviderApplication.Providers;

public sealed class MicrosoftEmailProviderSender : SmtpEmailProviderSenderBase
{
    public override EmailProviderType ProviderType => EmailProviderType.Microsoft;
}
