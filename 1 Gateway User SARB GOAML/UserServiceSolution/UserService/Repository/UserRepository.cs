//-----
using System.Data;
using System.Data.Common;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using UserService.Data;
using UserService.Model;
using visa_direct.Interfaces;

namespace UserService.Repository
{
    public class UserRepository
    {
        private readonly AppDbContextPlSql appDbContextPlSql;
        private readonly AppDbContextMsSql appDbContextMsSql;
        private readonly ITransactionService _transactionService;
        
        private readonly string schema = "dbo.";
        private readonly string spGet_all_roles = "Get_all_roles";
        private readonly string spPLSqlCall = "CALL";
        private readonly string spTSqlCall = "EXEC";
        private readonly string paramRsOut = "@rs_out OUTPUT";

        public UserRepository(AppDbContextPlSql appDbContextPlSql, AppDbContextMsSql appDbContextMsSql, ITransactionService transactionService)
        {
            this.appDbContextMsSql = appDbContextMsSql;
            this.appDbContextPlSql = appDbContextPlSql;
            _transactionService = transactionService;
        }
        /* ---------------------------------------------------------------
         *  Tell System.Text.Json to use EXACT property names.
         *  That way  "id_user_key"  in the JSON maps to  IdUserKey  in
         *  the C# model because we added  [JsonPropertyName("id_user_key")]
         *  on the property.
         * ------------------------------------------------------------- */
        private static readonly JsonSerializerOptions _opt = new()
        {
            PropertyNameCaseInsensitive = false
        };

        /* ---------- common helper: actually CALL the sel_user procedure ---------- */
        public async Task<string?> CallGetAllRoles(DbContext context, string jsonInput)
        {
            try {
                // first identify database from connect name and set output param
                // to create a SP calling statement pick separatly. for sql server 'EXEC' and Postgresql 'CALL'
                DbParameter inputParam = context.Database.GetDbConnection().GetType().Name.Contains("Npgsql")
                    ? new Npgsql.NpgsqlParameter("_json", NpgsqlTypes.NpgsqlDbType.Text) { Value = "'{\"actionName\": \"GET_ALL_ROLES\"}'" }
                    : new SqlParameter("@_json", SqlDbType.NVarChar, -1);

                DbParameter outputParam = context.Database.GetDbConnection().GetType().Name.Contains("Npgsql")
                    ? new Npgsql.NpgsqlParameter("rs_out", NpgsqlTypes.NpgsqlDbType.Text) { Direction = ParameterDirection.Output }
                    : new SqlParameter("@rs_out", SqlDbType.NVarChar,-1) { Direction = ParameterDirection.Output };

                string sp = context.Database.GetDbConnection().GetType().Name.Contains("Npgsql")
                    ? "CALL public.sel_user(null, @_json);"
                    : spTSqlCall + " " + schema + spGet_all_roles + " " + paramRsOut;

                //context.Database
                //    .ExecuteSqlRaw("EXEC dbo.get_all_roles @rs_out OUTPUT", outputParam);
                context.Database
                    .ExecuteSqlRaw(sp, outputParam, inputParam);
                return outputParam.Value.ToString();

            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

            return null;
        }


        private async Task<string?> CallSelUserAsync(object payload)
        {
            var jsonPayload = JsonSerializer.Serialize(payload);

            await using var conn = (NpgsqlConnection)appDbContextPlSql.Database.GetDbConnection();
            var needClose = false;
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync();
                needClose = true;
            }

            // sel_user expects TWO parameters:
            //    1) dummy NULL (OUT parameter placeholder)
            //    2) the JSON text that contains actionName etc.
            await using var cmd = new NpgsqlCommand(
                "CALL public.sel_user(NULL::text, @p_json::text);", conn);

            cmd.Parameters.AddWithValue("p_json",
                NpgsqlTypes.NpgsqlDbType.Text, jsonPayload);

            await using var reader = await cmd.ExecuteReaderAsync();

            string? jsonResult = null;
            if (await reader.ReadAsync())
                jsonResult = reader.IsDBNull(0) ? null : reader.GetString(0);

            if (needClose) await conn.CloseAsync();
            return jsonResult;
        }

        /* ---------- common helper: actually CALL the act_user procedure ---------- */
        private async Task<string?> CallActUserAsync(object payload)
        {
            var jsonPayload = JsonSerializer.Serialize(payload, _opt);

            await using var conn = (NpgsqlConnection)appDbContextPlSql.Database.GetDbConnection();
            var needClose = false;
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync();
                needClose = true;
            }

            // act_user expects TWO parameters:
            //    1) dummy NULL (OUT parameter placeholder)
            //    2) the JSON text that contains actionName etc.
            await using var cmd = new NpgsqlCommand(
                "CALL public.act_user(NULL::text, @p_json::text);", conn);

            cmd.Parameters.AddWithValue("p_json",
                NpgsqlTypes.NpgsqlDbType.Text, jsonPayload);

            await using var reader = await cmd.ExecuteReaderAsync();

            string? jsonResult = null;
            if (await reader.ReadAsync())
                jsonResult = reader.IsDBNull(0) ? null : reader.GetString(0);

            if (needClose) await conn.CloseAsync();
            return jsonResult;
        }

        /* ----------------------- public API ----------------------- */

        public async Task<List<UserDto>> GetAllAsync()
        {
            var json = await CallSelUserAsync(new { actionName = "GET_ALL_USER" });
            return json is null
                ? new()
                : (JsonSerializer.Deserialize<List<UserDto>>(json, _opt) ?? new());
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            //var json = await CallSelUserAsync(new
            //{
            //    actionName = "GET_USER_BY_ID",
            //    userId = id
            //});


           var json =  _transactionService.Process(JsonSerializer.Serialize(new
            {
                actionName = "GET_USER_BY_ID",
                userId = id
            }), "sel_user", "Postgres");

            return json is null
                ? null
                : JsonSerializer.Deserialize<UserDto>(json, _opt);
        }

