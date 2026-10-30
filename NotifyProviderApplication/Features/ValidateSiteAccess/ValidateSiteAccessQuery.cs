using MediatR;

namespace NotifyProviderApplication.Features.ValidateSiteAccess;

public sealed record ValidateSiteAccessQuery(string NotificationKey, string SiteUrl) : IRequest<bool>;
