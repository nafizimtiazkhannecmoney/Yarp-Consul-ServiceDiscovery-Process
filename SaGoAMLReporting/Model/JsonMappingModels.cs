using System.Text.Json.Serialization;

namespace SaGoAMLReporting.Model
{
    public class JsonReportingPersonArrayModel
    {
        [JsonPropertyName("jsn_reporting_person")]
        public ReportingPerson? ReportingPerson { get; set; }
    }    
}
