namespace SaGoAMLReporting.Model
{
    public class SarbStatusDataModel
    {
        public int total_txn { get; set; }
        public Dictionary<string, DateTime?>? jsn_ref_issue_date { get; set; }
        public Dictionary<string, DateTime?>? jsn_ref_accept_date { get; set; }
        public Dictionary<string, DateTime?>? jsn_ref_upload_date { get; set; }
    }
}
