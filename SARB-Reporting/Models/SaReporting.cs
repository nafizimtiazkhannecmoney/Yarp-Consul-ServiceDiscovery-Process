using System.Xml.Serialization;

namespace SARB_Reporting.Models
{
    public class SaReporting
    {
        [XmlRoot("OriginalTransaction")]
        public class OriginalTransaction
        {
            public OriginalTransaction()
            {
                this.NonResident = new NonResident();
                this.ResidentCustomerAccountHolder = new ResidentCustomerAccountHolder();
                this.MonetaryDetails = new MonetaryDetails();
            }
            [XmlElement("LineNumber")]
            public int LineNumber { get; set; }

            [XmlElement("ReportingQualifier")]
            public string ReportingQualifier { get; set; }

            [XmlElement("Flow")]
            public string Flow { get; set; }

            [XmlElement("ReplacementTransaction")]
            public string ReplacementTransaction { get; set; }

            [XmlElement("Date")]
            public DateTime Date { get; set; }

            [XmlElement("TrnReference")]
            public string TrnReference { get; set; }

            [XmlElement("BranchCode")]
            public string BranchCode { get; set; }

            [XmlElement("BranchName")]
            public string BranchName { get; set; }

            [XmlElement("OriginatingBank")]
            public string OriginatingBank { get; set; }

            [XmlElement("OriginatingCountry")]
            public string OriginatingCountry { get; set; }

            [XmlElement("TotalValue")]
            public double TotalValue { get; set; }

            [XmlElement("ReceivingBank")]
            public string ReceivingBank { get; set; }

            [XmlElement("ReceivingCountry")]
            public string ReceivingCountry { get; set; }

            [XmlElement("NonResident")]
            public NonResident NonResident { get; set; }

            [XmlElement("ResidentCustomerAccountHolder")]
            public ResidentCustomerAccountHolder ResidentCustomerAccountHolder { get; set; }

            [XmlElement("MonetaryDetails")]
            public MonetaryDetails MonetaryDetails { get; set; }
            public string? FileName { get; set; }
            public string? FilePath { get; set; }
        }

        public class NonResident
        {
            public NonResident()
            {
                this.Individual = new Individual();
            }
            [XmlElement("Individual")]
            public Individual Individual { get; set; }
        }

        public class Individual
        {
            public Individual()
            {
                this.AdditionalNonResidentData = new AdditionalNonResidentData();
            }
            [XmlElement("Surname")]
            public string Surname { get; set; }

            [XmlElement("Name")]
            public string Name { get; set; }

            [XmlElement("AdditionalNonResidentData")]
            public AdditionalNonResidentData AdditionalNonResidentData { get; set; }
        }

        public class AdditionalNonResidentData
        {
            [XmlElement("AccountIdentifier")]
            public string AccountIdentifier { get; set; }

            [XmlElement("AccountNumber")]
            public string AccountNumber { get; set; }

            [XmlElement("Country")]
            public string Country { get; set; }
        }

        public class ResidentCustomerAccountHolder
        {
            public ResidentCustomerAccountHolder()
            {
                this.IndividualCustomer = new IndividualCustomer();
            }
            [XmlElement("IndividualCustomer")]
            public IndividualCustomer IndividualCustomer { get; set; }
        }

        public class IndividualCustomer
        {
            public IndividualCustomer()
            {
                this.AdditionalCustomerData = new AdditionalCustomerData();
            }
            [XmlElement("Surname")]
            public string Surname { get; set; }

            [XmlElement("Name")]
            public string Name { get; set; }

            [XmlElement("Gender")]
            public string Gender { get; set; }

            [XmlElement("DateOfBirth")]
            public DateTime DateOfBirth { get; set; }

            [XmlElement("PassportNumber")]
            public string PassportNumber { get; set; }

            [XmlElement("PassportCountry")]
            public string PassportCountry { get; set; }

            [XmlElement("IDNumber")]
            public string IDNumber { get; set; }

            [XmlElement("AdditionalCustomerData")]
            public AdditionalCustomerData AdditionalCustomerData { get; set; }
        }

        public class AdditionalCustomerData
        {
            [XmlElement("AccountIdentifier")]
            public string AccountIdentifier { get; set; }

            [XmlElement("StreetAddressLine1")]
            public string StreetAddressLine1 { get; set; }

            [XmlElement("StreetSuburb")]
            public string StreetSuburb { get; set; }

            [XmlElement("StreetCity")]
            public string StreetCity { get; set; }

            [XmlElement("StreetProvince")]
            public string StreetProvince { get; set; }

            [XmlElement("StreetPostalCode")]
            public string StreetPostalCode { get; set; }

            [XmlElement("PostalAddressLine1")]
            public string PostalAddressLine1 { get; set; }

            [XmlElement("PostalSuburb")]
            public string PostalSuburb { get; set; }

