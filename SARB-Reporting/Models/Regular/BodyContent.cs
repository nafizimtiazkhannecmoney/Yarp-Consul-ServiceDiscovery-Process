using System.Xml.Serialization;

namespace SARB_Reporting.Models.Regular
{
    [XmlRoot("FINSURV", Namespace = "", IsNullable = false)]
    public class BodyContent
    {
        public BodyContent()
        {
            this.OriginalTransactions = new List<OriginalTransaction>();
            this.XmlNamespaces = new XmlSerializerNamespaces();
            XmlNamespaces.Add("finsurv", "www.finsurv.com");
            XmlNamespaces.Add("noNamespaceSchemaLocation", "http://www.w3.org/2001/XMLSchema-instance");
            XmlNamespaces.Add("wmh", "http://www.wmhelp.com/2003/eGenerator");
            XmlNamespaces.Add("xs", "http://www.w3.org/2001/XMLSchema");
            XmlNamespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
        }
        [XmlAttribute]
        public string Reference { get; set; } = string.Empty;

        [XmlAttribute]
        public string Environment { get; set; } = string.Empty;

        [XmlAttribute]
        public int Version { get; set; }

        [XmlElement("OriginalTransaction")]
        public List<OriginalTransaction>? OriginalTransactions { get; set; }

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces XmlNamespaces { get; set; }
    }

    public class OriginalTransaction
    {
        [XmlAttribute]
        public int LineNumber { get; set; }

        [XmlAttribute]
        public string ReportingQualifier { get; set; } = string.Empty;

        [XmlAttribute]
        public string Flow { get; set; } = string.Empty;

        [XmlAttribute]
        public string ReplacementTransaction { get; set; } = string.Empty;

        [XmlAttribute(DataType = "date")]
        public DateTime Date { get; set; }

        [XmlAttribute]
        public string TrnReference { get; set; } = string.Empty;

        [XmlAttribute]
        public string BranchCode { get; set; } = string.Empty;

        [XmlAttribute]
        public string BranchName { get; set; } = string.Empty;

        [XmlAttribute]
        public string OriginatingBank { get; set; } = string.Empty;

        [XmlAttribute]
        public string OriginatingCountry { get; set; } = string.Empty;

        [XmlAttribute]
        public double TotalValue { get; set; }

        [XmlAttribute]
        public string ReceivingBank { get; set; } = string.Empty;

        [XmlAttribute]
        public string ReceivingCountry { get; set; } = string.Empty;

        [XmlElement("NonResident")]
        public NonResident? NonResident { get; set; }

        [XmlElement("ResidentCustomerAccountHolder")]
        public ResidentCustomerAccountHolder? ResidentCustomerAccountHolder { get; set; }

        [XmlElement("MonetaryDetails")]
        public MonetaryDetails? MonetaryDetails { get; set; }
    }
    public class MonetaryDetails
    {
        [XmlAttribute("SequenceNumber")]
        public string? SequenceNumber { get; set; }

        [XmlAttribute("MoneyTransferAgentIndicator")]
        public string? MoneyTransferAgentIndicator { get; set; }

        [XmlAttribute("RandValue")]
        public string? RandValue { get; set; }

        [XmlAttribute("ForeignValue")]
        public string? ForeignValue { get; set; }

        [XmlAttribute("ForeignCurrencyCode")]
        public string? ForeignCurrencyCode { get; set; }

        [XmlAttribute("BoPCategory")]
        public string? BoPCategory { get; set; }

        [XmlAttribute("RulingsSection")]
        public string? RulingsSection { get; set; }

        [XmlAttribute("LocationCountry")]
        public string? LocationCountry { get; set; }

        [XmlAttribute("ThirdPartyContactSurname")]
        public string? ThirdPartyContactSurname { get; set; }

        [XmlAttribute("ThirdPartyContactName")]
        public string? ThirdPartyContactName { get; set; }

        [XmlAttribute("ThirdPartyTelephone")]
        public string? ThirdPartyTelephone { get; set; }

