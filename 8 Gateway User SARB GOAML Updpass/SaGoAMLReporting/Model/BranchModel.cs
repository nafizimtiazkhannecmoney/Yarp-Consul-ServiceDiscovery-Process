using System.Text.Json.Serialization;

namespace SaGoAMLReporting.Model
{
    public class BranchRequest
    {
        public Header? Header { get; set; }
        public BranchPayload? Payload { get; set; }
    }   

    public class BranchPayload
    {
        public string? ActionName { get; set; }
    }

    // Model for the response
    public class BranchResponse
    {
        public BranchHeaderResponse? Header { get; set; }
        public List<BranchPayloadResponse>? Payload { get; set; }
    }

    public class BranchHeaderResponse
    {
        public Int64? UserId { get; set; }
        public string? ApiKey { get; set; }
        public string? ActionName { get; set; }
        public string? ServiceName { get; set; }
    }

    public class BranchPayloadResponse
    {
        public string? BranchCode { get; set; }
        public string? BranchName { get; set; }
    }
}
