namespace SaGoAMLReporting.Service.Interfaces
{
    public interface ISqlService
    {
        Task<string?> GetReturnedJsonString(dynamic? model, string query);
        string GetSqlServConnString();
    }
}
