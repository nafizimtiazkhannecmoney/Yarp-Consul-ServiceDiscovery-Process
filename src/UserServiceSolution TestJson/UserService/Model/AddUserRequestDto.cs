using System.Text.Json.Serialization;
namespace UserService.Model
{
    public class AddUserRequestDto
    {
        [JsonPropertyName("loginName")]
        public string LoginName { get; set; } = default!;

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = default!;

        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = default!;

        [JsonPropertyName("isAllowLogin")]
        public string IsAllowLogin { get; set; } = "1";

        [JsonPropertyName("isActive")]
        public string IsActive { get; set; } = "1";

        [JsonPropertyName("phone")]
        public string Phone { get; set; } = default!;

        [JsonPropertyName("email")]
        public string Email { get; set; } = default!;

        [JsonPropertyName("password")]
        public string Password { get; set; } = default!;

        //[JsonPropertyName("userModifiedId")]
        //public int UserModifiedId { get; set; }

        [JsonPropertyName("userInfo")]
        public AddUserInfoDto UserInfo { get; set; } = new();

        [JsonPropertyName("groups")]
        public List<AddUserGroupDto> Groups { get; set; } = new();
    }

    public class AddUserInfoDto
    {
        [JsonPropertyName("dob")]
        public string Dob { get; set; } = default!;

        [JsonPropertyName("idType")]
        public string IdType { get; set; } = default!;

        [JsonPropertyName("idNumber")]
        public string IdNumber { get; set; } = default!;

        [JsonPropertyName("gender")]
        public string Gender { get; set; } = default!;

        [JsonPropertyName("country")]
        public string Country { get; set; } = default!;

        [JsonPropertyName("countryCode")]
        public string CountryCode { get; set; } = default!;

        [JsonPropertyName("nationality")]
        public string Nationality { get; set; } = default!;

        [JsonPropertyName("occupation")]
        public string Occupation { get; set; } = default!;

        [JsonPropertyName("position")]
        public string Position { get; set; } = default!;

        [JsonPropertyName("department")]
        public string Department { get; set; } = default!;

        [JsonPropertyName("address1")]
        public string Address1 { get; set; } = default!;

        [JsonPropertyName("address2")]
        public string Address2 { get; set; } = default!;

        [JsonPropertyName("companyName")]
        public string CompanyName { get; set; } = default!;

        [JsonPropertyName("branchCode")]
        public string BranchCode { get; set; } = default!;

        [JsonPropertyName("branchName")]
        public string BranchName { get; set; } = default!;

        [JsonPropertyName("bloodGroup")]
        public string BloodGroup { get; set; } = default!;

        [JsonPropertyName("town")]
        public string Town { get; set; } = default!;

        [JsonPropertyName("city")]
        public string City { get; set; } = default!;

        [JsonPropertyName("state")]
        public string State { get; set; } = default!;

        [JsonPropertyName("zip")]
        public string Zip { get; set; } = default!;
    }

    public class AddUserGroupDto
    {
        [JsonPropertyName("groupId")]
        public int GroupId { get; set; }

        [JsonPropertyName("groupName")]
        public string GroupName { get; set; } = default!;
    }

    public class AddUserResultDto
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = default!;

        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }
}
