using NecCore.models;
using System.Text.Json.Serialization;

namespace SARB_Reporting.Models.Regular
{
    public class PreferenceMapping
    {
        [JsonPropertyName("PreferenceName")]
        public string? PreferenceName { get; set; }

        [JsonPropertyName("PreferenceValue")]
        public string? PreferenceValue { get; set; }
    }
}