            [XmlElement("PostalCity")]
            public string PostalCity { get; set; }

            [XmlElement("PostalProvince")]
            public string PostalProvince { get; set; }

            [XmlElement("PostalCode")]
            public string PostalCode { get; set; }

            [XmlElement("ContactSurname")]
            public string ContactSurname { get; set; }

            [XmlElement("ContactName")]
            public string ContactName { get; set; }

            [XmlElement("Telephone")]
            public string Telephone { get; set; }
        }

        public class MonetaryDetails
        {
            [XmlElement("SequenceNumber")]
            public int SequenceNumber { get; set; }

            [XmlElement("ThirdPartyContactSurname")]
            public string? ThirdPartyContactSurname { get; set; }

            [XmlElement("ThirdPartyContactName")]
            public string? ThirdPartyContactName { get; set; }

            [XmlElement("ThirdPartyTelephone")]
            public string? ThirdPartyTelephone { get; set; }

            [XmlElement("LegalEntityThirdPartyName")]
            public string? LegalEntityThirdPartyName { get; set; }

            [XmlElement("MoneyTransferAgentIndicator")]
            public string? MoneyTransferAgentIndicator { get; set; }

            [XmlElement("RandValue")]
            public double RandValue { get; set; }

            [XmlElement("ForeignValue")]
            public double ForeignValue { get; set; }

            [XmlElement("ForeignCurrencyCode")]
            public string ForeignCurrencyCode { get; set; }

            [XmlElement("BoPCategory")]
            public string BoPCategory { get; set; }

            [XmlElement("RulingsSection")]
            public string RulingsSection { get; set; }

            [XmlElement("LocationCountry")]
            public string LocationCountry { get; set; }
        }

        public class OriginalTransactionREport
        {
            public string? FileName { get; set; }
            public string? FilePath { get; set; }
            public string? TrnReference { get; set; }
            public string? ResponseByFile { get; set; }
            public string? SarbReference { get; set; }
            public string? ResponseByReference { get; set; }
            public string? Errors { get; set; }
            public string? Status { get; set; }
        }


        public class Errors
        {
            [XmlAttribute("reference")]
            public string Reference { get; set; }

            [XmlElement("Error")]
            public ErrorDetail? Error { get; set; }
        }

        public class ErrorDetail
        {
            [XmlAttribute("code")]
            public string? Code { get; set; }

            [XmlAttribute("description")]
            public string? Description { get; set; }

            [XmlAttribute("level")]
            public string Level { get; set; }

            [XmlAttribute("severity")]
            public string Severity { get; set; }
        }
        public class Errors2
        {
            [XmlText]
            public string Message { get; set; }
        }

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

        public class FileExtention
        {
            public string? FileName { get; set; }
            public string? FileLocation { get; set; }
            public bool? IsFileSave { get; set; }
        }



        [XmlRoot("FINSURV")]
        public class FINSURV
        {
            [XmlElement("FileReference")]
            public FileReference FileReference { get; set; }
            public string? responsebyreference { get; set; }
        }

        public class FileReference
        {
            [XmlAttribute("RefNumber")]
            public string RefNumber { get; set; }

            [XmlAttribute("Status")]
            public string Status { get; set; }

            [XmlElement("Transaction")]
            public List<Transaction> Transactions { get; set; }
        }

        public class Transaction
        {
            [XmlAttribute("LineNumber")]
            public int LineNumber { get; set; }

            [XmlAttribute("ReportingQualifier")]
            public string ReportingQualifier { get; set; }

            [XmlAttribute("Status")]
            public string Status { get; set; }

            [XmlAttribute("TrnReference")]
            public string TrnReference { get; set; }

            [XmlElement("Error")]
            public List<TransactionError> Errors { get; set; }

            [XmlElement("Warning")]
            public List<TransactionWarning> Warnings { get; set; }
        }

        public class TransactionError
        {
            [XmlAttribute("ErrorCode")]
            public string ErrorCode { get; set; }

            [XmlAttribute("ErrorDescription")]
            public string ErrorDescription { get; set; }

            [XmlAttribute("FieldName")]
            public string FieldName { get; set; }

            [XmlAttribute("SequenceNumber")]
            public int SequenceNumber { get; set; }

            [XmlAttribute("SubSequence")]
            public string SubSequence { get; set; }
        }

        public class TransactionWarning
        {
            [XmlAttribute("WarningCode")]
            public string WarningCode { get; set; }

            [XmlAttribute("WarningDescription")]
            public string WarningDescription { get; set; }

            [XmlAttribute("FieldName")]
            public string FieldName { get; set; }

            [XmlAttribute("SequenceNumber")]
            public int SequenceNumber { get; set; }

            [XmlAttribute("SubSequence")]
            public string SubSequence { get; set; }
        }
        public class TransactionError2
        {
            public string? ErrorCode { get; set; }
            public string ErrorCode2 { get; set; }

        }

    }
}
