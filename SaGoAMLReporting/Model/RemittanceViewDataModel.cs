namespace SaGoAMLReporting.Model
{
    public class RemittanceViewDataRequest 
    {
        public Header? Header { get; set; }
        public RemittanceViewDataRequestPayload? Payload { get; set; }
    }

    public class RemittanceViewDataRequestPayload
    {
        public string? ActionName { get; set; }
        public int? UserModifiedId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string? ReportCode { get; set; }
        public string? BranchCode { get; set; }
        public decimal? Amount { get; set; }
        public int? CustomerId { get; set; }
        public int? PageSize { get; set; }
        public int? PageNo { get; set; }
        public int? IsReported { get; set; }
    }
    
}
