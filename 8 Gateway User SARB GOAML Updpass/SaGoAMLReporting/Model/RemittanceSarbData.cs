namespace SaGoAMLReporting.Model
{
    public class RemittanceSarbData
    {

    }

    public class RemittanceSarbDataRequest 
    {
        public Header? Header { get; set; }
        public RemittanceSarbDataResponsePayload? Payload { get; set; }
    }



    public class RemittanceSarbDataResponsePayload
    {
        public Int64? UserModifiedId { get; set; }
        public string? ActionName { get; set; }
        public int? Id { get; set; }
        public int? Rows { get; set; }
        public string? BranchCode { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int? PageSize { get { return Id; } }
        public int? PageNo { get { return Rows; } }        
    }
}
