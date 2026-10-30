using NotifyProviderApplication.Models;

namespace NotifyProviderApplication.Providers;

public sealed class CustomEmailProviderSender : SmtpEmailProviderSenderBase
{
    public override EmailProviderType ProviderType => EmailProviderType.Custom;
}
