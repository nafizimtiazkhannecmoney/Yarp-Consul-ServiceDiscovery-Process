namespace SaGoAMLReporting.Model
{
    //public class STR_Monthly_Report_Request_Header
    //{
    //    public string? AuthToken { get; set; }
    //    public string? ActionName { get; set; }
    //    public string? CopyRight { get; set; }
    //    public string? RequestToken { get; set; }
    //    public string? ServiceName { get; set; }
    //    public string? Status { get; set; }
    //    public int Version { get; set; }
    //    public int UserModifidId { get; set; }
    //}
    public class STR_Monthly_Report_Request_Data
    {
        public string? ActionName { get; set; }
        public string? Opt1_v1 { get; set; }
        public string? Opt1_v2 { get; set; }
        public string? Opt2_v1 { get; set; }
        public string? Opt2_v2 { get; set; }

        public decimal? Opt3 { get; set; } = 0;
        public string? Opt4 { get; set; }
        public string? Opt5 { get; set; }

        public string? Opt6_v1 { get; set; }
        public string? Opt6_v2 { get; set; }
        public string? Opt6_v3 { get; set; }
        public int Opt7 { get; set; }
        public int Opt8 { get; set; }
        public string? Opt9 { get; set; }
        public string? AmlroName { get; set; }
        public string? FromDate { get; set; }
    }
    public class STR_Monthly_Report_Request_Body
    {
        public Header? Header { get; set; }
        public STR_Monthly_Report_Request_Data? Payload { get; set; }
    }    
}
