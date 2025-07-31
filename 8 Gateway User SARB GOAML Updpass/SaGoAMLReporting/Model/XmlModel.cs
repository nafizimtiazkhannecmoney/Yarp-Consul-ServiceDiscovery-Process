using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Xml.Serialization;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SaGoAMLReporting.Model
{

    [XmlRoot("report")]
    public class XmlModel
    {
        public XmlModel()
        {
            Transaction = new List<Transaction>();
            ReportIndicators = new List<string>();
        }

        [XmlElement("rentity_id")]
        [JsonPropertyName("rentity_id")]
        public string? RentityId { get; set; }

        [XmlElement("rentity_branch")]
        [JsonPropertyName("rentity_branch")]
        public string? RentityBranch { get; set; }

        [XmlElement("submission_code")]
        [JsonPropertyName("submission_code")]
        public string? SubmissionCode { get; set; }

        [XmlElement("report_code")]
        [JsonPropertyName("report_code")]
        public string? ReportCode { get; set; }

        [XmlElement("entity_reference")]
        [JsonPropertyName("entity_reference")]
        public string? EntityReference { get; set; }

        [XmlElement("fiu_ref_number")]
        [JsonPropertyName("fiu_ref_number")]
        public string? FiuRefNumber { get; set; }

        [XmlElement("submission_date")]
        [JsonPropertyName("submission_date")]
        public DateTime SubmissionDate { get; set; }

        [XmlElement("currency_code_local")]
        [JsonPropertyName("currency_code_local")]
        public string? CurrencyCodeLocal { get; set; }

        [XmlElement("reporting_person")]
        [JsonPropertyName("reporting_person")]
        public ReportingPerson? ReportingPerson { get; set; }

        [XmlElement("location")]
        [JsonPropertyName("location")]
        public Location? Location { get; set; }

        [XmlElement("reason")]
        [JsonPropertyName("reason")]
        public string? Reason { get; set; }

        [XmlElement("action")]
        [JsonPropertyName("action")]
        public string? Action { get; set; }

        [XmlElement("transaction")]
        [JsonPropertyName("transaction")]
        public List<Transaction> Transaction { get; set; }

        [XmlArray("report_indicators")]
        [XmlArrayItem("indicator")]
        [JsonPropertyName("report_indicators")]
        public List<string> ReportIndicators { get; set; }
    }

    public class ReportingPerson
    {
        [XmlElement("gender")]
        [JsonPropertyName("gender")]
        public string? Gender { get; set; }

        [XmlElement("title")]
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [XmlElement("first_name")]
        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }

        [XmlElement("middle_name")]
        [JsonPropertyName("middle_name")]
        public string? MiddleName { get; set; }

        [XmlElement("prefix")]
        [JsonPropertyName("prefix")]
        public string? Prefix { get; set; }

        [XmlElement("last_name")]
        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }

        [XmlElement("birthdate")]
        [JsonPropertyName("birthdate")]
        public DateTime Birthdate { get; set; }

        [XmlElement("birth_place")]
        [JsonPropertyName("birth_place")]
        public string? BirthPlace { get; set; }

        [XmlElement("mothers_name")]
        [JsonPropertyName("mothers_name")]
        public string? MothersName { get; set; }

        [XmlElement("alias")]
        [JsonPropertyName("alias")]
        public string? Alias { get; set; }

        [XmlElement("ssn")]
        [JsonPropertyName("ssn")]
        public string? SSN { get; set; }

        [XmlElement("passport_number")]
        [JsonPropertyName("passport_number")]
        public string? PassportNumber { get; set; }

        [XmlElement("passport_country")]
        [JsonPropertyName("passport_country")]
        public string? PassportCountry { get; set; }

        [XmlElement("id_number")]
        [JsonPropertyName("id_number")]
        public string? IDNumber { get; set; }

        [XmlElement("nationality1")]
        [JsonPropertyName("nationality1")]
        public string? Nationality1 { get; set; }

        [XmlElement("nationality2")]
        [JsonPropertyName("nationality2")]
        public string? Nationality2 { get; set; }

        [XmlElement("nationality3")]
        [JsonPropertyName("nationality3")]
        public string? Nationality3 { get; set; }

        [XmlElement("residence")]
        [JsonPropertyName("residence")]
        public string? Residence { get; set; }

        [XmlElement("phones")]
        [JsonPropertyName("phones")]
        public Phones? Phones { get; set; }

        [XmlElement("addresses")]
        [JsonPropertyName("addresses")]
        public Addresses? Addresses { get; set; }

        [XmlElement("email")]
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [XmlElement("occupation")]
        [JsonPropertyName("occupation")]
        public string? Occupation { get; set; }

        [XmlElement("employer_name")]
        [JsonPropertyName("employer_name")]
        public string? EmployerName { get; set; }

        [XmlElement("employer_address_id")]
        [JsonPropertyName("employer_address_id")]
        public Address? EmployerAddressId { get; set; }

        [XmlElement("employer_phone_id")]
        [JsonPropertyName("employer_phone_id")]
        public Phone? EmployerPhoneID { get; set; }

        [XmlElement("identification")]
        [JsonPropertyName("identification")]
        public Identification? Identification { get; set; }

        [XmlElement("tax_number")]
        [JsonPropertyName("tax_number")]
        public string? TaxNumber { get; set; }

        [XmlElement("tax_reg_number")]
        [JsonPropertyName("tax_reg_number")]
        public string? TaxRegNumber { get; set; }

        [XmlElement("source_of_wealth")]
        [JsonPropertyName("source_of_wealth")]
        public string? SourceOfWealth { get; set; }

        [XmlElement("comments")]
        [JsonPropertyName("comments")]
        public string? Comments { get; set; }
    }

    public class Phones
    {
        [XmlElement("phone")]
        [JsonPropertyName("phone")]
        public Phone? Phone { get; set; }
    }

    public class Phone
    {
        [XmlElement("tph_contact_type")]
        [JsonPropertyName("tph_contact_type")]
        public int ContactType { get; set; }

        [XmlElement("tph_communication_type")]
        [JsonPropertyName("tph_communication_type")]
        public int CommunicationType { get; set; }

        [XmlElement("tph_country_prefix")]
        [JsonPropertyName("tph_country_prefix")]
        public string? CountryPrefix { get; set; }

        [XmlElement("tph_number")]
        [JsonPropertyName("tph_number")]
        public string? Number { get; set; }
    }

    public class Addresses
    {
        [XmlElement("address")]
        [JsonPropertyName("address")]
        public Address? Address { get; set; }
    }

    public class Address
    {
        [XmlElement("address_type")]
        [JsonPropertyName("addressType")]
        public int AddressType { get; set; }

        [XmlElement("address")]
        [JsonPropertyName("address")]
        public string AddressLine { get; set; }

        [XmlElement("town")]
        [JsonPropertyName("town")]
        public string? Town { get; set; }

        [XmlElement("city")]
        [JsonPropertyName("city")]
        public string? City { get; set; }

        [XmlElement("zip")]
        [JsonPropertyName("zip")]
        public string? Zip { get; set; }

        [XmlElement("country_code")]
        [JsonPropertyName("countryCode")]
        public string? CountryCode { get; set; }

        [XmlElement("state")]
        [JsonPropertyName("state")]
        public string? State { get; set; }
    }

    public class Transaction
    {
        [XmlElement("id")]
        [XmlIgnore]
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [XmlElement("transactionnumber")]
        [JsonPropertyName("transactionnumber")]
        public string? TransactionNumber { get; set; }

        [XmlElement("internal_ref_number")]
        [JsonPropertyName("internal_ref_number")]
        public string? InternalRefNumber { get; set; }

        [XmlElement("transaction_location")]
        [JsonPropertyName("transaction_location")]
        public string? TransactionLocation { get; set; }

        [XmlElement("transaction_description")]
        [JsonPropertyName("transaction_description")]
        public string? TransactionDescription { get; set; }

        [XmlElement("date_transaction")]
        [JsonPropertyName("date_transaction")]
        [DisplayFormat(DataFormatString = "{yyyy-MM-ddTHH:mm:ss.fffZ}")]
        public DateTime? DateTransaction { get; set; }

        [XmlElement("teller")]
        [JsonPropertyName("teller")]
        public string? Teller { get; set; }

        [XmlElement("authorized")]
        [JsonPropertyName("authorized")]
        public string? Authorized { get; set; }

        [XmlElement("late_deposit")]
        [JsonPropertyName("late_deposit")]
        public string? LateDeposit { get; set; }

        [XmlElement("date_posting")]
        [JsonPropertyName("date_posting")]
        public string? DatePosting { get; set; }

        [XmlElement("value_date")]
        [JsonPropertyName("value_date")]
        public string? ValueDate { get; set; }

        [XmlElement("transmode_code")]
        [JsonPropertyName("transmode_code")]
        public int TransmodeCode { get; set; } = 0;

        [XmlElement("transmode_comment")]
        [JsonPropertyName("transmode_comment")]
        public string? TransmodeComment { get; set; }

        [XmlElement("amount_local")]
        [JsonPropertyName("amount_local")]
        public decimal AmountLocal { get; set; }

        [XmlElement("t_from_my_client")]
        [JsonPropertyName("t_from_my_client")]
        public FromMyClient? FromMyClient { get; set; }

        [XmlElement("t_to")]
        [JsonPropertyName("t_to")]
        public To? To { get; set; }

        [XmlElement("comments")]
        [JsonPropertyName("comments")]
        public string? Comments { get; set; }
    }

    public class FromMyClient
    {
        [XmlElement("from_funds_code")]
        [JsonPropertyName("from_funds_code")]
        public int FromFundsCode { get; set; }

        [XmlElement("from_person")]
        [JsonPropertyName("from_person")]
        public Person? FromPerson { get; set; }

        [XmlElement("from_country")]
        [JsonPropertyName("from_country")]
        public string? FromCountry { get; set; }
    }

    public class Person
    {
        [XmlElement("gender")]
        [JsonPropertyName("gender")]
        public string? Gender { get; set; }

        [XmlElement("title")]
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [XmlElement("first_name")]
        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }

        [XmlElement("middle_name")]
        [JsonPropertyName("middle_name")]
        public string? MiddleName { get; set; }

        [XmlElement("prefix")]
        [JsonPropertyName("prefix")]
        public string? Prefix { get; set; }

        [XmlElement("last_name")]
        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }

        [XmlElement("birthdate")]
        [JsonPropertyName("birthdate")]
        //[DisplayFormat(DataFormatString = "{yyyy-MM-ddT00:00:00}")]
        public string? Birthdate { get; set; }        

        [XmlElement("birth_place")]
        [JsonPropertyName("birth_place")]
        public string? BirthPlace { get; set; }

        [XmlElement("mothers_name")]
        [JsonPropertyName("mothers_name")]
        public string? MothersName { get; set; }

        [XmlElement("alias")]
        [JsonPropertyName("alias")]
        public string? Alias { get; set; }
        
        [XmlElement("ssn")]
        [JsonPropertyName("ssn")]
        public string? Ssn { get; set; }

        [XmlElement("passport_number")]
        [JsonPropertyName("passport_number")]
        public string? PassportNumber { get; set; }

        [XmlElement("passport_country")]
        [JsonPropertyName("passport_country")]
        public string? PassportCountry { get; set; }

        [XmlElement("id_number")]
        [JsonPropertyName("id_number")]
        public string? IdNumber { get; set; } 

        [XmlElement("nationality1")]
        [JsonPropertyName("nationality1")]
        public string? Nationality1 { get; set; }

        [XmlElement("nationality2")]
        [JsonPropertyName("nationality2")]
        public string? Nationality2 { get; set; }

        [XmlElement("nationality3")]
        [JsonPropertyName("nationality3")]
        public string? Nationality3 { get; set; }

        [XmlElement("residence")]
        [JsonPropertyName("residence")]
        public string? Residence { get; set; }
        
        [XmlElement("phones")]
        [JsonPropertyName("phones")]
        public Phones? Phones { get; set; }

        [XmlElement("addresses")]
        [JsonPropertyName("addresses")]
        public Addresses? Addresses { get; set; }

        [XmlElement("identification")]
        [JsonPropertyName("identification")]
        public Identification? Identification { get; set; }        

        [XmlElement("tax_number")]
        [JsonPropertyName("tax_number")]
        public string? TaxNumber { get; set; }

        [XmlElement("tax_reg_number")]
        [JsonPropertyName("tax_reg_number")]
        public string? TaxRegNumber { get; set; }

        [XmlElement("source_of_wealth")]
        [JsonPropertyName("source_of_wealth")]
        public string? SourceOfWealth { get; set; }

        [XmlElement("comments")]
        [JsonPropertyName("comments")]
        public string? Comments { get; set; }
    }

    public class Identification
    {
        [XmlElement("type")]
        [JsonPropertyName("type")]
        public int Type { get; set; }

        [XmlElement("number")]
        [JsonPropertyName("number")]
        public string? Number { get; set; }

        [XmlElement("issue_country")]
        [JsonPropertyName("issue_country")]
        public string? IssueCountry { get; set; }
    }

    public class To
    {
        [XmlElement("to_funds_code")]
        [JsonPropertyName("to_funds_code")]
        public int ToFundsCode { get; set; }

        [XmlElement("to_funds_comment")]
        [JsonPropertyName("to_funds_comment")]
        public int ToFundsComment { get; set; }

        [XmlElement("to_foreign_currency")]
        [JsonPropertyName("to_foreign_currency")]
        public string? ToForeignCurrency { get; set; }

        [XmlElement("to_person")]
        [JsonPropertyName("to_person")]
        public Person? ToPerson { get; set; }

        [XmlElement("to_country")]
        [JsonPropertyName("to_country")]
        public string? ToCountry { get; set; }
    }

    public class ToPerson
    {
        [XmlElement("first_name")]
        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }

        [XmlElement("last_name")]
        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }
    }

    public class Location
    {
        [XmlElement("address_type")]
        [JsonPropertyName("address_type")]
        public int AddressType { get; set; }

        [XmlElement("address")]
        [JsonPropertyName("address")]
        public string? AddressLine { get; set; }

        [XmlElement("town")]
        [JsonPropertyName("town")]
        public string? Town { get; set; }

        [XmlElement("city")]
        [JsonPropertyName("city")]
        public string? City { get; set; }

        [XmlElement("zip")]
        [JsonPropertyName("zip")]
        public string? Zip { get; set; }

        [XmlElement("country_code")]
        [JsonPropertyName("country_code")]
        public string? CountryCode { get; set; }

        [XmlElement("state")]
        [JsonPropertyName("state")]
        public string? State { get; set; }
    }
}
