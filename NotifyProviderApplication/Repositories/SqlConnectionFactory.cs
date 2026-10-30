using System.Data;
using Microsoft.Data.SqlClient;
using NotifyProviderApplication.Abstractions;

namespace NotifyProviderApplication.Repositories;

public sealed class SqlConnectionFactory(string connectionString) : ISqlConnectionFactory
{
    private readonly string _connectionString = connectionString;

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}
