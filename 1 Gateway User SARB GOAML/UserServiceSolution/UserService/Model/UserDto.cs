using System.Text.Json.Serialization;

namespace UserService.Model
{
    public class UserDto
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; } = default!;

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = default!;

        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = default!;

        [JsonPropertyName("loginName")]
        public string LoginName { get; set; } = default!;

        [JsonPropertyName("userInfo")]
        public UserInfoDto UserInfo { get; set; } = new();

        [JsonPropertyName("group")]
        public List<GroupDto> Group { get; set; } = new();
    }

    // --------------------------------------------------
    // Nested objects
    // --------------------------------------------------

    public class UserInfoDto
    {
        [JsonPropertyName("dob")]
        public DateTime Dob { get; set; }

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

    public class GroupDto
    {
        [JsonPropertyName("groupId")]
        public int GroupId { get; set; }

        [JsonPropertyName("groupName")]
        public string GroupName { get; set; } = default!;

        [JsonPropertyName("role")]
        public List<RoleDto> Role { get; set; } = new();
    }

    public class RoleDto
    {
        [JsonPropertyName("roleId")]
        public int RoleId { get; set; }

        [JsonPropertyName("roleName")]
        public string RoleName { get; set; } = default!;

        [JsonPropertyName("permission")]
        public List<PermissionDto> Permission { get; set; } = new();
    }

    public class PermissionDto
    {
        [JsonPropertyName("permissionId")]
        public int PermissionId { get; set; }

        [JsonPropertyName("permissionType")]
        public string PermissionType { get; set; } = default!;

        [JsonPropertyName("permissionName")]
        public string PermissionName { get; set; } = default!;
    }
}
