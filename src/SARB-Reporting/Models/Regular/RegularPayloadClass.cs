namespace SARB_Reporting.Models.Regular
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public class TransactionData
    {
        public int? Rows { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }

        [JsonProperty("lastRunDate")]
        public DateTime? LastRunDate { get; set; }

        [JsonProperty("txnId")]
        public int? TxnId { get; set; }

        [JsonProperty("remitSarbDataId")]
        public int? RemitSarbDataId { get; set; }

        [JsonProperty("remitSarbDataVer")]
        public int? RemitSarbDataVer { get; set; }

        [JsonProperty("txnSarbDataId")]
        public int? TxnSarbDataId { get; set; }

        [JsonProperty("txnSarbDataVer")]
        public int? TxnSarbDataVer { get; set; }

        [JsonProperty("remitPrefix")]
        public string RemitPrefix { get; set; }

        [JsonProperty("beneficiaryId")]
        public string BeneficiaryId { get; set; }

        [JsonProperty("beneficiaryName")]
        public string BeneficiaryName { get; set; }

        [JsonProperty("beneficiarySurname")]
        public string BeneficiarySurname { get; set; }

        [JsonProperty("beneficiaryCountry")]
        public string BeneficiaryCountry { get; set; }

        [JsonProperty("beneficiaryAccNo")]
        public string BeneficiaryAccNo { get; set; }

        [JsonProperty("beneficiaryAccIdentifier")]
        public string BeneficiaryAccIdentifier { get; set; }

        [JsonProperty("beneficiaryCurrencyCode")]
        public string BeneficiaryCurrencyCode { get; set; }

        [JsonProperty("beneficiaryAmount")]
        public double? BeneficiaryAmount { get; set; }

        [JsonProperty("customerId")]
        public string CustomerId { get; set; }

        [JsonProperty("customerName")]
        public string CustomerName { get; set; }

        [JsonProperty("customerSurname")]
        public string CustomerSurname { get; set; }

        [JsonProperty("customerGender")]
        public string CustomerGender { get; set; }

        [JsonProperty("customerDob")]
        public DateTime? CustomerDob { get; set; }

        [JsonProperty("customerIdentityNo")]
        public string CustomerIdentityNo { get; set; }

        [JsonProperty("customerPassportNo")]
        public string CustomerPassportNo { get; set; }

        [JsonProperty("customerPassportCountry")]
        public string CustomerPassportCountry { get; set; }

        [JsonProperty("customerAccIdentifier")]
        public string CustomerAccIdentifier { get; set; }

        [JsonProperty("customerAddress")]
        public string CustomerAddress { get; set; }

        [JsonProperty("customerSuburb")]
        public string CustomerSuburb { get; set; }

        [JsonProperty("customerCity")]
        public string CustomerCity { get; set; }

        [JsonProperty("customerProvince")]
        public string CustomerProvince { get; set; }

        [JsonProperty("customerPostalCode")]
        public string CustomerPostalCode { get; set; }

        public string CustomerPostalAddress { get; set; }
        public string CustomerPostalSuburb { get; set; }
        public string CustomerPostalCity { get; set; }
        public string CustomerPostalProvince { get; set; }
        public string CustomerContactName { get; set; }
        public string CustomerContactSurname { get; set; }

        [JsonProperty("origTxnRefNo")]
        public string OrigTxnRefNo { get; set; }

        [JsonProperty("customerAccNo")]
        public string CustomerAccNo { get; set; }

        [JsonProperty("customerPhone")]
        public string CustomerPhone { get; set; }

        [JsonProperty("customerAmount")]
        public double? CustomerAmount { get; set; }

        [JsonProperty("sarbEnvironment")]
        public string SarbEnvironment { get; set; }

        [JsonProperty("sarbVersion")]
        public string SarbVersion { get; set; }

        [JsonProperty("reportingQualifier")]
        public string ReportingQualifier { get; set; }

        [JsonProperty("flow")]
        public string Flow { get; set; }

        [JsonProperty("txnDate")]
        public DateTime? TxnDate { get; set; }

        [JsonProperty("referenceNo")]
        public string ReferenceNo { get; set; }

        [JsonProperty("branchCode")]
        public string BranchCode { get; set; }

        [JsonProperty("branchName")]
        public string BranchName { get; set; }

        [JsonProperty("originatingBank")]
        public string OriginatingBank { get; set; }

        [JsonProperty("originatingCountry")]
        public string OriginatingCountry { get; set; }

        [JsonProperty("receivingBank")]
        public string ReceivingBank { get; set; }

        [JsonProperty("receivingCountry")]
        public string ReceivingCountry { get; set; }

        [JsonProperty("sequenceNo")]
        public string SequenceNo { get; set; }

        [JsonProperty("mtai")]
        public string Mtai { get; set; }

        [JsonProperty("tpcSurname")]
        public string TpcSurname { get; set; }

        [JsonProperty("tpcName")]
        public string TpcName { get; set; }

        [JsonProperty("tpTelephone")]
        public string TpTelephone { get; set; }

        [JsonProperty("letpName")]
        public string LetpName { get; set; }

        [JsonProperty("bopCategory")]
        public string BopCategory { get; set; }

        [JsonProperty("rulingSection")]
        public string RulingSection { get; set; }

        [JsonProperty("locationCountry")]
        public string LocationCountry { get; set; }

        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("txnSarbKey")]
        public int? TxnSarbKey { get; set; }

        [JsonProperty("version")]
        public int? Version { get; set; }

        [JsonProperty("sarbRefNo")]
        public string SarbRefNo { get; set; }

        [JsonProperty("response")]
        public string Response { get; set; }

        [JsonProperty("retrieveResponse")]
        public string RetrieveResponse { get; set; }

        [JsonProperty("responseByRef")]
        public string ResponseByRef { get; set; }

        [JsonProperty("sarbRequest")]
        public string SarbRequest { get; set; }

        [JsonProperty("warningDetails")]
        public string WarningDetails { get; set; }

        [JsonProperty("sarbErrorDesc")]
        public string SarbErrorDesc { get; set; }

        [JsonProperty("txnType")]
        public string TxnType { get; set; }

        [JsonProperty("dailyLimit")]
        public double? DailyLimit { get; set; }

        [JsonProperty("monthlyLimit")]
        public double? MonthlyLimit { get; set; }

        [JsonProperty("yearlyLimit")]
        public double? YearlyLimit { get; set; }

        [JsonProperty("totalTxn")]
        public int? TotalTxn { get; set; }

        [JsonProperty("uploadDate")]
        public DateTime? UploadDate { get; set; }

        [JsonProperty("acceptedDate")]
        public DateTime? AcceptedDate { get; set; }

        [JsonProperty("dataRow")]
        public List<string> DataRow { get; set; }

        [JsonProperty("sarbStatus")]
        public Dictionary<string, DateTime> SarbStatus { get; set; }

        [JsonProperty("sarbIssueDates")]
        public Dictionary<string, DateTime> SarbIssueDates { get; set; }

        [JsonProperty("sarbUploadDates")]
        public Dictionary<string, DateTime> SarbUploadDates { get; set; }

        [JsonProperty("sarbAcceptDates")]
        public Dictionary<string, DateTime> SarbAcceptDates { get; set; }

        public string Base64 { get; set; }
        public string Dob { get; set; }
        public int? IsSubmitted { get; set; }
    }

}