        [XmlAttribute("LegalEntityThirdPartyName")]
        public string? LegalEntityThirdPartyName { get; set; }

        public void SetThirdPartyDetails()
        {
            if (MoneyTransferAgentIndicator == "ADLA|S2S")
            {
                ThirdPartyContactSurname = this.ThirdPartyContactSurname;
                ThirdPartyContactName = this.ThirdPartyContactName;
                ThirdPartyTelephone = this.ThirdPartyTelephone;
                LegalEntityThirdPartyName = this.LegalEntityThirdPartyName;
                MoneyTransferAgentIndicator = "ADLA";
            }
            else
            {
                ThirdPartyContactSurname = null;
                ThirdPartyContactName = null;
                ThirdPartyTelephone = null;
                LegalEntityThirdPartyName = null;
                MoneyTransferAgentIndicator = this.MoneyTransferAgentIndicator;
            }
        }
    }

    public class NonResident
    {
        [XmlElement("Individual")]
        public Individual? Individual { get; set; }
    }

    public class Individual
    {
        [XmlAttribute("Surname")]
        public string? Surname { get; set; }

        [XmlAttribute("Name")]
        public string? Name { get; set; }

        [XmlElement("AdditionalNonResidentData")]
        public AdditionalNonResidentData? AdditionalNonResidentData { get; set; }
    }

    public class AdditionalNonResidentData
    {
        [XmlAttribute("AccountIdentifier")]
        public string? AccountIdentifier { get; set; }

        [XmlAttribute("Country")]
        public string? Country { get; set; }

        [XmlAttribute("AccountNumber")]
        public string? AccountNumber { get; set; }
    }

    public class ResidentCustomerAccountHolder
    {
        [XmlElement("IndividualCustomer")]
        public IndividualCustomer? IndividualCustomer { get; set; }
    }

    public class IndividualCustomer
    {
        [XmlAttribute("Surname")]
        public string? Surname { get; set; }

        [XmlAttribute("Name")]
        public string? Name { get; set; }

        [XmlAttribute("Gender")]
        public string? Gender { get; set; }

        [XmlAttribute(DataType = "date")]
        public DateTime DateOfBirth { get; set; }

        [XmlAttribute("TempResPermitNumber")]
        public string? TempResPermitNumber { get; set; }

        [XmlAttribute("IDNumber")]
        public string? IDNumber { get; set; }

        [XmlAttribute("PassportNumber")]
        public string? PassportNumber { get; set; }

        [XmlAttribute("PassportCountry")]
        public string? PassportCountry { get; set; }

        [XmlElement("AdditionalCustomerData")]
        public AdditionalCustomerData? AdditionalCustomerData { get; set; }
    }

    public class AdditionalCustomerData
    {
        [XmlAttribute("AccountIdentifier")]
        public string? AccountIdentifier { get; set; }

        [XmlAttribute("StreetAddressLine1")]
        public string? StreetAddressLine1 { get; set; }

        [XmlAttribute("StreetSuburb")]
        public string? StreetSuburb { get; set; }

        [XmlAttribute("StreetCity")]
        public string? StreetCity { get; set; }

        [XmlAttribute("StreetProvince")]
        public string? StreetProvince { get; set; }

        [XmlAttribute("StreetPostalCode")]
        public string? StreetPostalCode { get; set; }

        [XmlAttribute("PostalAddressLine1")]
        public string? PostalAddressLine1 { get; set; }

        [XmlAttribute("PostalSuburb")]
        public string? PostalSuburb { get; set; }

        [XmlAttribute("PostalCity")]
        public string? PostalCity { get; set; }

        [XmlAttribute("PostalProvince")]
        public string? PostalProvince { get; set; }

        [XmlAttribute("PostalCode")]
        public string? PostalCode { get; set; }

        [XmlAttribute("ContactSurname")]
        public string? ContactSurname { get; set; }

        [XmlAttribute("ContactName")]
        public string? ContactName { get; set; }

        [XmlAttribute("Telephone")]
        public string? Telephone { get; set; }
    }
}
