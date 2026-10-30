using MediatR;
using Microsoft.AspNetCore.Mvc;
using NotifyProviderAPI.Contracts;
using NotifyProviderApplication.Features.SendNotification;
using NotifyProviderApplication.Features.ValidateSiteAccess;

namespace NotifyProviderAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class NotificationsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("send")]
    [ProducesResponseType(typeof(SendNotificationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SendNotificationResponse>> SendAsync(
        [FromBody] SendNotificationRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.NotificationKey) ||
            string.IsNullOrWhiteSpace(request.SiteUrl) ||
            string.IsNullOrWhiteSpace(request.Subject) ||
            string.IsNullOrWhiteSpace(request.Body))
        {
            return BadRequest("NotificationKey, SiteUrl, Subject, Body are required.");
        }

        try
        {           
            var result = await _mediator.Send(
                new SendNotificationCommand(
                    request.NotificationKey,
                    request.SiteUrl,
                    request.Subject,
                    request.Body,
                    request.IsBodyHtml,
                    request.ToAddresses),
                cancellationToken);

            return Ok(new SendNotificationResponse
            {
                NotificationKey = result.NotificationKey,
                Provider = result.ProviderType.ToString(),
                RecipientCount = result.RecipientCount,
                SentAtUtc = result.SentAtUtc
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            // Log the exception (not shown here for brevity)
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }

    [HttpPost("validate-site")]
    [ProducesResponseType(typeof(ValidateSiteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ValidateSiteResponse>> ValidateSiteAsync(
        [FromBody] ValidateSiteRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.NotificationKey) || string.IsNullOrWhiteSpace(request.SiteUrl))
        {
            return BadRequest("NotificationKey and SiteUrl are required.");
        }

        try
        {
            var isAllowed = await _mediator.Send(
                new ValidateSiteAccessQuery(request.NotificationKey, request.SiteUrl),
                cancellationToken);

            return Ok(new ValidateSiteResponse
            {
                NotificationKey = request.NotificationKey,
                SiteUrl = request.SiteUrl,
                IsAllowed = isAllowed
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
