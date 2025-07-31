//-----
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using UserService.Data;
using UserService.Model;

namespace UserService.Repository
{
    public class UserRepository
    {
        private readonly AppDbContext _db;
        public UserRepository(AppDbContext db) => _db = db;

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
        private async Task<string?> CallSelUserAsync(object payload)
        {
            var jsonPayload = JsonSerializer.Serialize(payload);

            await using var conn = (NpgsqlConnection)_db.Database.GetDbConnection();
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

            await using var conn = (NpgsqlConnection)_db.Database.GetDbConnection();
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
            var json = await CallSelUserAsync(new
            {
                actionName = "GET_USER_BY_ID",
                userId = id
            });

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

        // Add User With Actual id_mod_key
        public async Task<AddUserResultDto> AddUserAsync(AddUserRequestDto request, int userModifiedId)
        {
            try
            {
                // Ensure numeric values are sent as actual numbers, not strings
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
                    userModifiedId = userModifiedId, // Automatically from logged-in user's JWT "uid" claim
                    userInfo = request.UserInfo,
                    groups = request.Groups.Select(g => new
                    {
                        groupId = g.GroupId, // Keep as number, not string
                        groupName = g.GroupName
                    }).ToArray()
                };

                // Use JsonSerializerOptions to ensure numbers are serialized as numbers
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = null,
                    WriteIndented = false
                };

                var jsonPayload = JsonSerializer.Serialize(payload, options);

                Console.WriteLine("=== FIXED NUMERIC JSON PAYLOAD ===");
                Console.WriteLine(jsonPayload);
                Console.WriteLine($"Created by user ID: {userModifiedId} (from JWT 'uid' claim)");
                Console.WriteLine("=== END PAYLOAD ===");

                await using var conn = (NpgsqlConnection)_db.Database.GetDbConnection();
                var needClose = false;

                if (conn.State != ConnectionState.Open)
                {
                    await conn.OpenAsync();
                    needClose = true;
                }

                await using var cmd = new NpgsqlCommand(
                    "CALL public.act_user(NULL::text, @p_json::text);", conn);

                cmd.Parameters.AddWithValue("p_json", NpgsqlTypes.NpgsqlDbType.Text, jsonPayload);

                await cmd.ExecuteNonQueryAsync();

                if (needClose) await conn.CloseAsync();

                return new AddUserResultDto
                {
                    Success = true,
                    Message = $"User created successfully by user ID: {userModifiedId} with {request.Groups?.Count ?? 0} groups assigned",
                    UserId = 0
                };
            }
            catch (Npgsql.PostgresException pgEx)
            {
                Console.WriteLine($"=== POSTGRES EXCEPTION ===");
                Console.WriteLine($"SqlState: {pgEx.SqlState}");
                Console.WriteLine($"Message: {pgEx.MessageText}");
                Console.WriteLine($"Detail: {pgEx.Detail}");
                Console.WriteLine($"=== END POSTGRES EXCEPTION ===");

                return new AddUserResultDto
                {
                    Success = false,
                    Message = pgEx.MessageText ?? pgEx.Message,
                    UserId = 0
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== GENERAL EXCEPTION ===");
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"=== END ERROR ===");

                return new AddUserResultDto
                {
                    Success = false,
                    Message = $"User Added successfully by user ID: {userModifiedId}",
                    UserId = 0
                };
            }
        }

        // Replace your existing UpdateUserAsync method with this
        public async Task<UpdateUserResultDto> UpdateUserAsync(UpdateUserRequestDto request, int userModifiedId)
        {
            try
            {
                // Create payload for UPDATE action with automatic userModifiedId from JWT
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
                    password = request.Password, // Optional - only update if provided
                    userModifiedId = userModifiedId, // Automatically from logged-in user's JWT "uid" claim
                    isDisabled = request.IsDisabled,
                    userInfo = request.UserInfo
                    // Note: No groups - groups are not updated via UPDATE action
                };

                // Use JsonSerializerOptions to ensure numbers are serialized as numbers
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = null,
                    WriteIndented = false
                };

                var jsonPayload = JsonSerializer.Serialize(payload, options);

                Console.WriteLine("=== UPDATE USER JSON PAYLOAD (AUTO USER MOD ID FROM JWT) ===");
                Console.WriteLine($"User being updated: {request.UserId}");
                Console.WriteLine($"Modified by user ID: {userModifiedId} (from JWT 'uid' claim)");
                Console.WriteLine(jsonPayload);
                Console.WriteLine("=== END PAYLOAD ===");

                await using var conn = (NpgsqlConnection)_db.Database.GetDbConnection();
                var needClose = false;

                if (conn.State != ConnectionState.Open)
                {
                    await conn.OpenAsync();
                    needClose = true;
                }

                await using var cmd = new NpgsqlCommand(
                    "CALL public.act_user(NULL::text, @p_json::text);", conn);

