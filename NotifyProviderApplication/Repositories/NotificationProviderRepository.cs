using Dapper;
using NotifyProviderApplication.Abstractions;
using NotifyProviderApplication.Models;
using System.Data.Common;

namespace NotifyProviderApplication.Repositories;

public sealed class NotificationProviderRepository : INotificationProviderRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public NotificationProviderRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ProviderConfiguration?> GetByNotificationKeyAsync(string notificationKey, CancellationToken cancellationToken)
    {
        const string providerSql = """
            SELECT
                Id,
                NotificationKey,
                ProviderType,
                SmtpHost,
                SmtpPort,
                UseSsl,
                Username,
                Password,
                FromAddress,
                FromName
            FROM dbo.NotificationProviderConfigurations
            WHERE NotificationKey = @NotificationKey;
            """;

        const string recipientsSql = """
            SELECT EmailAddress
            FROM dbo.NotificationRecipients
            WHERE NotificationKey = @NotificationKey;
            """;

        using var connection = _connectionFactory.CreateConnection();
        if (connection is DbConnection dbConnection)
        {
            await dbConnection.OpenAsync(cancellationToken);
        }
        else
        {
            connection.Open();
        }

        var configuration = await connection.QueryFirstOrDefaultAsync<ProviderConfigurationRecord>(
            new CommandDefinition(providerSql, new { NotificationKey = notificationKey }, cancellationToken: cancellationToken));

        if (configuration is null)
        {
            return null;
        }

        var recipients = await connection.QueryAsync<string>(
            new CommandDefinition(recipientsSql, new { NotificationKey = notificationKey }, cancellationToken: cancellationToken));

        return new ProviderConfiguration
        {
            Id = configuration.Id,
            NotificationKey = configuration.NotificationKey,
            ProviderType = (EmailProviderType)configuration.ProviderType,
            SmtpHost = configuration.SmtpHost,
            SmtpPort = configuration.SmtpPort,
            UseSsl = configuration.UseSsl,
            Username = configuration.Username,
            Password = configuration.Password,
            FromAddress = configuration.FromAddress,
            FromName = configuration.FromName,
            ToAddresses = recipients.ToArray()
        };
    }

    public async Task<bool> IsSiteAllowedAsync(string notificationKey, string siteHost, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT TOP 1 1
            FROM dbo.NotificationAllowedSites
            WHERE NotificationKey = @NotificationKey
              AND (
                    Host = @SiteHost
                    OR (AllowSubdomains = 1 AND @SiteHost LIKE '%.' + Host)
                  );
            """;

        using var connection = _connectionFactory.CreateConnection();
        if (connection is DbConnection dbConnection)
        {
            await dbConnection.OpenAsync(cancellationToken);
        }
        else
        {
            connection.Open();
        }

        var result = await connection.QueryFirstOrDefaultAsync<int?>(
            new CommandDefinition(
                sql,
                new { NotificationKey = notificationKey, SiteHost = siteHost },
                cancellationToken: cancellationToken));

        return result.HasValue;
    }

    private sealed record ProviderConfigurationRecord(
        int Id,
        string NotificationKey,
        int ProviderType,
        string SmtpHost,
        int SmtpPort,
        bool UseSsl,
        string Username,
        string Password,
        string FromAddress,
        string FromName);
}
