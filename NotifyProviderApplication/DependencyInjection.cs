using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotifyProviderApplication.Abstractions;
using NotifyProviderApplication.Providers;
using NotifyProviderApplication.Repositories;
using System.Text;

namespace NotifyProviderApplication;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("NotifyProviderDb");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'NotifyProviderDb' is missing.");
        }

        var sqlBuilder = new SqlConnectionStringBuilder(connectionString);

        // Keep password encoded in config and decode it only at runtime.
        if (!string.IsNullOrWhiteSpace(sqlBuilder.Password))
        {
            try
            {
                var decodedBytes = Convert.FromBase64String(sqlBuilder.Password);
                sqlBuilder.Password = Encoding.UTF8.GetString(decodedBytes);
                connectionString = sqlBuilder.ConnectionString;
            }
            catch (FormatException)
            {
                // If password is not Base64, keep original value unchanged.
            }
        }

        services.AddSingleton<ISqlConnectionFactory>(_ => new SqlConnectionFactory(connectionString));
        services.AddScoped<INotificationProviderRepository, NotificationProviderRepository>();

        services.AddScoped<IEmailProviderSender, GoogleEmailProviderSender>();
        services.AddScoped<IEmailProviderSender, MicrosoftEmailProviderSender>();
        services.AddScoped<IEmailProviderSender, CustomEmailProviderSender>();
        services.AddScoped<IEmailProviderFactory, EmailProviderFactory>();

        services.AddMediatR(typeof(ApplicationMarker).Assembly);

        return services;
    }
}
