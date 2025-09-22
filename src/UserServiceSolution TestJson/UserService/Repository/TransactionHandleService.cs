using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using visa_direct;
using visa_direct.Interfaces;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static Azure.Core.HttpHeader;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Npgsql;
namespace pay_at.Interfaces
{
    public class TransactionHandleService : ITransactionService
    {
        private readonly ILogger<TransactionHandleService> _logger;
        public IIDbConnection _dbConnection;

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        public TransactionHandleService(IConfiguration config, IIDbConnection dbConnection, ILogger<TransactionHandleService> logger)
        {
            _dbConnection = dbConnection;
            _logger = logger;
        }

        public string? Process(string inParam, string spName, string provider)
        {
           return HandleDbData(inParam, spName, provider);
        }

        /// <summary>
        /// Execute any stored procedure using the parameters and spName.
        /// </summary>
        /// <param name="inParams"> Contains input Parameters for stored procedure. </param>
        /// <param name="spName"> Contains stored procedure name </param>
        /// <param name="isOutputRequired"> Contains boolean value </param>
        /// <returns>
        /// <see cref="String"/> returns json or other data types specified in Param List.
        /// </returns>
        private string? HandleDbData(string jsonInputParam, string spName, string provider)
        {
            string? outputStr = null;

            using (IDbConnection dbConnection = provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase) ? _dbConnection.CreateConnectionTSql(): _dbConnection.CreateConnectionPgSql())
            {
                dbConnection.Open();

                using (IDbTransaction iDbTransaction = dbConnection.BeginTransaction(IsolationLevel.ReadCommitted))
                using (IDbCommand command = dbConnection.CreateCommand())
                {
                    try
                    {
                        command.Transaction = iDbTransaction;
                        command.CommandText = spName;

                        // Differentiate between PostgreSQL and SQL Server
                        if (provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            var inputParam = new SqlParameter("@_json", SqlDbType.NVarChar, -1)
                            {
                                Value = jsonInputParam
                            };
                            command.Parameters.Add(inputParam);
                        }
                        else if (provider.Equals("Postgres", StringComparison.OrdinalIgnoreCase))
                        {
                            // PostgreSQL CALL syntax instead of CommandType.StoredProcedure
                            command.CommandType = CommandType.Text;
                            command.CommandText = $"CALL public.{spName}(null::text,@_json::text)";
                            var inputParam = new NpgsqlParameter("@_json", NpgsqlTypes.NpgsqlDbType.Text)
                            {
                                Value = jsonInputParam
                            };
                            command.Parameters.Add(inputParam);
                        }

                        // Define output parameter
                        IDbDataParameter outputParam = null;

                        if (provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
                        {
                            outputParam = new SqlParameter("@_rs_out", SqlDbType.NVarChar, -1)
                            {
                                Direction = ParameterDirection.Output
                            };
                            command.Parameters.Add(outputParam);
                        }

                        if (provider.Equals("Postgres", StringComparison.OrdinalIgnoreCase)) {
                            var reader = command.ExecuteReader();

                            if (reader.Read())
                                outputStr = reader.IsDBNull(0) ? "{}" : reader.GetString(0);

                            dbConnection.Close();

                            return outputStr;
                        }


                        // Execute
                        command.ExecuteNonQuery();

                        // Retrieve result
                        outputStr = outputParam.Value?.ToString();

                        iDbTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error while executing {SpName}", spName);
                        if(provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase)) iDbTransaction.Rollback();
                        throw;
                    }
                }
            }

            return outputStr;
        }
    }
}

