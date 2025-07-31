namespace SARB_Reporting.Helpers
{
    public class DbContext
    {
        public string? ConnectionString { get; }

        public DbContext(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("ConnectionString");
        }

    }
}
