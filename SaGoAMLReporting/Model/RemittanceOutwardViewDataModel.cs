using System.Security.Cryptography.Xml;

namespace SaGoAMLReporting.Model
{
    public class RemittanceOutwardViewDataRequest
    {
        public Header? Header { get; set; }
        public RemittanceSarbDataResponsePayload? Payload { get; set; }
    }
    public class RemittanceOutwardViewDataModel
    {
        public string? ActionName { get; set; }
        public long? Id { get; set; }
        public long? TxnId { get; set; }
        public string? ReportingQualifier { get; set; }
        public string? Flow { get; set; }
        public int? SequenceNo { get; set; }
        public string? Mtai { get; set; }
        public string? RulingSection { get; set; }
        public string? OriginatingCountry { get; set; }
        public string? CustomerAccIdentifier { get; set; }
        public string? ReceivingBank { get; set; }
        public DateTime? TxnDate { get; set; }
        public string? RemitPrefix { get; set; }
        public string? OrigTxnRefNo { get; set; }
        public string? ReferenceNo { get; set; }
        public string? BranchCode { get; set; }
        public string? BranchName { get; set; }
        public string? OriginatingBank { get; set; }
        public string? ReceivingCountry { get; set; }
        public long? BeneficiaryId { get; set; }
        public string? BeneficiaryName { get; set; }
        public string? BeneficiarySurname { get; set; }
        public string? BeneficiaryAccIdentifier { get; set; }
        public string? BeneficiaryAccNo { get; set; }
        public string? BeneficiaryCountry { get; set; }
        public string? BeneficiaryCurrencyCode { get; set; }
        public string? BeneficiaryAmount { get; set; }
        public long? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerSurname { get; set; }
        public string? CustomerGender { get; set; }
        public DateTime? CustomerDob { get; set; }
        public string? CustomerIdentityNo { get; set; }
        public DateTime? CustomerPassportNo { get; set; }
        public string? CustomerPassportCountry { get; set; }
        public string? CustomerAddress { get; set; }
        public string? CustomerSuburb { get; set; }
        public string? CustomerAccNo { get; set; }
        public string? CustomerCity { get; set; }
        public string? CustomerProvince { get; set; }
        public string? CustomerPostalCode { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerAmount { get; set; }
        public string? BopCategory { get; set; }
        public string? LocationCountry { get; set; }
        public DateTime? ExportedDate { get; set; }
        public string? SarbEnvironment { get; set; }
    }
}