        public async Task<UserDto?> SignInAsync(string login, string password)
        {
            var json = await CallSelUserAsync(new
            {
                actionName = "SIGN_IN",
                loginName = login,
                password
            });

            return json is null
                ? null
                : JsonSerializer.Deserialize<UserDto?>(json, _opt);
        }

        public async Task<AddUserResultDto> AddUserAsync(AddUserRequestDto request, int userModifiedId)
        {
            try
            {
                var payload = new
                {
                    actionName = "ADD_USER",
                    loginName = request.LoginName,
                    firstName = request.FirstName,
                    lastName = request.LastName,
                    isAllowLogin = request.IsAllowLogin,
                    isActive = request.IsActive,
                    phone = request.Phone,
                    email = request.Email,
                    password = request.Password,
                    userModifiedId = userModifiedId,
                    userInfo = request.UserInfo,
                    groups = request.Groups.Select(g => new
                    {
                        groupId = g.GroupId,
                        groupName = g.GroupName
                    }).ToArray()
                };

                var json = await CallActUserAsync(payload); //  Use helper

                return new AddUserResultDto
                {
                    Success = true,
                    Message = $"User created successfully by user ID: {userModifiedId} with {request.Groups?.Count ?? 0} groups assigned",
                    UserId = 0
                };
            }
            catch (Npgsql.PostgresException pgEx)
            {
                return new AddUserResultDto
                {
                    Success = false,
                    Message = pgEx.MessageText ?? pgEx.Message,
                    UserId = 0
                };
            }
            catch (Exception ex)
            {
                return new AddUserResultDto
                {
                    Success = false,
                    Message = ex.Message,
                    UserId = 0
                };
            }
        }


        public async Task<UpdateUserResultDto> UpdateUserAsync(UpdateUserRequestDto request, int userModifiedId)
        {
            try
            {
                var payload = new
                {
                    actionName = "UPDATE",
                    userId = request.UserId,
                    firstName = request.FirstName,
                    lastName = request.LastName,
                    isAllowLogin = request.IsAllowLogin,
                    isActive = request.IsActive,
                    phone = request.Phone,
                    email = request.Email,
                    password = request.Password,
                    userModifiedId = userModifiedId,
                    isDisabled = request.IsDisabled,
                    userInfo = request.UserInfo
                };

                var json = await CallActUserAsync(payload); // ✅ Use helper

                return new UpdateUserResultDto
                {
                    Success = true,
                    Message = $"User updated successfully by user ID: {userModifiedId} at {DateTime.Now.ToString()}",
                    UserId = request.UserId
                };
            }
            catch (Npgsql.PostgresException pgEx)
            {
                return new UpdateUserResultDto
                {
                    Success = false,
                    Message = pgEx.MessageText ?? pgEx.Message,
                    UserId = request.UserId
                };
            }
            catch (Exception ex)
            {
                return new UpdateUserResultDto
                {
                    Success = false,
                    Message = ex.Message,
                    UserId = request.UserId
                };
            }
        }

        public async Task<ChangePasswordResultDto> ChangePasswordAsync(ChangePasswordRequestDto request, int userModifiedId)
        {
            try
            {
                await using var conn = (NpgsqlConnection)appDbContextPlSql.Database.GetDbConnection();

                if (conn.State != ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }

                // Step 1: Validate old password with a single query
                await using var validateCmd = new NpgsqlCommand(
                    "SELECT COUNT(*) FROM T_USER WHERE id_user_key = @userId AND tx_password = crypt(@oldPassword, tx_password);",
                    conn);

                validateCmd.Parameters.AddWithValue("userId", request.UserId);
                validateCmd.Parameters.AddWithValue("oldPassword", request.OldPassword);

                var validPasswordCount = (long)(await validateCmd.ExecuteScalarAsync() ?? 0);

                if (validPasswordCount == 0)
                {
                    return new ChangePasswordResultDto
                    {
                        Success = false,
                        Message = "Current password is incorrect or user not found",
                        UserId = request.UserId
                    };
                }

                // Step 2: Old password is valid, now change the password
                await using var changeCmd = new NpgsqlCommand(
                    "CALL public.act_user(NULL::text, @p_json::text);", conn);

                var changePayload = new
                {
                    actionName = "UPD_PWD",
                    userId = request.UserId,
                    password = request.Password,
                    oldPassword = request.OldPassword,
                    userModifiedId = userModifiedId
                };

                var jsonPayload = JsonSerializer.Serialize(changePayload);

                Console.WriteLine("=== CHANGE PASSWORD (VALIDATED) ===");
                Console.WriteLine($"User: {request.UserId}, Changed by: {userModifiedId}");
                Console.WriteLine("Old password validation: SUCCESS");
                Console.WriteLine("=== END ===");

                changeCmd.Parameters.AddWithValue("p_json", NpgsqlTypes.NpgsqlDbType.Text, jsonPayload);
                await changeCmd.ExecuteNonQueryAsync();

                return new ChangePasswordResultDto
                {
                    Success = true,
                    Message = $"Password changed successfully by user ID: {userModifiedId}",
                    UserId = request.UserId
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== CHANGE PASSWORD ERROR ===");
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"=== END ERROR ===");

                return new ChangePasswordResultDto
                {
                    Success = false,
                    Message = ex.Message,
                    UserId = request.UserId
                };
            }
        }
    }
}
