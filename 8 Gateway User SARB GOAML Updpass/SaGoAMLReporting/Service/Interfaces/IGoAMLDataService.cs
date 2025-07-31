using Npgsql;
using SaGoAMLReporting.Model;
using System.Collections;
using System.Globalization;
using System.Xml.Serialization;

namespace SaGoAMLReporting.Service
{
    public interface IGoAMLDataService
    {        
        Task<GoAMLModel?> GetExistFileInGoAML(Int64 txnId, string txnType);
        Task<(XmlModel?, GoAMLModel?)> GetRemittanceSarbData(XMLReportRequest request);
        Task<dynamic?> GetRemittanceViewData(RemittanceViewDataRequest? model);
        Task<dynamic?> GetBranchViewData(BranchRequest? model);
        Task GetGeneratedAMLMonthlyReport(STR_Monthly_Report_Request_Data model);
        Task<int> UpdateGoAML(GoAMLModel model);
        Task<int> InsertGoAML(GoAMLModel model);
        Task<byte[]> ExportReportFile(string ReportPath, string FileName, 
                                    string FileType = "Excel", string ContentType = "application/vnd.ms-excel", 
                                    string reportName = "", Hashtable? parameterList = null, 
                                    STR_Monthly_Report_Request_Body? data = null);
        Task<byte[]> GetXmlBytesOfRequestedData(XmlModel? request);
    }
}

