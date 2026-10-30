using System.Data;

namespace NotifyProviderApplication.Abstractions;

public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}