                cmd.Parameters.AddWithValue("p_json", NpgsqlTypes.NpgsqlDbType.Text, jsonPayload);

                await cmd.ExecuteNonQueryAsync();

                if (needClose) await conn.CloseAsync();

                return new UpdateUserResultDto
                {
                    Success = true,
                    Message = $"User updated successfully by user ID: {userModifiedId}",
                    UserId = request.UserId
                };
            }
            catch (Npgsql.PostgresException pgEx)
            {
                Console.WriteLine($"=== POSTGRES EXCEPTION (UPDATE USER) ===");
                Console.WriteLine($"SqlState: {pgEx.SqlState}");
                Console.WriteLine($"Message: {pgEx.MessageText}");
                Console.WriteLine($"Detail: {pgEx.Detail}");
                Console.WriteLine($"=== END POSTGRES EXCEPTION ===");

                return new UpdateUserResultDto
                {
                    Success = false,
                    Message = pgEx.MessageText ?? pgEx.Message,
                    UserId = request.UserId
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== GENERAL EXCEPTION (UPDATE USER) ===");
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"=== END ERROR ===");

                return new UpdateUserResultDto
                {
                    Success = false,
                    Message = ex.Message,
                    UserId = request.UserId
                };
            }
        }




        //// Fixed version - single database call to avoid connection disposal issues
        //public async Task<ChangePasswordResultDto> ChangePasswordAsync(ChangePasswordRequestDto request, int userModifiedId)
        //{
        //    try
        //    {
        //        // Direct call to stored procedure - let the database handle validation
        //        var payload = new
        //        {
        //            actionName = "UPD_PWD",
        //            userId = request.UserId,
        //            password = request.Password, // New password
        //            oldPassword = request.OldPassword, // Old password for reference
        //            userModifiedId = userModifiedId // Automatically from logged-in user's JWT "uid" claim
        //        };

        //        var options = new JsonSerializerOptions
        //        {
        //            PropertyNamingPolicy = null,
        //            WriteIndented = false
        //        };

        //        var jsonPayload = JsonSerializer.Serialize(payload, options);

        //        Console.WriteLine("=== CHANGE PASSWORD JSON PAYLOAD (FIXED) ===");
        //        Console.WriteLine($"User changing password: {request.UserId}");
        //        Console.WriteLine($"Password changed by user ID: {userModifiedId} (from JWT 'uid' claim)");
        //        Console.WriteLine(jsonPayload.Replace(request.Password, "***NEW_PASSWORD***").Replace(request.OldPassword, "***OLD_PASSWORD***")); // Hide passwords in logs
        //        Console.WriteLine("=== END PAYLOAD ===");

        //        await using var conn = (NpgsqlConnection)_db.Database.GetDbConnection();
        //        var needClose = false;

        //        if (conn.State != ConnectionState.Open)
        //        {
        //            await conn.OpenAsync();
        //            needClose = true;
        //        }

        //        await using var cmd = new NpgsqlCommand(
        //            "CALL public.act_user(NULL::text, @p_json::text);", conn);

        //        cmd.Parameters.AddWithValue("p_json", NpgsqlTypes.NpgsqlDbType.Text, jsonPayload);

        //        await cmd.ExecuteNonQueryAsync();

        //        if (needClose) await conn.CloseAsync();

        //        return new ChangePasswordResultDto
        //        {
        //            Success = true,
        //            Message = $"Password changed successfully by user ID: {userModifiedId}",
        //            UserId = request.UserId
        //        };
        //    }
        //    catch (Npgsql.PostgresException pgEx)
        //    {
        //        Console.WriteLine($"=== POSTGRES EXCEPTION (CHANGE PASSWORD) ===");
        //        Console.WriteLine($"SqlState: {pgEx.SqlState}");
        //        Console.WriteLine($"Message: {pgEx.MessageText}");
        //        Console.WriteLine($"Detail: {pgEx.Detail}");
        //        Console.WriteLine($"=== END POSTGRES EXCEPTION ===");

        //        return new ChangePasswordResultDto
        //        {
        //            Success = false,
        //            Message = pgEx.MessageText ?? pgEx.Message,
        //            UserId = request.UserId
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"=== GENERAL EXCEPTION (CHANGE PASSWORD) ===");
        //        Console.WriteLine($"Error: {ex.Message}");
        //        Console.WriteLine($"=== END ERROR ===");

        //        return new ChangePasswordResultDto
        //        {
        //            Success = false,
        //            Message = ex.Message,
        //            UserId = request.UserId
        //        };
        //    }
        //}


        // Simplest approach - single query validation
        public async Task<ChangePasswordResultDto> ChangePasswordAsync(ChangePasswordRequestDto request, int userModifiedId)
        {
            try
            {
                await using var conn = (NpgsqlConnection)_db.Database.GetDbConnection();

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
