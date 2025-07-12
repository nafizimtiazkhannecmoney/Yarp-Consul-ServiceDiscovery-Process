namespace SaGoAMLReporting.Model
{
    public class Header
    {
        public string? AuthToken { get; set; }
        public string? ActionName { get; set; }
        public string? ServiceName { get; set; }
        public string? CopyRight { get; set; }
        public string? RequestToken { get; set; }
        public string? Status { get; set; }
        public int Version { get; set; }
        public Int64? UserModifidId { get; set; }
    }
}
