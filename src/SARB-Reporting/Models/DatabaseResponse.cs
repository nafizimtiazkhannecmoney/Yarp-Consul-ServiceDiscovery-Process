namespace SARB_Reporting.Models
{
    public class DatabaseResponse
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
        public DatabaseResponse(bool success = true, string message = "", string data = "")
        {
            Success = success;
            Message = message;
            Data = data;
        }
    }
}
