using Npgsql;
using UserService.Data;
using UserService.Model;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.Data;

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

        /* ---------- common helper: actually CALL the procedure ---------- */
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
                //: JsonSerializer.Deserialize<List<UserDto>>(json, _opt)?.SingleOrDefault();
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
            //var model = JsonSerializer.Deserialize<UserDto?>(json, _opt);

            return json is null
                ? null
                //: JsonSerializer.Deserialize<TUser>(json, _opt);
                : JsonSerializer.Deserialize<UserDto?>(json, _opt);
        }
    }
}
