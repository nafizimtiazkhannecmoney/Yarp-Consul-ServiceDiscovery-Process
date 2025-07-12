using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Reporting.NETCore;
using Microsoft.ReportingServices.Diagnostics.Internal;
using Npgsql;
using SaGoAMLReporting.Constants;
using SaGoAMLReporting.Files.Report.DataSet;
using SaGoAMLReporting.Helper;
using SaGoAMLReporting.Model;
using SaGoAMLReporting.Service.Interfaces;
using System.Collections;
using System.Data;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Text.Json;
using System.Xml.Serialization;

namespace SaGoAMLReporting.Service
{
    public class GoAMLDataService : IGoAMLDataService
    {
        private ISqlService SqlService { get; set; }
        public string Query { get; set; }

        public GoAMLDataService(ISqlService SqlService)
        {
            this.SqlService = SqlService;
            this.Query = GoAmlConstants.ActGoAml;
        }

        #region get_data_from_db        

        /**
         * Get Existing File Info in GoAML table   
         * 
         * @author: Mohiuddin
         * @since: 01/01/2025      
         */
        public async Task<GoAMLModel?> GetExistFileInGoAML(Int64 txnId, string txnType)
        {
            try
            {
                GoAMLModel goAMLModel = new GoAMLModel
                {
                    ActionName = GoAmlConstants.GoAmlExists,
                    TxnId = txnId,
                    TxnType = txnType
                };

                string? rs_out = await SqlService.GetReturnedJsonString(goAMLModel, Query);

                if (!string.IsNullOrEmpty(rs_out))
                {
                    var resultArray = JsonSerializer.Deserialize<JsonElement>(rs_out);

                    if (resultArray.ValueKind == JsonValueKind.Array && resultArray.GetArrayLength() > 0)
                    {
                        goAMLModel.GoAmlId = resultArray[0].GetProperty("id_goaml_key").GetInt64();
                        goAMLModel.TxnId = resultArray[0].GetProperty("id_txn_key").GetInt64();
                        goAMLModel.TxnType = resultArray[0].GetProperty("tx_txn_type").GetString() ?? "";
                        goAMLModel.ReportCode = resultArray[0].GetProperty("tx_report_code").GetString() ?? "";
                        goAMLModel.EntityReference = resultArray[0].GetProperty("tx_entity_reference").GetString() ?? "";
                        goAMLModel.CurrencyCodeLocal = resultArray[0].GetProperty("tx_currency_code_local").GetString() ?? "";
                        goAMLModel.ReportIndicator = resultArray[0].GetProperty("tx_report_indicator").GetString() ?? "";
                        goAMLModel.ReportReason = resultArray[0].GetProperty("tx_report_reason").GetString() ?? "";
                        goAMLModel.ActionTaken = resultArray[0].GetProperty("tx_action_taken").GetString() ?? "";
                        goAMLModel.Comments = resultArray[0].GetProperty("tx_comments").GetString() ?? "";
                    }
                }
                return goAMLModel;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        /**
         * inline documents
         * get remittance xml transaction data of person info from sp
         * 
         * @author: Mohiuddin
         * @since: 23/01/2025      
         */
        private async Task GetRemittanceXMLData(Int64[] txnIds, XmlModel model)
        {
            try
            {
                GoAMLModel goAMLModel = new GoAMLModel
                {
                    ActionName = GoAmlConstants.RemittanceXmlData,
                    TxnIdList = txnIds,
                };

                string? rs_out = await SqlService.GetReturnedJsonString(goAMLModel, Query);
                if (!string.IsNullOrEmpty(rs_out))
                {
                    List<Transaction>? resultArray = JsonSerializer.Deserialize<List<Transaction>>(rs_out);
                    if (resultArray != null)
                    {
                        model.Transaction = resultArray;
                        //resultArray.ForEach(transaction => transaction.TransactionLocation = transaction.TransactionLocation ?? "43 Mint Road (1st Floor) - Head Office");

                        foreach (var transaction in resultArray)
                        {
                            //transaction.TransactionLocation = transaction.TransactionLocation ?? "43 Mint Road (1st Floor) - Head Office";
                            transaction.TransactionLocation = transaction.TransactionLocation ?? "";

                            if (transaction?.FromMyClient?.FromPerson != null)
                            {
                                transaction.FromMyClient.FromPerson.IdNumber = transaction.FromMyClient.FromPerson.IdNumber ?? "";
                                transaction.FromMyClient.FromPerson.Nationality1 = transaction.FromMyClient.FromPerson.Nationality1 ?? "";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        /**
         * Get STRB Anti Money Laundering Monthly Summary Data or Report
         * 
         * @author: Mohiuddin
         * @since: 25/01/2025      
         */
        public async Task GetGeneratedAMLMonthlyReport(STR_Monthly_Report_Request_Data model)
        {
            try
            {
                DateTime fmDt = Convert.ToDateTime(model.FromDate + "-01");
                DateTime toDt = Convert.ToDateTime(model.FromDate + "-31");

                GoAMLModel goAMLModel = new GoAMLModel
                {
                    ActionName = GoAmlConstants.AmlMonthlyReport,
                    FromDate = fmDt,
                    ToDate = toDt,
                };

                string? rs_out = await SqlService.GetReturnedJsonString(goAMLModel, Query);

                var resultArray = JsonSerializer.Deserialize<JsonElement>(rs_out);

                if (!string.IsNullOrEmpty(rs_out))
                {
                    if (resultArray.ValueKind == JsonValueKind.Array && resultArray.GetArrayLength() > 0)
                    {
                        //result = resultArray[0].GetProperty("tx_entity_reference").GetString() ?? "";

                        model.Opt3 = resultArray[0].GetProperty("total_suspicious_txn").GetDecimal();
                        model.Opt4 = resultArray[0].GetProperty("total_rand").GetString();
                        model.Opt5 = resultArray[0].GetProperty("amount_involved").GetString();
                        model.AmlroName = resultArray[0].GetProperty("amlro_name").GetString();
                    }
                }

                //return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        /**
         * get reporting person info from sp
         * 
         * @author: Mohiuddin
         * @since: 23/01/2025      
         */
        private async Task<string> GetPreviousEntityRef(string reportCode, long[] txnId, string txnType)
        {
            try
            {
                GoAMLModel goAMLModel = new GoAMLModel
                {
                    ActionName = GoAmlConstants.PreviosEntityReference,
                    ReportCode = reportCode.Replace("_AMEND", ""),
                    TxnId = txnId[0],
                    TxnType = txnType
                };

                string? rs_out = await SqlService.GetReturnedJsonString(goAMLModel, Query);
                string? result = "";

                if (!string.IsNullOrEmpty(rs_out))
                {
                    var resultArray = JsonSerializer.Deserialize<JsonElement>(rs_out);

                    if (resultArray.ValueKind == JsonValueKind.Array && resultArray.GetArrayLength() > 0)
                    {
                        result = resultArray[0].GetProperty("tx_entity_reference").GetString() ?? "";

                        var jsnIdList = resultArray[0].GetProperty("jsn_id_list").EnumerateArray();
                        long[] jsnTxnIdList = jsnIdList.Select(id => id.GetInt64()).ToArray();
                    }
                }


                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        /**
         * get branch and location info from sp
         * 
         * @author: Mohiuddin
         * @since: 23/01/2025      
         */
        private async Task GetBranchAndLocationInfo(string branchCode, XmlModel model)
        {
            try
            {
                GoAMLModel goAMLModel = new GoAMLModel
                {
                    ActionName = GoAmlConstants.BranchAndLocation,
                    BranchCode = branchCode,
                };

                string? rs_out = await SqlService.GetReturnedJsonString(goAMLModel, Query);

                if (!string.IsNullOrEmpty(rs_out))
                {
                    dynamic? resultArray = JsonSerializer.Deserialize<dynamic>(rs_out);
                    if (resultArray is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in jsonElement.EnumerateArray())
                        {
                            model.RentityId = item.GetProperty("id_rentity_key").GetString();
                            model.RentityBranch = item.GetProperty("tx_rentity_branch").GetString();
                            model.SubmissionCode = item.GetProperty("tx_submission_code").GetString() ?? "";

                            var loc = item.GetProperty("loc");
                            model.Location = new Location();
                            model.Location.AddressType = loc.GetProperty("addressType").GetInt32();
                            model.Location.AddressLine = loc.GetProperty("address").GetString();
                            model.Location.Town = loc.GetProperty("town").GetString();
                            model.Location.City = loc.GetProperty("city").GetString();
                            model.Location.Zip = loc.GetProperty("zip").GetString();
                            model.Location.CountryCode = loc.GetProperty("countryCode").GetString();
                            model.Location.State = loc.GetProperty("state").GetString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        /**
         * get reporting person info from sp
         * 
         * @author: Mohiuddin
         * @since: 23/01/2025      
         */
        private async Task GetReportingPersonInfo(Int64? user_Id, XmlModel model)
        {
            try
            {
                GoAMLModel goAMLModel = new GoAMLModel
                {
                    ActionName = GoAmlConstants.ReportingPerson,
                    UserId = user_Id,
                };

                string? rs_out = await SqlService.GetReturnedJsonString(goAMLModel, Query);

                if (!string.IsNullOrEmpty(rs_out))
                {
                    var resultArray = JsonSerializer.Deserialize<List<JsonReportingPersonArrayModel>>(rs_out);
                    if (resultArray != null)
                    {
                        model.ReportingPerson = resultArray?[0].ReportingPerson;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        #endregion

        #region controller_call            

        /**
         * Get XML Bytes of Requested Data
         * 
         * @author: Mohiuddin
         * @since: 01/02/2025      
         */
        public async Task<byte[]> GetXmlBytesOfRequestedData(XmlModel? report)
        {
            try
            {
                // Serialize the CTR object to a memory stream
                byte[] xmlBytes;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (StreamWriter writer = new StreamWriter(memoryStream))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(XmlModel));
                        serializer.Serialize(writer, report);
                        await writer.FlushAsync();
                        memoryStream.Position = 0; // Reset the memory stream position to the beginning
                        xmlBytes = memoryStream.ToArray();
                    }
                }

                return xmlBytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        /**
        * Get Remittance Sarb Data from t_remittance_sarb_data table
        * 
        * @author: Mohiuddin
        * @since: 01/01/2025      
         */
        public async Task<(XmlModel?, GoAMLModel?)> GetRemittanceSarbData(XMLReportRequest request)
        {
            var txnIds = request.Payload.Transactions.Select(t => t.Id).ToArray();

            XmlModel data = new XmlModel();
            data.ReportCode = request.Payload.ReportCode.Replace("_AMEND", "");
            data.CurrencyCodeLocal = request.Payload.CurrencyCodeLocal;
            data.SubmissionDate = DateTime.Now;

            //this function calls will set the node wise xml information into the data object
            await GetBranchAndLocationInfo(request.Payload.BranchCode, data);
            await GetReportingPersonInfo(request.Header.UserModifidId, data);
            await GetRemittanceXMLData(txnIds, data);

            //get previous entity reference number if exist
            string previousEntityRef = await GetPreviousEntityRef(request.Payload.ReportCode, txnIds, request.Payload.TxnType);
            if (!string.IsNullOrEmpty(previousEntityRef))
            {
                data.EntityReference = data.ReportCode + "_AMEND_" + DateTime.Now.ToString("yyyyMMddHHmmssff");
                data.FiuRefNumber = previousEntityRef;
            }
            else
            {
                data.EntityReference = data.ReportCode + "_" + DateTime.Now.ToString("yyyyMMddHHmmssff");
            }

            data.ReportIndicators.Add(GoAmlConstants.GetReportIndicator(data.ReportCode));

            //keeping set goAml table data for insert/update on success xml file validation from controller          
            GoAMLModel goAMLModel = new GoAMLModel()
            {
                TxnIdList = txnIds,
                TxnType = request.Payload.TxnType,
                ReportCode = data.ReportCode,
                EntityReference = data.EntityReference,
                CurrencyCodeLocal = request.Payload.CurrencyCodeLocal,
                ReportIndicator = data.ReportIndicators.First(),
                ReportReason = request.Payload.ReportReason,
                ActionTaken = request.Payload.ActionTaken,
                Comments = request.Payload.Comments,
                UserModifiedId = request.Header.UserModifidId
            };

            return (data, goAMLModel);
        }

        /**
        * get remmittance data for view page
        * 
        * @author: Mohiuddin
        * @since: 04/02/2025     
        */
        public async Task<dynamic?> GetRemittanceViewData(RemittanceViewDataRequest? model)
        {
            try
            {
                string? rs_out = await SqlService.GetReturnedJsonString(model?.Payload, Query);

                if (!string.IsNullOrEmpty(rs_out))
                {
                    var result = new
                    {
                        header = new
                        {
                            UserId = model?.Header?.UserModifidId,
                            model?.Header?.ActionName,
                            model?.Header?.ServiceName
                        },
                        payload = JsonSerializer.Deserialize<dynamic>(rs_out)
                    };

                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        /**
        * get remmittance data for view page
        * 
        * @author: Mohiuddin
        * @since: 08/02/2025     
        */
        public async Task<dynamic?> GetBranchViewData(BranchRequest? model)
        {
            try
            {
                string? rs_out = await SqlService.GetReturnedJsonString(model?.Payload, Query);
                dynamic? resultArray = JsonSerializer.Deserialize<dynamic>(rs_out);

                var result = new BranchResponse()
                {
                    Header = new BranchHeaderResponse
                    {
                        UserId = model?.Header?.UserModifidId,
                        ActionName = model?.Header?.ActionName,
                        ServiceName = model?.Header?.ServiceName
                    },
                    Payload = new List<BranchPayloadResponse>()
                };

                if (!string.IsNullOrEmpty(rs_out))
                {
                    if (resultArray is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in jsonElement.EnumerateArray())
                        {
                            result?.Payload?.Add(new BranchPayloadResponse
                            {
                                BranchCode = item.GetProperty("branch_code").GetString(),
                                BranchName = item.GetProperty("branch_name").GetString()
                            });
                        }
                    }

                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        #endregion

        #region ins_upd_db_operation

        /**
         * inline documents
         * insert into GoAml table from remittance sarb data on xml file request
         * 
         * @author: Mohiuddin
         * @since: 25/01/2025      
         */
        public async Task<int> InsertGoAML(GoAMLModel model)
        {
            try
            {
                model.ActionName = GoAmlConstants.Insert;

                string? rs_out = await SqlService.GetReturnedJsonString(model, Query);

                if (!string.IsNullOrEmpty(rs_out))
                {
                    var resultArray = JsonSerializer.Deserialize<dynamic>(rs_out);
                    return resultArray;
                }

                return 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /**
         * inline documents
         * update GoAml table from remittance sarb data on xml file request
         * 
         * @author: Mohiuddin
         * @since: 25/01/2025      
         */
        public async Task<int> UpdateGoAML(GoAMLModel model)
        {
            try
            {
                model.ActionName = GoAmlConstants.Update;

                string? rs_out = await SqlService.GetReturnedJsonString(model, Query);

                if (!string.IsNullOrEmpty(rs_out))
                {
                    var resultArray = JsonSerializer.Deserialize<dynamic>(rs_out);
                }

                return 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region report_generation

        /**
         * inline documents
         * Action - Export Report Data to a Report in File Format
         * 
         * @author: Mohiuddin
         * @since: 01/01/2025      
         */
        public async Task<byte[]> ExportReportFile(string ReportPath, string FileName, string FileType = "Excel",
                                                    string ContentType = "application/vnd.ms-excel", string reportName = "",
                                                    Hashtable? parameterList = null, STR_Monthly_Report_Request_Body? data = null)
        {
            try
            {
                LocalReport localReport = new LocalReport();
                localReport.ReportPath = ReportPath;
                if (parameterList != null)
                {
                    List<ReportParameter> parameters = new List<ReportParameter>();
                    foreach (DictionaryEntry item in parameterList)
                    {
                        ReportParameter param = new ReportParameter(item.Key.ToString(), item.Value?.ToString(), false);
                        parameters.Add(param);
                    }
                    localReport.SetParameters(parameters);
                }

                if (data?.Payload != null)
                {
                    DataTable dataTable = await ReportHelper.GetDataTableFromListAsync(data.Payload, new ReportDS.STR_Monthly_Report_Data_TableDataTable());
                    if (dataTable != null)
                        localReport.DataSources.Add(new ReportDataSource("ReportDS", dataTable));
                }

                string reportType = FileType, mimeType, encoding, fileNameExtension = ".pdf";
                string deviceInfo = "<DeviceInfo>" +
                "  <OutputFormat>" + FileType + "</OutputFormat>" +
                "</DeviceInfo>";

                Warning[] warnings;
                string[] streams;
                byte[] renderedBytes = localReport.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);


                return renderedBytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        #endregion


    }
}