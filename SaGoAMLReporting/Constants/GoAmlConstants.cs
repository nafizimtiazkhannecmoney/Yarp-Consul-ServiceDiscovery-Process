namespace SaGoAMLReporting.Constants
{
    public static class GoAmlConstants
    {
        static GoAmlConstants()
        {

        }

        //connection string name
        public const string ConnectionString = "DefaultConnection";
        public const string ConnectionString2 = "DefaultConnection2";

        //xsd dataset file for validation
        public const string xsdFilePath = "Files\\goAML_EE_FIC_XSD_Schema_V4_2_3.xsd";


        //store procedure name

        #region Common
        public const string Insert = "NEW";
        public const string Update = "UPDATE";
        public const string Select = "SELECT";
        #endregion

        #region GoAML_Data
        //Store Procedures
        public const string ActGoAml = "public.act_goaml";

        //Action Names for SP call parameter
        public const string GoAmlExists = "SELECT_GOAML_EXISTS";
        public const string AmlMonthlyReport = "GENERATE_AML_MONTHLY_REPORT";
        public const string PreviosEntityReference = "SELECT_PREVIOUS_ENTITY_REF";
        public const string BranchAndLocation = "SELECT_BRANCH_AND_LOCATION";
        public const string ReportingPerson = "SELECT_REPORTING_PERSON";
        public const string RemittanceXmlData = "SELECT_REMITTANCE_XML_DATA";
        public const string RemittanceData = "SELECT_REMITTANCE_DATA";        

        //for report indicators by report code
        public static string GetReportIndicator(string reportCode)
        {
            Dictionary<string, string> indicators = new Dictionary<string, string>()
            {
                { "CTR","RIND010" },
                { "CTRA","RIND111" },
                { "STR","RIND094" },
                { "STRB","RIND133" },
                { "IFTR","RIND130" },
            };
            return indicators[reportCode];
        }

        #endregion

        #region Sarb_Data

        //Store Procedures
        public const string ActRemittanceSarbData = "public.act_remittance_sarb_data";
        public const string ActTransactionSarbData = "public.act_transaction_sarb_data";


        public const string SarbData = "SELECT_SARB_DATA";
        public const string RejectedData = "SELECT_REJECTED";
        #endregion




    }
}
