using System.Text.Json.Serialization;

namespace UserService.Model
{
    public class ChangePasswordRequestDto
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; } = default!; // New password

        [JsonPropertyName("oldPassword")]
        public string OldPassword { get; set; } = default!; // Current password for validation

        // UserModifiedId removed - automatically set from logged-in user's JWT "uid" claim
    }

    public class ChangePasswordResultDto
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = default!;

        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }
}
