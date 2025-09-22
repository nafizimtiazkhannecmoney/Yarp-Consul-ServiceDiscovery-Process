using System.Xml.Serialization;

namespace SARB_Reporting.Models
{
    /// <summary>
    /// Represents the standard SARB response with attributes and message content.
    /// </summary>
    [XmlRoot("SARBDexResponse")]
    public class SARBDexResponse
    {
        [XmlAttribute("statusDescription")]
        public string StatusDescription { get; set; }

        [XmlAttribute("statusCode")]
        public string StatusCode { get; set; }

        [XmlAttribute("reference")]
        public string Reference { get; set; }

        [XmlAttribute("SARBreference")]
        public string SARBReference { get; set; }

        [XmlText]
        public string Message { get; set; }
    }

    /// <summary>
    /// Represents a simple error message without attributes.
    /// </summary>
    [XmlRoot("Errors")]
    public class SimpleErrorResponse
    {
        [XmlText]
        public string Message { get; set; }
    }

    /// <summary>
    /// Represents the structured SARB error response.
    /// </summary>
    [XmlRoot("Errors")]
    public class ErrorsResponse
    {
        [XmlAttribute("reference")]
        public string Reference { get; set; }

        [XmlElement("Error")]
        public List<ErrorDetail> ErrorDetails { get; set; }
    }

    /// <summary>
    /// Represents individual error details inside an Errors response.
    /// </summary>
    public class ErrorDetail
    {
        [XmlAttribute("code")]
        public string Code { get; set; }

        [XmlAttribute("description")]
        public string Description { get; set; }

        [XmlAttribute("level")]
        public string Level { get; set; }

        [XmlAttribute("severity")]
        public string Severity { get; set; }
    }
}
