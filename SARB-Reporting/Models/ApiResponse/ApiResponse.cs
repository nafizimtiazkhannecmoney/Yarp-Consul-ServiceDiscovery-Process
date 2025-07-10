namespace SARB_Reporting.Models.ApiResponse
{
    public class ApiResponse
    {
        public int? StatusCode { get; set; }
        public bool IsSuccessStatusCode { get; set; }
        public string? Message { get; set; }
        public string? Error { get; set; }
    }
}
