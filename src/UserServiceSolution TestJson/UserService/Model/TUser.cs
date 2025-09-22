using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UserService.Model
{
    [Table("t_user")]                                       // table name
    public class TUser
    {
        [Key]
        [Column("id_user_key")]
        [JsonPropertyName("id_user_key")]
        public int IdUserKey { get; set; }

        [Column("dtt_mod")]
        [JsonPropertyName("dtt_mod")]
        public DateTime? Modified { get; set; }

        [Column("tx_login_name")]
        [JsonPropertyName("tx_login_name")]
        public string LoginName { get; set; } = default!;

        [Column("tx_first_name")]
        [JsonPropertyName("tx_first_name")]
        public string? FirstName { get; set; }

        [Column("tx_last_name")]
        [JsonPropertyName("tx_last_name")]
        public string? LastName { get; set; }

        [Column("tx_email")]
        [JsonPropertyName("tx_email")]
        public string? Email { get; set; }

        [Column("j_user_info")]
        [JsonPropertyName("j_user_info")]
        public string? UserInfoRaw { get; set; }
    }
}
