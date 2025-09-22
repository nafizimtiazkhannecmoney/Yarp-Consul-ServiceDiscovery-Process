using System.Data;
using Microsoft.Data.SqlClient;
using Npgsql;
using visa_direct.Interfaces;
/**
 * @Author: Masud Ahmed
 * @Since: April 2024
 */
namespace visa_direct.Config
{
    public class DbConnection : IIDbConnection
    {
        public readonly string? _connectionStringTSql;
        public readonly string? _connectionStringPgSql;

        public DbConnection(IConfiguration configuration)
        {
            _connectionStringTSql = configuration.GetConnectionString("MssqlConnection");
            _connectionStringPgSql = configuration.GetConnectionString("PgConnection");
        }

        public IDbConnection CreateConnectionPgSql()
        {
            return new NpgsqlConnection(_connectionStringPgSql);
        }

        public IDbConnection CreateConnectionTSql()
        {
            return new SqlConnection(_connectionStringTSql);
        }
    }
}
