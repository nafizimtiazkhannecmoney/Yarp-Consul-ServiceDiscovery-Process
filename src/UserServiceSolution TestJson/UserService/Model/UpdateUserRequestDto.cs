using System.Text.Json.Serialization;

namespace UserService.Model
{
    public class UpdateUserRequestDto
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("isAllowLogin")]
        public string? IsAllowLogin { get; set; }

        [JsonPropertyName("isActive")]
        public string? IsActive { get; set; }

        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("password")]
        public string? Password { get; set; } // Optional - only update if provided

        //[JsonPropertyName("userModifiedId")]
        //public int UserModifiedId { get; set; }

        [JsonPropertyName("isDisabled")]
        public string? IsDisabled { get; set; }

        [JsonPropertyName("userInfo")]
        public UpdateUserInfoDto? UserInfo { get; set; }
    }

    public class UpdateUserInfoDto
    {
        [JsonPropertyName("dob")]
        public string? Dob { get; set; }

        [JsonPropertyName("idType")]
        public string? IdType { get; set; }

        [JsonPropertyName("idNumber")]
        public string? IdNumber { get; set; }

        [JsonPropertyName("gender")]
        public string? Gender { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("countryCode")]
        public string? CountryCode { get; set; }

        [JsonPropertyName("nationality")]
        public string? Nationality { get; set; }

        [JsonPropertyName("occupation")]
        public string? Occupation { get; set; }

        [JsonPropertyName("position")]
        public string? Position { get; set; }

        [JsonPropertyName("department")]
        public string? Department { get; set; }

        [JsonPropertyName("address1")]
        public string? Address1 { get; set; }

        [JsonPropertyName("address2")]
        public string? Address2 { get; set; }

        [JsonPropertyName("companyName")]
        public string? CompanyName { get; set; }

        [JsonPropertyName("branchCode")]
        public string? BranchCode { get; set; }

        [JsonPropertyName("branchName")]
        public string? BranchName { get; set; }

        [JsonPropertyName("bloodGroup")]
        public string? BloodGroup { get; set; }

        [JsonPropertyName("town")]
        public string? Town { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("zip")]
        public string? Zip { get; set; }
    }

    public class UpdateUserResultDto
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = default!;

        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }
}
