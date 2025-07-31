namespace SaGoAMLReporting.Model
{
    public class Transaction1
    {
        public Int64 Id { get; set; }
    }
    public class XMLReportPayloadRequest
    {
        public int ReportingPersonId { get; set; }
        public string? ActionName { get; set; }
        public string? TxnType { get; set; }
        public string? ReportCode { get; set; }
        public string? CurrencyCodeLocal { get; set; }
        public string? Comments { get; set; }
        public string? ReportReason { get; set; }
        public string? ActionTaken { get; set; }
        public List<Transaction1>? Transactions { get; set; }
        public string? BranchCode { get; set; }
    }
    public class XMLReportRequest
    {
        public Header? Header { get; set; }
        public XMLReportPayloadRequest? Payload { get; set; }
    }
    public class XMLReportPayloadResponse
    {
        public string? Base64 { get; set; }
        public string? FileName { get; set; }
    }
    public class XMLReportResponse
    {
        public Header? Header { get; set; }
        public XMLReportPayloadResponse? Payload { get; set; }
    }

}
