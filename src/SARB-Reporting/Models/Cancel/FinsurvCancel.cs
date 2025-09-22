using System.Xml.Serialization;

namespace SARB_Reporting.Models.Cancel
{
    [XmlRoot("FINSURV", Namespace = "")]
    public class FinsurvCancel
    {
        public FinsurvCancel()
        {
            CancelledTransaction = new CancelledTransaction();
            XmlNamespaces = new XmlSerializerNamespaces();
            XmlNamespaces.Add("finsurv", "www.finsurv.com");
            XmlNamespaces.Add("noNamespaceSchemaLocation", "http://www.w3.org/2001/XMLSchema-instance");
            XmlNamespaces.Add("wmh", "http://www.wmhelp.com/2003/eGenerator");
            XmlNamespaces.Add("xs", "http://www.w3.org/2001/XMLSchema");
            XmlNamespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
        }

        [XmlAttribute("Reference")]
        public string? Reference { get; set; }

        [XmlAttribute("Environment")]
        public string? Environment { get; set; }

        [XmlAttribute("Version")]
        public string? Version { get; set; }

        [XmlElement("CancelledTransaction")]
        public CancelledTransaction? CancelledTransaction { get; set; }

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces XmlNamespaces { get; set; }
    }

    public class CancelledTransaction
    {
        [XmlAttribute("LineNumber")]
        public int LineNumber { get; set; }

        [XmlAttribute("SequenceNumber")]
        public int SequenceNumber { get; set; }

        [XmlAttribute("Flow")]
        public string? Flow { get; set; }

        [XmlAttribute("TrnReference")]
        public string? TrnReference { get; set; }
    }
}
