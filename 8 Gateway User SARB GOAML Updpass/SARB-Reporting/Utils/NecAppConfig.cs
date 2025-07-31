namespace SARB_Reporting.Utils
{
    public  class NecAppConfig
    {

        public string? REGULARFilelocation { get; }
        public string? REPLACEFilelocation { get; }
        public string? REFUNDFilelocation { get; }
        public string? CANCELFilelocation { get; }

        public NecAppConfig(IConfiguration configuration)
        {
            REGULARFilelocation = configuration["GUYAMLSettings:REGULARFilelocation"];
            REPLACEFilelocation = configuration["GUYAMLSettings:REPLACEFilelocation"];
            REFUNDFilelocation = configuration["GUYAMLSettings:REFUNDFilelocation"];
            CANCELFilelocation = configuration["GUYAMLSettings:CANCELFilelocation"];
        }

    }
}
