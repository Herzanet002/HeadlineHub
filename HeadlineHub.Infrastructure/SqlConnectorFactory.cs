using System.Data;
using System.Data.SqlClient;

namespace HeadlineHub.Infrastructure;

public class SqlConnectorFactory
{
    private readonly string _connectionString;

    public SqlConnectorFactory(string connectionString) 
        => _connectionString = connectionString;

    public IDbConnection CreateConnection() 
        => new SqlConnection(_connectionString);
}