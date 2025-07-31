using Npgsql;
using SaGoAMLReporting.Constants;
using SaGoAMLReporting.Service.Interfaces;
using System.Data;
using System.Reflection.Metadata;
using System.Text.Json;

namespace SaGoAMLReporting.Service
{
    public class SqlService : ISqlService
    {

        private string _connectionString;
        private string _connectionString2 { get; set; }
        public SqlService(IConfiguration configuration)
        {
            var enc_key = configuration["Encryption:Key"] ?? "";
            var enc_iv = configuration["Encryption:IV"] ?? "";

            _connectionString = configuration.GetConnectionString(GoAmlConstants.ConnectionString) ?? "";
            _connectionString2 = configuration.GetConnectionString(GoAmlConstants.ConnectionString2) ?? "";
            //_connectionString = EncryptionHelper.Decrypt(_connectionString, enc_key, enc_iv);
        }

        #region common_db_connection

        /** 
        * get reporting person info from sp
        * 
        * @author: Mohiuddin
        * @since: 24/01/2025      
         */
        public async Task<string?> GetReturnedJsonString(dynamic? model, string query)
        {
            try
            {
                string? rs_out = "";

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                string json = JsonSerializer.Serialize(model, options);

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("_json", NpgsqlTypes.NpgsqlDbType.Text, json);
                        var rsOutParam = new NpgsqlParameter("_rs_out", NpgsqlTypes.NpgsqlDbType.Text)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(rsOutParam);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int columnIndex = reader.GetOrdinal("_rs_out");
                                rs_out = reader.IsDBNull(columnIndex) ? null : reader.GetString(columnIndex);
                            }
                        }
                    }
                }
                return rs_out;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public string GetSqlServConnString()
        {
            try
            {                
                return _connectionString2;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        #endregion
    }
}
