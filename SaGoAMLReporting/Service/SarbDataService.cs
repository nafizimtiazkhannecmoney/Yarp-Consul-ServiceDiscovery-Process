using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Reporting.NETCore;
using Microsoft.ReportingServices.Diagnostics.Internal;
using Newtonsoft.Json.Serialization;
using Npgsql;
using SaGoAMLReporting.Constants;
using SaGoAMLReporting.Files.Report.DataSet;
using SaGoAMLReporting.Helper;
using SaGoAMLReporting.Model;
using SaGoAMLReporting.Service.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SaGoAMLReporting.Service
{
    public class SarbDataService : ISarbDataService
    {
        private ISqlService SqlService { get; set; }

        public SarbDataService(ISqlService SqlService)
        {
            this.SqlService = SqlService;
        }

        #region get_data_from_db   

        /**
         * inline documents
         * Get Remittence sarb data list from db
         * 
         * @author: Mohiuddin
         * @since: 11/02/2025      
         */
        public async Task<dynamic?> GetRemittanceSarbViewData(RemittanceSarbDataRequest? model)
        {
            try
            {
                if (model?.Payload != null)
                    model.Payload.ActionName = GoAmlConstants.Select;

                string? rs_out = await SqlService.GetReturnedJsonString(model?.Payload, GoAmlConstants.ActRemittanceSarbData);

                if (!string.IsNullOrEmpty(rs_out))
                {
                    var result = new
                    {
                        Header = new
                        {
                            UserId = model?.Header?.UserModifidId,
                            ActionName = model?.Header?.ActionName,
                            ServiceName = model?.Header?.ServiceName
                        },
                        Payload = JsonSerializer.Deserialize<dynamic>(rs_out)
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
         * inline documents
         * Get Transaction sarb data list from db
         * 
         * @author: Mohiuddin
         * @since: 12/02/2025      
         */
        public async Task<dynamic?> GetTransactionSarbViewData(RemittanceSarbDataRequest? model)
        {
            try
            {
                string? rs_out = await SqlService.GetReturnedJsonString(model?.Payload, GoAmlConstants.ActTransactionSarbData);

                if (!string.IsNullOrEmpty(rs_out))
                {
                    var result = new
                    {
                        Header = new
                        {
                            UserId = model?.Header?.UserModifidId,
                            ActionName = model?.Header?.ActionName,
                            ServiceName = model?.Header?.ServiceName
                        },
                        Payload = JsonSerializer.Deserialize<dynamic>(rs_out)
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
         * inline documents
         * Get Rejected sarb data list from db
         * 
         * @author: Mohiuddin
         * @since: 12/02/2025      
         */
        public async Task<dynamic?> GetRejectedSarbViewData(RemittanceSarbDataRequest? model)
        {
            try
            {
                string? rs_out = await SqlService.GetReturnedJsonString(model?.Payload, GoAmlConstants.ActTransactionSarbData);

                if (!string.IsNullOrEmpty(rs_out))
                {
                    var result = new
                    {
                        Header = new
                        {
                            UserId = model?.Header?.UserModifidId,
                            ActionName = model?.Header?.ActionName,
                            ServiceName = model?.Header?.ServiceName
                        },
                        Payload = JsonSerializer.Deserialize<dynamic>(rs_out)
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
         * inline documents
         * Get sarb status view data list from db
         * 
         * @author: Mohiuddin
         * @since: 12/02/2025      
         */
        public async Task<dynamic?> GetSarbStatusViewData(RemittanceSarbDataRequest? model)
        {
            try
            {
                string? rs_out = await SqlService.GetReturnedJsonString(model?.Payload, GoAmlConstants.ActTransactionSarbData);

                if (!string.IsNullOrEmpty(rs_out))
                {
                    SarbStatusDataModel? data = JsonSerializer.Deserialize<List<SarbStatusDataModel>>(rs_out)?.FirstOrDefault();                   

                    var jsn_ref_issue_date = data?.jsn_ref_issue_date ?? new Dictionary<string, DateTime?>();
                    var jsn_ref_accept_date = data?.jsn_ref_accept_date ?? new Dictionary<string, DateTime?>();
                    var jsn_ref_upload_date = data?.jsn_ref_upload_date ?? new Dictionary<string, DateTime?>();

                    var result = new
                    {
                        Header = new
                        {
                            UserId = model?.Header?.UserModifidId,
                            model?.Header?.ActionName,
                            model?.Header?.ServiceName
                        },
                        Payload = (from a in jsn_ref_issue_date
                                  join b in jsn_ref_accept_date on a.Key equals b.Key into b1
                                  from b in b1.DefaultIfEmpty()
                                  join c in jsn_ref_upload_date on a.Key equals c.Key into c1
                                  from c in c1.DefaultIfEmpty()
                                  select new
                                  {
                                      id = new Random().Next(int.MaxValue),
                                      txnDate = a.Value?.ToString("yyyy-MM-dd") ?? "",
                                      referenceNo = a.Key ?? "",
                                      totalTxn = data?.total_txn,
                                      uploadDate = c.Value?.ToString("yyyy-MM-dd") ?? "",
                                      acceptedDate = b.Value?.ToString("yyyy-MM-dd") ?? "",
                                  }).ToList()
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
         * inline documents
         * Get Transaction sarb data list from db
         * 
         * @author: Mohiuddin
         * @since: 12/02/2025      
         */
        public async Task<dynamic?> GetRemittanceOutwardViewData(DateTime from, DateTime to)
        {
            try
            {
                List<RemittanceOutwardViewDataModel> dataList = new List<RemittanceOutwardViewDataModel>();
                string query = @"SELECT * FROM dbo.V_REMITTANCE_OUTWARD WHERE CAST(txnDate AS DATE) >= @from 
			                     AND CAST(txnDate AS DATE) <= @to ORDER BY txnId ASC";

                using (SqlConnection connection = new SqlConnection(SqlService.GetSqlServConnString()))
                {
                    connection.Open();

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@from ", from);
                        command.Parameters.AddWithValue("@to", to);                        

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                RemittanceOutwardViewDataModel data = new RemittanceOutwardViewDataModel()
                                {
                                    Id = reader["id"] as long?,
                                    TxnId = reader["txnId"] as long?,
                                    ReportingQualifier = reader["reportingQualifier"] as string,
                                    Flow = reader["flow"] as string,
                                    SequenceNo = reader["sequenceNo"] as int?,
                                    Mtai = reader["mtai"] as string,
                                    RulingSection = reader["rulingSection"] as string,
                                    OriginatingCountry = reader["originatingCountry"] as string,
                                    CustomerAccIdentifier = reader["customerAccIdentifier"] as string,
                                    ReceivingBank = reader["receivingBank"] as string,
                                    TxnDate = reader["txnDate"] as DateTime?,
                                    RemitPrefix = reader["remitPrefix"] as string,
                                    OrigTxnRefNo = reader["origTxnRefNo"] as string,
                                    ReferenceNo = reader["referenceNo"] as string,
                                    BranchCode = reader["branchCode"] as string,
                                    BranchName = reader["branchName"] as string,
                                    OriginatingBank = reader["originatingBank"] as string,
                                    ReceivingCountry = reader["receivingCountry"] as string,
                                    BeneficiaryId = reader["beneficiaryId"] as long?,
                                    BeneficiaryName = reader["beneficiaryName"] as string,
                                    BeneficiarySurname = reader["beneficiarySurname"] as string,
                                    BeneficiaryAccIdentifier = reader["beneficiaryAccIdentifier"] as string,
                                    BeneficiaryAccNo = reader["beneficiaryAccNo"] as string,
                                    BeneficiaryCountry = reader["beneficiaryCountry"] as string,
                                    BeneficiaryCurrencyCode = reader["beneficiaryCurrencyCode"] as string,
                                    BeneficiaryAmount = reader["beneficiaryAmount"] as string,
                                    CustomerId = reader["customerId"] as long?,
                                    CustomerName = reader["customerName"] as string,
                                    CustomerSurname = reader["customerSurname"] as string,
                                    CustomerGender = reader["customerGender"] as string,
                                    CustomerDob = reader["customerDob"] as DateTime?,
                                    CustomerIdentityNo = reader["customerIdentityNo"] as string,
                                    CustomerPassportNo = reader["customerPassportNo"] as DateTime?,
                                    CustomerPassportCountry = reader["customerPassportCountry"] as string,
                                    CustomerAddress = reader["customerAddress"] as string,
                                    CustomerSuburb = reader["customerSuburb"] as string,
                                    CustomerAccNo = reader["customerAccNo"] as string,
                                    CustomerCity = reader["customerCity"] as string,
                                    CustomerProvince = reader["customerProvince"] as string,
                                    CustomerPostalCode = reader["customerPostalCode"] as string,
                                    CustomerPhone = reader["customerPhone"] as string,
                                    CustomerAmount = reader["customerAmount"] as string,
                                    BopCategory = reader["bopCategory"] as string,
                                    LocationCountry = reader["locationCountry"] as string,
                                    ExportedDate = reader["exportedDate"] as DateTime?,
                                    SarbEnvironment = reader["sarbEnvironment"] as string
                                };


                                if (data.TxnId != null)
                                {
                                    var result = await InsertRemittanceSarbData(data);
                                }
                                
                                dataList.Add(data);
                            }
                        }
                    }
                }
                return dataList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        /**
         * inline documents
         * Get Remittence sarb data list from db
         * 
         * @author: Mohiuddin
         * @since: 11/02/2025      
         */
        public async Task<dynamic?> GetTransactionSarbExcelData(RemittanceSarbDataRequest? model)
        {
            try
            {
                string? rs_out = await SqlService.GetReturnedJsonString(model?.Payload, GoAmlConstants.ActTransactionSarbData);

                if (!string.IsNullOrEmpty(rs_out))
                {
                    var result = new
                    {
                        Header = new
                        {
                            UserId = model?.Header?.UserModifidId,
                            ActionName = model?.Header?.ActionName,
                            ServiceName = model?.Header?.ServiceName
                        },
                        Payload = JsonSerializer.Deserialize<List<SarbExcelDataModel>>(rs_out)
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

        #endregion


        #region controller_call            

        #endregion


        #region ins_upd_db_operation        

        /**
         * inline documents
         * insert into GoAml table from remittance sarb data on xml file request
         * 
         * @author: Mohiuddin
         * @since: 25/01/2025      
         */
        public async Task<int> InsertRemittanceSarbData(RemittanceOutwardViewDataModel model)
        {
            try
            {
                string query = GoAmlConstants.ActRemittanceSarbData;
                model.ActionName = GoAmlConstants.Insert;

                string? rs_out = await SqlService.GetReturnedJsonString(model, query);

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

        #endregion


        #region report_generation   

        /**
         * inline documents
         * Get Remittence sarb data list from db
         * 
         * @author: Mohiuddin
         * @since: 17/02/2025      
         */
        public string GenerateSarbStatusTimelineCSV(dynamic dataList)
        {
            try
            {
                var sb = new StringBuilder();

                // Write headers
                sb.AppendLine("Reference. No,Transaction Date,Uploaded Date,Accepted Date");

                // Write data
                foreach (var data in dataList)
                {
                    sb.AppendLine($"{data.referenceNo},{data.txnDate},{data.uploadDate},{data.acceptedDate}");
                }

                return sb.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /**
         * inline documents
         * Get Remittence sarb data list from db
         * 
         * @author: Mohiuddin
         * @since: 17/02/2025      
         */
        public string GenerateTransactionSarbCSV(List<SarbExcelDataModel> dataList)
        {
            try
            {
                var sb = new StringBuilder();

                // Write headers
                sb.AppendLine("Ref. No,Txn. Date,Name,Amount,Ben. Name,Ben. (Amt),Category,Status,File,Id No.,Flow,Remit No.,Branch Code,Address,Suburb,City,Province,Postal Code,Phone");

                // Write data
                for (int i = 0; i < dataList?.Count; i++)
                {
                    for (int j = 0; j < dataList[i].data_row.Count(); j++)
                    {
                        sb.AppendLine($"{dataList[i].data_row[0]},"  +
                                      $"{dataList[i].data_row[1]},"  +
                                      $"{dataList[i].data_row[2]},"  +
                                      $"{dataList[i].data_row[3]},"  +
                                      $"{dataList[i].data_row[4]},"  +
                                      $"{dataList[i].data_row[5]},"  +
                                      $"{dataList[i].data_row[6]},"  +
                                      $"{dataList[i].data_row[7]},"  +
                                      $"{dataList[i].data_row[8]},"  +
                                      $"{dataList[i].data_row[9]},"  +
                                      $"{dataList[i].data_row[10]}," +
                                      $"{dataList[i].data_row[11]}," +
                                      $"{dataList[i].data_row[12]}," +
                                      $"{dataList[i].data_row[13]}," +
                                      $"{dataList[i].data_row[14]}," +
                                      $"{dataList[i].data_row[15]}," +
                                      $"{dataList[i].data_row[16]}," +
                                      $"{dataList[i].data_row[17]}," +
                                      $"{dataList[i].data_row[18]}");
                    }
                }

                return sb.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        
        #endregion

    }
}