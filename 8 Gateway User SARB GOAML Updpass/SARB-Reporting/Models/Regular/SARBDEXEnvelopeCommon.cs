using System.Xml.Serialization;

namespace SARB_Reporting.Models.Regular
{
    [XmlRoot("SARBDEXEnvelope", Namespace = "x-schema:http://sarbdex.resbank.co.za/schemas/sarbdex_schema.xml")]
    public class SARBDEXEnvelopeCommon<T>
    {
        public SARBDEXEnvelopeCommon()
        {
            this.Header = new SARBDEXHeader();
            this.Body = new SARBDEXBody<T>();
            this.XmlNamespacesForSarb = new XmlSerializerNamespaces();
            XmlNamespacesForSarb.Add("sarbdex", "x-schema:http://sarbdex.resbank.co.za/schemas/sarbdex_schema.xml");
        }

        [XmlElement("SARBDEXHeader", Namespace = "x-schema:http://sarbdex.resbank.co.za/schemas/sarbdex_schema.xml")]
        public SARBDEXHeader? Header { get; set; }

        [XmlElement("SARBDEXBody", Namespace = "x-schema:http://sarbdex.resbank.co.za/schemas/sarbdex_schema.xml")]
        public SARBDEXBody<T>? Body { get; set; }

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces XmlNamespacesForSarb { get; set; }
    }

    public class SARBDEXHeader
    {
        [XmlAttribute]
        public string Sender { get; set; } = string.Empty;

        [XmlAttribute]
        public string Recipient { get; set; } = string.Empty;

        [XmlAttribute]
        public string Identity { get; set; } = string.Empty;

        [XmlAttribute]
        public int IdentityType { get; set; }

        [XmlAttribute]
        public DateTimeOffset SentAt { get; set; }

        [XmlAttribute]
        public string Type { get; set; } = string.Empty;

        [XmlAttribute]
        public int Version { get; set; }

        [XmlAttribute]
        public string Reference { get; set; } = string.Empty;
    }

    [XmlRoot("FINSURV", Namespace = "", IsNullable = false)]
    public class SARBDEXBody<T>
    {
        [XmlElement("FINSURV")]
        public T? Content { get; set; }
    }
}
