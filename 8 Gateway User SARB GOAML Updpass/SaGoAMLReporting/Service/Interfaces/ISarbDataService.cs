using Npgsql;
using SaGoAMLReporting.Model;
using System.Collections;
using System.Globalization;
using System.Xml.Serialization;

namespace SaGoAMLReporting.Service
{
    public interface ISarbDataService
    {
        Task<dynamic?> GetRemittanceSarbViewData(RemittanceSarbDataRequest? payload);
        Task<dynamic?> GetTransactionSarbViewData(RemittanceSarbDataRequest? payload);
        Task<dynamic?> GetRejectedSarbViewData(RemittanceSarbDataRequest? model);
        Task<dynamic?> GetSarbStatusViewData(RemittanceSarbDataRequest? model);
        Task<dynamic?> GetTransactionSarbExcelData(RemittanceSarbDataRequest? model);
        string GenerateSarbStatusTimelineCSV(dynamic dataList);
        string GenerateTransactionSarbCSV(List<SarbExcelDataModel> dataList);
        Task<dynamic?> GetRemittanceOutwardViewData(DateTime from, DateTime to);
    }
}

