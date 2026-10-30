using MediatR;
using NotifyProviderApplication.Models;

namespace NotifyProviderApplication.Features.SendNotification;

public sealed record SendNotificationCommand(
    string NotificationKey,
    string SiteUrl,
    string Subject,
    string Body,
    bool IsBodyHtml,
    List<string> ToAddresses) : IRequest<SendNotificationResult>;
