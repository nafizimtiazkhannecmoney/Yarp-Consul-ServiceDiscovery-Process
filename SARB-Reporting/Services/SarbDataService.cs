using Nec.Web.Utils;
using NecCore.services;
using SARB_Reporting.Helpers;
using SARB_Reporting.Models;
using SARB_Reporting.Models.ApiResponse;
using SARB_Reporting.Models.Cancel;
using SARB_Reporting.Models.Refund;
using SARB_Reporting.Models.Regular;
using SARB_Reporting.Models.Replacement;
using SARB_Reporting.Utils;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;


namespace SARB_Reporting.Services
{
    public class SarbDataService : PgSqlService
    {
        DbContext? _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly NecAppConfig _appConfig;
        public SarbDataService(DbContext dbContext, IConfiguration configuration, IHostEnvironment hostEnvironment,NecAppConfig appConfig)
        {
            _appConfig = appConfig;
            _dbContext = dbContext;
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
        }


        /**
        * Description : Save sarb data 
        * @since      : 11/01/2025      
        */
        public async Task<bool> SaveRegular(List<AddSarbeModel> originalTransaction)
        {
            try
            {
                const string GET_TYPE = "ins_update_sarb";
                var json = JsonSerializer.Serialize(originalTransaction);
                var resData = CallStoreProcedure(_dbContext!.ConnectionString!, GET_TYPE, json);
                DatabaseResponse model = JsonSerializer.Deserialize<DatabaseResponse>(resData)!;

                if (model.Success)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        /**
        * Description : Convert C# class to xml model 
        * @author     : Rakibul   
        * @since      : 20/01/2025      
        */
        public SARBDEXEnvelopeCommon<BodyContent> CovertXMLModel(List<SaReporting.OriginalTransaction> originalTransactions)
        {
            string generatedNumber = GetFileName();  // generate file name
            int count = 1;
            var envelope = new SARBDEXEnvelopeCommon<BodyContent>();
            var userName = _configuration.GetSection("GetUrl").GetValue<string>("Username");
            var password = _configuration.GetSection("GetUrl").GetValue<string>("Pwd");
            List<Models.Regular.OriginalTransaction> originals = new List<Models.Regular.OriginalTransaction>();

            foreach (var item in originalTransactions)
            {
                Models.Regular.OriginalTransaction original = new Models.Regular.OriginalTransaction();
                original.BranchCode = item.BranchCode;
                original.BranchName = item.BranchName;
                original.ReportingQualifier = item.ReportingQualifier;
                original.TrnReference = item.TrnReference;
                original.OriginatingBank = item.OriginatingBank;
                original.ReceivingBank = item.ReceivingBank;
                original.Flow = item.Flow;
                original.LineNumber = count++;
                original.ReplacementTransaction = item.ReplacementTransaction;
                original.Date = item.Date;
                original.OriginatingCountry = item.OriginatingCountry;
                original.TotalValue = item.TotalValue;
                original.ReceivingCountry = item.ReceivingCountry;
                original.NonResident = new Models.Regular.NonResident
                {
                    Individual = new Models.Regular.Individual
                    {
                        Surname = item.NonResident.Individual.Surname,
                        Name = item.NonResident.Individual.Name,
                        AdditionalNonResidentData = new Models.Regular.AdditionalNonResidentData
                        {
                            AccountIdentifier = item.NonResident.Individual.AdditionalNonResidentData.AccountIdentifier,
                            Country = item.NonResident.Individual.AdditionalNonResidentData.Country,
                            AccountNumber = item.NonResident.Individual.AdditionalNonResidentData.AccountIdentifier == "CASH"
                                    ? null
                                    : item.NonResident.Individual.AdditionalNonResidentData.AccountNumber
                        },
                    }
                };
                original.ResidentCustomerAccountHolder = new Models.Regular.ResidentCustomerAccountHolder
                {
                    IndividualCustomer = new Models.Regular.IndividualCustomer
                    {
                        Surname = item.ResidentCustomerAccountHolder.IndividualCustomer.Surname,
                        Name = item.ResidentCustomerAccountHolder.IndividualCustomer.Name,
                        Gender = item.ResidentCustomerAccountHolder.IndividualCustomer.Gender,
                        DateOfBirth = item.ResidentCustomerAccountHolder.IndividualCustomer.DateOfBirth,
                        TempResPermitNumber = item.MonetaryDetails.BoPCategory=="401" ? null: item.ResidentCustomerAccountHolder.IndividualCustomer.IDNumber,
                        IDNumber = item.MonetaryDetails.BoPCategory=="401" ? item.ResidentCustomerAccountHolder.IndividualCustomer.IDNumber: null,
                        PassportNumber = item.ResidentCustomerAccountHolder.IndividualCustomer.PassportNumber,
                        PassportCountry = item.ResidentCustomerAccountHolder.IndividualCustomer.PassportCountry,
                        AdditionalCustomerData = new Models.Regular.AdditionalCustomerData
                        {
                            AccountIdentifier = item.ResidentCustomerAccountHolder.IndividualCustomer.AdditionalCustomerData.AccountIdentifier,
                            StreetAddressLine1 = item.ResidentCustomerAccountHolder.IndividualCustomer.AdditionalCustomerData.StreetAddressLine1,
                            StreetSuburb = item.ResidentCustomerAccountHolder.IndividualCustomer.AdditionalCustomerData.StreetSuburb,
                            StreetCity = item.ResidentCustomerAccountHolder.IndividualCustomer.AdditionalCustomerData.StreetCity,
                            StreetProvince = item.ResidentCustomerAccountHolder.IndividualCustomer.AdditionalCustomerData.StreetProvince,
                            StreetPostalCode = item.ResidentCustomerAccountHolder.IndividualCustomer.AdditionalCustomerData.StreetPostalCode,
                            PostalAddressLine1 = item.ResidentCustomerAccountHolder.IndividualCustomer.AdditionalCustomerData.PostalAddressLine1,
                            PostalSuburb = item.ResidentCustomerAccountHolder.IndividualCustomer.AdditionalCustomerData.PostalSuburb,
                            PostalCity = item.ResidentCustomerAccountHolder.IndividualCustomer.AdditionalCustomerData.PostalCity,
                            PostalProvince = item.ResidentCustomerAccountHolder.IndividualCustomer.AdditionalCustomerData.PostalProvince,
                            PostalCode = item.ResidentCustomerAccountHolder.IndividualCustomer.AdditionalCustomerData.PostalCode,
                            ContactSurname = item.ResidentCustomerAccountHolder.IndividualCustomer.AdditionalCustomerData.ContactSurname,
                            ContactName = item.ResidentCustomerAccountHolder.IndividualCustomer.AdditionalCustomerData.ContactName,
                            Telephone = item.ResidentCustomerAccountHolder.IndividualCustomer.AdditionalCustomerData.Telephone
                        }
                    }
                };
                original.MonetaryDetails = new Models.Regular.MonetaryDetails
                {
                    SequenceNumber = item.MonetaryDetails.SequenceNumber.ToString(),
                    MoneyTransferAgentIndicator = item.MonetaryDetails.MoneyTransferAgentIndicator,
                    RandValue = item.MonetaryDetails.RandValue.ToString(),
                    ForeignValue = item.MonetaryDetails.ForeignValue.ToString(),
                    ForeignCurrencyCode = item.MonetaryDetails.ForeignCurrencyCode,
                    BoPCategory = item.MonetaryDetails.BoPCategory,
                    RulingsSection = item.MonetaryDetails.RulingsSection,
                    LocationCountry = item.MonetaryDetails.LocationCountry,
                    LegalEntityThirdPartyName = item.MonetaryDetails.LegalEntityThirdPartyName,
                    ThirdPartyContactSurname = item.MonetaryDetails.ThirdPartyContactSurname,
                    ThirdPartyContactName = item.MonetaryDetails.ThirdPartyContactName,
                    ThirdPartyTelephone = item.MonetaryDetails.ThirdPartyTelephone
                };
                originals.Add(original);
            }
            BodyContent bodyContent = new BodyContent();
            bodyContent.Version = 1;
            bodyContent.Reference = generatedNumber.Replace(".xml", ""); 
            bodyContent.Environment = _hostEnvironment.EnvironmentName == "Production" ? "P" : "T"; 
            bodyContent.OriginalTransactions = originals;
            envelope.Header!.Sender = userName!;
            envelope.Header.Identity = password!;
            envelope.Header.SentAt = DateTimeOffset.Now;
            envelope.Header.Reference = generatedNumber.Replace(".xml", ""); 
            envelope.Header.Recipient = "SARB";
            envelope.Header.IdentityType = 1;
            envelope.Header.Type = "FINSURV";
            envelope.Header.Version = 1;
            envelope.Body!.Content = bodyContent;
            return envelope;
        }


        /**
        * Description : Convert Regular XML    
        * @author     : Rakibul   
        * @since      : 20/01/2025      
        */
        public List<SaReporting.OriginalTransaction> GetSarbDataForRegularByTrnReference(List<string> refrenceList)
        {
            string connectionString = _dbContext.ConnectionString ?? throw new Exception("not get connection string");
            const string GET_TYPE = "getbyReference_sarb";
            var jsonValue = JsonSerializer.Serialize(refrenceList);
            var resData = CallStoreProcedure(connectionString, GET_TYPE, jsonValue);
            DatabaseResponse model = JsonSerializer.Deserialize<DatabaseResponse>(resData)!;
            List<RemittanceSarbData> remittanceSarbData = JsonSerializer.Deserialize<List<RemittanceSarbData>>(model.Data)!;
            Dictionary<string,string> preferencesDict = GetPreferenceMapping();
            List<SaReporting.OriginalTransaction> originalTransactions = new List<SaReporting.OriginalTransaction>();

            foreach (var item in remittanceSarbData)
            {
                SaReporting.OriginalTransaction transaction = new SaReporting.OriginalTransaction();
                transaction.BranchCode = item.TxBranchCode;
                transaction.BranchName = item.TxBranchName;
                transaction.ReportingQualifier = item.TxReportingQualifier;
                transaction.TrnReference = item.TxReferenceNo;
                transaction.Date = Convert.ToDateTime(item.DtTxnDate);
                transaction.OriginatingBank = item.TxOriginatingBank;
                transaction.OriginatingCountry = item.TxOriginatingCountry;
                transaction.TotalValue = Convert.ToDouble(item.DecBenAmount) + Convert.ToDouble(item.DecCustomerAmount);
                transaction.Flow = item.TxFlow;
                transaction.ReceivingBank = item.TxReceivingBank;
                transaction.ReceivingCountry = item.TxReceivingCountry;
                transaction.NonResident.Individual.Surname = item.TxBenSurname;
                transaction.NonResident.Individual.Name = item.TxBenSurname;
                transaction.NonResident.Individual.AdditionalNonResidentData.AccountIdentifier = item.TxBenAccIdentifier;
                transaction.NonResident.Individual.AdditionalNonResidentData.AccountNumber = item.TxBenAccNo;
                transaction.NonResident.Individual.AdditionalNonResidentData.Country = item.TxBenCountry;
                transaction.ResidentCustomerAccountHolder.IndividualCustomer.Surname = item.TxCustomerSurname;
                transaction.ResidentCustomerAccountHolder.IndividualCustomer.Name = item.TxCustomerName;
                transaction.ResidentCustomerAccountHolder.IndividualCustomer.Gender = item.TxCustomerGender;
                transaction.ResidentCustomerAccountHolder.IndividualCustomer.DateOfBirth = Convert.ToDateTime(item.DtCustomerDob);
                transaction.ResidentCustomerAccountHolder.IndividualCustomer.PassportNumber = item.TxCustomerPassportNo;
                transaction.ResidentCustomerAccountHolder.IndividualCustomer.PassportCountry = item.TxCustomerPassportCountry;
                transaction.ResidentCustomerAccountHolder.IndividualCustomer.IDNumber = item.TxCustomerIdentityNo;
                transaction.ResidentCustomerAccountHolder.IndividualCustomer.AdditionalCustomerData.AccountIdentifier = item.TxCustomerAccIdentifier;
    
                transaction.ResidentCustomerAccountHolder.IndividualCustomer.AdditionalCustomerData.StreetAddressLine1 = item.TxCustomerAddress;
                transaction.ResidentCustomerAccountHolder.IndividualCustomer.AdditionalCustomerData.StreetSuburb = item.TxCustomerSuburb;
                transaction.ResidentCustomerAccountHolder.IndividualCustomer.AdditionalCustomerData.StreetCity = item.TxCustomerCity;
                transaction.ResidentCustomerAccountHolder.IndividualCustomer.AdditionalCustomerData.StreetProvince = item.TxCustomerProvince;
                transaction.ResidentCustomerAccountHolder.IndividualCustomer.AdditionalCustomerData.StreetPostalCode = item.TxCustomerPostalCode;

                transaction.ResidentCustomerAccountHolder.IndividualCustomer.AdditionalCustomerData.PostalAddressLine1 = item.TxCustomerAddress;
                transaction.ResidentCustomerAccountHolder.IndividualCustomer.AdditionalCustomerData.PostalSuburb = item.TxCustomerSuburb;
                transaction.ResidentCustomerAccountHolder.IndividualCustomer.AdditionalCustomerData.PostalCity = item.TxCustomerCity;
                transaction.ResidentCustomerAccountHolder.IndividualCustomer.AdditionalCustomerData.PostalProvince = item.TxCustomerProvince;
                transaction.ResidentCustomerAccountHolder.IndividualCustomer.AdditionalCustomerData.PostalCode = item.TxCustomerPostalCode;

                transaction.ResidentCustomerAccountHolder.IndividualCustomer.AdditionalCustomerData.ContactSurname = item.TxCustomerSurname;
                transaction.ResidentCustomerAccountHolder.IndividualCustomer.AdditionalCustomerData.ContactName = item.TxCustomerName;
                transaction.ResidentCustomerAccountHolder.IndividualCustomer.AdditionalCustomerData.Telephone = item.TxCustomerPhone;
                transaction.MonetaryDetails.SequenceNumber = Convert.ToInt32(item.TxSequenceNo);  
                transaction.MonetaryDetails.MoneyTransferAgentIndicator = "ADLA";
                transaction.MonetaryDetails.RandValue = Convert.ToDouble(item.DecCustomerAmount);
                transaction.MonetaryDetails.ForeignValue = Convert.ToDouble(item.DecBenAmount);
                transaction.MonetaryDetails.ForeignCurrencyCode = item.TxBenCurrencyCode;
                transaction.MonetaryDetails.BoPCategory = item.TxBopCategory;
                transaction.MonetaryDetails.RulingsSection = item.TxRulingSection;
                transaction.MonetaryDetails.LocationCountry = item.TxLocationCountry;
                if (item.TxMTAI == "ADLA|S2S")
                {
                    transaction.MonetaryDetails.LegalEntityThirdPartyName = preferencesDict["LETP_NAME"];
                    transaction.MonetaryDetails.ThirdPartyTelephone = preferencesDict["TP_TELEPHONE"];
                    transaction.MonetaryDetails.ThirdPartyContactName = preferencesDict["TPC_NAME"];
                    transaction.MonetaryDetails.ThirdPartyContactSurname = preferencesDict["TPC_SURNAME"];
                }
                else
                {
                    transaction.MonetaryDetails.LegalEntityThirdPartyName = null;
                    transaction.MonetaryDetails.ThirdPartyTelephone = null;
                    transaction.MonetaryDetails.ThirdPartyContactName = null;
                    transaction.MonetaryDetails.ThirdPartyContactSurname =null;
                }
                transaction.ReplacementTransaction = "N";
                originalTransactions.Add(transaction);
            }

            return originalTransactions;
        }


        /**
        * Description : Cancel Sarb Request 
        * @author     : Rakibul
        * @since      : 25/01/2025      
        */
        public async Task<bool> CancelSarbRequest(string refrence)
        {
            string filename = GetFileName();
            var result = GetSarbData(refrence);
            var regularData = ConvertXmlCancel(result,filename);
            var file = Helper.FileSave(regularData, _appConfig.CANCELFilelocation!);
            var saveFile = await UploadFile(file.FileLocation!, file.FileName!); 
            if (!string.IsNullOrWhiteSpace(file.FileLocation))
            {
                string fie=GetFile(file.FileLocation);
                List<AddSarbeModel> addSarbeModels = new List<AddSarbeModel>();
                AddSarbeModel ot = new AddSarbeModel()
                {
                    tx_file_name = filename,
                    tx_txn_ref_no = refrence,
                    tx_warning_code = fie,
                    tx_response = saveFile.Message
                };
                addSarbeModels.Add(ot);
                var res=await SaveRegular(addSarbeModels);
                return res;
            }
            else
            {
                return false;
            }
        }


        /**
        * Description : Convert Sarb data to xml 
        * @author     : Rakibul
        * @since      : 25/01/2025      
        */
        private SARBDEXEnvelopeCommon<FinsurvCancel> ConvertXmlCancel(RemittanceSarbData sarbData,string fileName)
        {
            try
            {
                var userName = _configuration.GetSection("GetUrl").GetValue<string>("Username");
                var password = _configuration.GetSection("GetUrl").GetValue<string>("Pwd");
                SARBDEXEnvelopeCommon<FinsurvCancel> envelope = new SARBDEXEnvelopeCommon<FinsurvCancel>();
                FinsurvCancel finsurvCancel = new FinsurvCancel();
                finsurvCancel.Reference = fileName.Replace(".xml", ""); 
                finsurvCancel.Version = "1";
                finsurvCancel.Environment = _hostEnvironment.EnvironmentName == "Production" ? "P" : "T";
                finsurvCancel.CancelledTransaction!.TrnReference = (sarbData.TxReferenceNo).Replace(".xml", ""); 
                finsurvCancel.CancelledTransaction!.Flow = sarbData.TxFlow;
                finsurvCancel.CancelledTransaction.SequenceNumber = Convert.ToInt32(sarbData.TxSequenceNo);
                finsurvCancel.CancelledTransaction.LineNumber = 1;

                envelope.Body!.Content = finsurvCancel;
                envelope.Header!.Sender = userName!;
                envelope.Header.Identity = password!;
                envelope.Header.SentAt = DateTimeOffset.Now;
                envelope.Header.Reference = fileName.Replace(".xml", ""); 
                envelope.Header.Recipient = "SARB";
                envelope.Header.IdentityType = 1;
                envelope.Header.Type = "FINSURV";
                envelope.Header.Version = 1;
                return envelope;
            }
            catch (Exception)
            {
                throw;
            }
        }


        /**
        * Description : Replace request for sarb data
        * @author     : Rakibul
        * @since      : 25/01/2025      
        */
        public async Task<bool> ReplaceSarbRequest(string refrence)
        {
            string filename = GetFileName();
            var result = GetSarbData(refrence);
            var regularData = ConvertReplaceXML(result, filename);
            var file = Helper.FileSave(regularData, _appConfig.REPLACEFilelocation!);
            var saveFile = await UploadFile(file.FileLocation!, file.FileName!);  // send on sarb database
            if (!string.IsNullOrWhiteSpace(file.FileLocation))
            {
                string fie = GetFile(file.FileLocation);
                List<AddSarbeModel> addSarbeModels = new List<AddSarbeModel>();
                AddSarbeModel ot = new AddSarbeModel()
                {
                    tx_file_name = filename,
                    tx_txn_ref_no = refrence,
                    tx_warning_code = fie,
                    tx_response = saveFile.Message
                };
                addSarbeModels.Add(ot);
                var res = await SaveRegular(addSarbeModels);
                return res;
            }
            else
            {
                return false;
            }
        }


        /**
        * Description : Convert Replace to sarb data on xml
        * @author     : Rakibul
        * @since      : 25/01/2025      
        */
        public SARBDEXEnvelopeCommon<FinsurvReplacement> ConvertReplaceXML(RemittanceSarbData item, string fileName)
        {
            try
            {
                var userName = _configuration.GetSection("GetUrl").GetValue<string>("Username");
                var password = _configuration.GetSection("GetUrl").GetValue<string>("Pwd");
                Dictionary<string, string> preferencesDict = GetPreferenceMapping();
                SARBDEXEnvelopeCommon<FinsurvReplacement> envelope = new SARBDEXEnvelopeCommon<FinsurvReplacement>();
                Models.Replacement.OriginalTransaction original = new Models.Replacement.OriginalTransaction();
                original.BranchCode = item.TxBranchCode;
                original.BranchName = item.TxBranchName;
                original.ReportingQualifier = item.TxReportingQualifier;
                original.TrnReference = item.TxReferenceNo;
                original.OriginatingBank = item.TxOriginatingBank;
                original.ReceivingBank = item.TxReceivingBank;
                original.Flow = item.TxFlow;
                original.LineNumber = 1;
                original.ReplacementTransaction = "Y";
                original.Date = Convert.ToDateTime(item.DtTxnDate);
                original.OriginatingCountry = item.TxOriginatingCountry;
                original.TotalValue = Convert.ToDouble(item.DecBenAmount) + Convert.ToDouble(item.DecCustomerAmount);
                original.ReceivingCountry = item.TxReceivingCountry;
                original.ReplacementOriginalReference = item.TxReferenceNo;
                original.NonResident = new Models.Replacement.NonResident
                {
                    Individual = new Models.Replacement.Individual
                    {
                        Surname = item.TxBenSurname,
                        Name = item.TxBenSurname,
                        AdditionalNonResidentData = new Models.Replacement.AdditionalNonResidentData
                        {
                            AccountIdentifier = item.TxBenAccIdentifier,
                            Country = item.TxBenCountry,
                            AccountNumber = item.TxBenAccIdentifier == "CASH"? null : item.TxBenAccNo,
                        },
                    }
                };
                original.ResidentCustomerAccountHolder = new Models.Replacement.ResidentCustomerAccountHolder
                {
                    IndividualCustomer = new Models.Replacement.IndividualCustomer
                    {
                        Surname = item.TxCustomerSurname,
                        Name = item.TxCustomerName,
                        Gender = item.TxCustomerGender,
                        DateOfBirth = Convert.ToDateTime(item.DtCustomerDob),
                        TempResPermitNumber = item.TxBopCategory == "401" ? null : item.TxCustomerIdentityNo,
                        IDNumber = item.TxBopCategory == "401" ? item.TxCustomerIdentityNo : null,
                        PassportNumber = item.TxCustomerPassportNo,
                        PassportCountry = item.TxCustomerPassportCountry,
                        AdditionalCustomerData = new Models.Replacement.AdditionalCustomerData
                        {
                            AccountIdentifier = item.TxCustomerAccIdentifier,
                            StreetAddressLine1 = item.TxCustomerAddress,
                            StreetSuburb = item.TxCustomerSuburb,
                            StreetCity = item.TxCustomerCity,
                            StreetProvince = item.TxCustomerProvince,
                            StreetPostalCode = item.TxCustomerPostalCode,

                            PostalAddressLine1 = item.TxCustomerAddress,
                            PostalSuburb = item.TxCustomerSuburb,
                            PostalCity = item.TxCustomerCity,
                            PostalProvince = item.TxCustomerProvince,
                            PostalCode = item.TxCustomerPostalCode,
                            ContactSurname = item.TxCustomerSurname,
                            ContactName = item.TxCustomerName,
                            Telephone = item.TxCustomerPhone,
                        }
                    }
                };
                original.MonetaryDetails = new Models.Replacement.MonetaryDetails
                {
                    SequenceNumber = item.TxSequenceNo,
                    MoneyTransferAgentIndicator = "ADLA",
                    RandValue = item.DecCustomerAmount.ToString(),
                    ForeignValue = item.DecBenAmount.ToString(),
                    ForeignCurrencyCode = item.TxBenCurrencyCode,
                    BoPCategory = item.TxBopCategory,
                    RulingsSection = item.TxRulingSection,
                    LocationCountry = item.TxLocationCountry,
                    LegalEntityThirdPartyName = item.TxMTAI == "ADLA|S2S" ? preferencesDict["LETP_NAME"] : null,
                    ThirdPartyContactSurname = item.TxMTAI == "ADLA|S2S" ? preferencesDict["TPC_SURNAME"] : null,
                    ThirdPartyContactName = item.TxMTAI == "ADLA|S2S" ? preferencesDict["TPC_NAME"] : null,
                    ThirdPartyTelephone = item.TxMTAI == "ADLA|S2S" ? preferencesDict["TP_TELEPHONE"] : null,
                    ReversalTrnRefNumber= item.TxReferenceNo.Contains("REFUND") ? item.TxReferenceNo.Replace("REFUND","") : null,
                    ReversalSequence= item.TxReferenceNo.Contains("REFUND") ? "1" : null
                };
                FinsurvReplacement bodyContent = new FinsurvReplacement();
                bodyContent.Version = 1;
                bodyContent.Reference = fileName.Replace(".xml", "");
                bodyContent.Environment = _hostEnvironment.EnvironmentName == "Production" ? "P" : "T";
                bodyContent.OriginalTransactions = original;
                envelope.Body!.Content = bodyContent;
                envelope.Header!.Sender = userName!;
                envelope.Header.Identity = password!;
                envelope.Header.SentAt = DateTimeOffset.Now;
                envelope.Header.Reference = fileName.Replace(".xml", "");
                envelope.Header.Recipient = "SARB";
                envelope.Header.IdentityType = 1;
                envelope.Header.Type = "FINSURV";
                envelope.Header.Version = 1;
                return envelope;
            }
            catch (Exception)
            {

                throw;
            }
        }


        /**
        * Description : Refund request for sarb data   
        * @author     : Rakibul      
        * @since      : 25/01/2025        
        */
        public async Task<bool> RefundSarbRequest(string refrence)
        {
            string filename = GetFileName();
            var result = GetSarbData(refrence);
            var regularData = ConvertRefundXML(result,filename);
            var file = Helper.FileSave(regularData, _appConfig.REFUNDFilelocation!);
            var saveFile = await UploadFile(file.FileLocation!, file.FileName!);  // send on sarb database
            if (!string.IsNullOrWhiteSpace(file.FileLocation))
            {
                string fie = GetFile(file.FileLocation);
                List<AddSarbeModel> addSarbeModels = new List<AddSarbeModel>();
                AddSarbeModel ot = new AddSarbeModel()
                {
                    tx_file_name = filename,
                    tx_txn_ref_no = refrence + "REFUND",
                    tx_txn_type = "REMITTANCE",
                    tx_warning_code = fie,
                    is_active = 1,
                    id_env_key = 100000, // need to write store procedure to get environment key
                    id_user_mod_key = -9999,
                    tx_response = saveFile.Message
                };
                addSarbeModels.Add(ot);
                var res = await SaveRegular(addSarbeModels);
                if (res)
                {
                   var saveSarb= SaveTransactionRemitanceSarbData(result);
                   return saveSarb;
                }
                return res;
            }
            else
            {
                return false;
            }
        }


        /**
        * Description : Convert Refund to sarb data on xml     
        * @author     : Rakibul       
        * @since      : 25/01/2025      
        */
        public SARBDEXEnvelopeCommon<FinsurvRefund> ConvertRefundXML(RemittanceSarbData item , string fileName)
        {
            try
            {
                item.TxBopCategory=setBopCategory(item.TxBopCategory);
                
                var userName = _configuration.GetSection("GetUrl").GetValue<string>("Username");
                var password = _configuration.GetSection("GetUrl").GetValue<string>("Pwd");
                Dictionary<string, string> preferencesDict = GetPreferenceMapping();
                SARBDEXEnvelopeCommon<FinsurvRefund> envelope = new SARBDEXEnvelopeCommon<FinsurvRefund>();
                Models.Refund.OriginalTransaction original = new Models.Refund.OriginalTransaction();
                original.BranchCode = item.TxBranchCode;
                original.BranchName = item.TxBranchName;
                original.ReportingQualifier = item.TxReportingQualifier;
                original.TrnReference = item.TxReferenceNo +"REFUND";
                original.OriginatingBank = item.TxReceivingBank;
                original.ReceivingBank = item.TxOriginatingBank;
                original.Flow = "IN";
                original.LineNumber = 1;
                original.ReplacementTransaction = "N";
                original.Date = Convert.ToDateTime(item.DtTxnDate);
                original.OriginatingCountry = item.TxReceivingCountry;
                original.TotalValue = Convert.ToDouble(item.DecBenAmount) + Convert.ToDouble(item.DecCustomerAmount);
                original.ReceivingCountry = item.TxOriginatingCountry;
                original.NonResident = new Models.Refund.NonResident
                {
                    Individual = new Models.Refund.Individual
                    {
                        Surname = item.TxBenSurname,
                        Name = item.TxBenSurname,
                        AdditionalNonResidentData = new Models.Refund.AdditionalNonResidentData
                        {
                            AccountIdentifier = item.TxBenAccIdentifier,
                            Country = item.TxBenCountry,
                            AccountNumber = item.TxBenAccIdentifier == "CASH" ? null : item.TxBenAccNo,
                        },
                    }
                };
                original.ResidentCustomerAccountHolder = new Models.Refund.ResidentCustomerAccountHolder
                {
                    IndividualCustomer = new Models.Refund.IndividualCustomer
                    {
                        Surname = item.TxCustomerSurname,
                        Name = item.TxCustomerName,
                        Gender = item.TxCustomerGender,
                        DateOfBirth = Convert.ToDateTime(item.DtCustomerDob),
                        TempResPermitNumber = item.TxBopCategory == "401" ? null : item.TxCustomerIdentityNo,
                        IDNumber = item.TxBopCategory == "401" ? item.TxCustomerIdentityNo : null,
                        PassportNumber = item.TxCustomerPassportNo,
                        PassportCountry = item.TxCustomerPassportCountry,
                        AdditionalCustomerData = new Models.Refund.AdditionalCustomerData
                        {
                            AccountIdentifier = item.TxCustomerAccIdentifier,
                            StreetAddressLine1 = item.TxCustomerAddress,
                            StreetSuburb = item.TxCustomerSuburb,
                            StreetCity = item.TxCustomerCity,
                            StreetProvince = item.TxCustomerProvince,
                            StreetPostalCode = item.TxCustomerPostalCode,
                            PostalAddressLine1 = item.TxCustomerAddress,
                            PostalSuburb = item.TxCustomerSuburb,
                            PostalCity = item.TxCustomerCity,
                            PostalProvince = item.TxCustomerProvince,
                            PostalCode = item.TxCustomerPostalCode,
                            ContactSurname = item.TxCustomerSurname,
                            ContactName = item.TxCustomerName,
                            Telephone = item.TxCustomerPhone,
                        }
                    }
                };
                original.MonetaryDetails = new Models.Refund.MonetaryDetails
                {
                    SequenceNumber = item.TxSequenceNo,
                    MoneyTransferAgentIndicator = "ADLA",
                    RandValue = item.DecCustomerAmount.ToString(),
                    ForeignValue = item.DecBenAmount.ToString(),
                    ForeignCurrencyCode = item.TxBenCurrencyCode,
                    BoPCategory = item.TxBopCategory,
                    RulingsSection = item.TxRulingSection,
                    LocationCountry = item.TxLocationCountry,
                    ReversalTrnRefNumber= item.TxReferenceNo,
                    ReversalSequence="1",
                    LegalEntityThirdPartyName = item.TxMTAI == "ADLA|S2S" ?  preferencesDict["LETP_NAME"] : null ,
                    ThirdPartyContactSurname = item.TxMTAI == "ADLA|S2S" ? preferencesDict["TPC_SURNAME"] : null,
                    ThirdPartyContactName = item.TxMTAI == "ADLA|S2S" ? preferencesDict["TPC_NAME"] : null,
                    ThirdPartyTelephone = item.TxMTAI == "ADLA|S2S" ? preferencesDict["TP_TELEPHONE"] : null
                };


                FinsurvRefund bodyContent = new FinsurvRefund();
                bodyContent.Version = 1;
                bodyContent.Reference = fileName.Replace(".xml", "");
                bodyContent.Environment = _hostEnvironment.EnvironmentName == "Production" ? "P" : "T";
                bodyContent.OriginalTransactions = original;
                envelope.Body!.Content = bodyContent;
                envelope.Header!.Sender = userName!;
                envelope.Header.Identity = password!;
                envelope.Header.SentAt = DateTimeOffset.Now;
                envelope.Header.Reference = fileName.Replace(".xml", "");
                envelope.Header.Recipient = "SARB";
                envelope.Header.IdentityType = 1;
                envelope.Header.Type = "FINSURV";
                envelope.Header.Version = 1;
                return envelope;
            }
            catch (Exception)
            {

                throw;
            }
        }


        /**
        * Description : Get file from file path
        * @author     : Rakibul
        * @since      : 25/01/2025      
        */
        public string GetFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    return File.ReadAllText(filePath);
                }
                else
                {
                    throw new FileNotFoundException("The specified file does not exist.", filePath);
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }


        /**
        * Description : Replace request for sarb data
        * @author     : Rakibul
        * @since      : 25/01/2025      
        */
        private Dictionary<string, string> GetPreferenceMapping()
        {
            string connectionString = _dbContext!.ConnectionString ?? throw new Exception("not get connection string");
            string inputValue = "{ \"pref_group\": \"SARB_REPORTABLE\"}";
            const string spName = "getPreferenceByGroup_sarb";
            var res = CallStoreProcedure(connectionString, spName, inputValue);
            DatabaseResponse modelResponse = JsonSerializer.Deserialize<DatabaseResponse>(res)!;
            var preferencesDict = new Dictionary<string, string>();
            if (modelResponse.Success)
            {
                var preferences = JsonSerializer.Deserialize<HashSet<PreferenceMapping>>(modelResponse.Data)!;
                foreach (var pref in preferences)
                {
                    preferencesDict[pref.PreferenceName!] = pref.PreferenceValue!;
                }
                return preferencesDict;
            }
            else
            {
                return new Dictionary<string, string>();
            }
        }


        /**
        * Description : Get file name from database
        * @author     : Rakibul
        * @since      : 25/01/2025      
        */
        private string GetFileName()
        {
            const string GET_TYPE = "gethieghtnumber_sarb_data";
            var resData = CallStoreProcedure(_dbContext!.ConnectionString!, GET_TYPE);
            DatabaseResponse model = JsonSerializer.Deserialize<DatabaseResponse>(resData)!;
            if (model.Data.Contains(".xml"))
            {
                model.Data= model.Data.Replace(".xml", "");
            }

            if (model.Data == "0")
            {
                model.Data = "0000001";
            }
            else
            {
                int number = Convert.ToInt32(model.Data) + 1;
                model.Data = number.ToString().PadLeft(7, '0');
            }
            string code = "137";
            string year = DateTime.Now.Year.ToString();
            string generatedNumber = $"{code}{year}{model.Data}{".xml"}";
            return generatedNumber;
        }


        /**
        * Description : Get sarb data from database
        * @author     : Rakibul
        * @since      : 25/01/2025      
        */
        public RemittanceSarbData GetSarbData(string refrence)
        {
            try
            {
                List<string> list = new List<string>();
                list.Add(refrence);
                string connectionString = _dbContext.ConnectionString ?? throw new Exception("not get connection string");
                const string GET_TYPE = "getbyReference_sarb";
                var jsonValue = JsonSerializer.Serialize(list);
                var resData = CallStoreProcedure(connectionString, GET_TYPE, jsonValue);
                DatabaseResponse model = JsonSerializer.Deserialize<DatabaseResponse>(resData)!;
                List<RemittanceSarbData> remittanceSarbData = JsonSerializer.Deserialize<List<RemittanceSarbData>>(model.Data)!;
                return remittanceSarbData[0];
            }
            catch (Exception)
            {
                return new RemittanceSarbData();
            }
        }

         
        /**
        * Description : Get Bop Category on Refund
        * @author     : Rakibul
        * @since      : 29/01/2025      
        */
        public string setBopCategory(string category)
        {
            if (Convert.ToInt32(category) >= 200 && Convert.ToInt32(category) < 300)
            {
                return "200";
            }
            else if (Convert.ToInt32(category) >= 300 && Convert.ToInt32(category) < 400)
            {
                return "300";
            }
            else if (Convert.ToInt32(category) >= 400 && Convert.ToInt32(category) < 500)
            {
                return "400";
            }
            else
            {
                return "";
            }
        }


        /**
        * Description : Save Transaction Remitance Sarb Data
        * @author     : Rakibul
        * @since      : 29/01/2025      
        */
        public bool SaveTransactionRemitanceSarbData(RemittanceSarbData item)
        {
            try
            {
                string tempOrganization= item.TxReceivingBank;
                string tempCountry=item.TxReceivingCountry;
                item.TxBopCategory = setBopCategory(item.TxBopCategory);
                item.TxFlow = "IN";
                item.TxReferenceNo = item.TxReferenceNo + "REFUND";
                item.TxReceivingBank = item.TxOriginatingBank;
                item.TxOriginatingBank = tempOrganization;
                item.TxReceivingCountry = item.TxOriginatingCountry;
                item.TxOriginatingCountry = tempCountry;
                string connectionString = _dbContext!.ConnectionString ?? throw new Exception("not get connection string");
                const string GET_TYPE = "ins_remittance_sarb_data";
                var jsonValue = JsonSerializer.Serialize(item);
                var resData = CallStoreProcedure(connectionString, GET_TYPE, jsonValue);
                DatabaseResponse model = JsonSerializer.Deserialize<DatabaseResponse>(resData)!;
                return model.Success;
            }
            catch (Exception)
            {
                throw;
            }
        }


        /**
        * Description : upload sarb reporting file
        * @author     : Rakibul
        * @since      : 14/01/2025      
        */
        public async Task<ApiResponse> UploadFile(string fileUrl, string fileName)
        {
            try
            {
                // Read configuration values
                var url = _configuration.GetSection("GetUrl").GetValue<string>("fileUpload");
                var userName = _configuration.GetSection("GetUrl").GetValue<string>("Username");
                var password = _configuration.GetSection("GetUrl").GetValue<string>("Pwd");

                // Combine URL and query parameters
                var requestUrl = $"{url}{fileName}?{userName}?{password}";

                // Initialize HttpClient
                using var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);

                // Add custom headers (if required)
                request.Headers.Add("fileName", fileName);

                // Get XML content based on the fileUrl type (local or remote)
                string xmlContent;

                // Local file path
                xmlContent = await File.ReadAllTextAsync(fileUrl);

                // Add the XML content to the request
                var content = new StringContent(xmlContent, Encoding.UTF8, "application/xml");
                request.Content = content;

                // Send the request
                var response = await client.SendAsync(request);

                // Ensure response is successful
                response.EnsureSuccessStatusCode();

                var rr = await response.Content.ReadAsStringAsync();
                ApiResponse apiResponse = new ApiResponse();
                apiResponse.IsSuccessStatusCode = response.IsSuccessStatusCode;
                apiResponse.StatusCode = Convert.ToInt32(response.StatusCode);
                apiResponse.Message = rr;

                return apiResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading file: {ex.Message}");           
                return new ApiResponse() { Message = ex.Message, IsSuccessStatusCode = false, StatusCode = 500, Error = "Internal Server error" };
            }
        }


        /**
        * Description : Get Sarb data by fileName
        * @author     : Rakibul
        * @since      : 12/02/2025      
        */
        public async Task<List<AddSarbeModel>> GetSarbDataByFileName(string fileName)
        {
            try
            {
                const string sp_name = "getbyfilename_sarb";
                string connectionString = _dbContext!.ConnectionString ?? throw new Exception("not get connection string");
                var list = new
                {
                    tx_file_name = fileName,
                };
                var jsonValue = JsonSerializer.Serialize(list);
                var resData = CallStoreProcedure(connectionString, sp_name, jsonValue);
                DatabaseResponse model = JsonSerializer.Deserialize<DatabaseResponse>(resData)!;
                List<AddSarbeModel> remittanceSarbData = JsonSerializer.Deserialize<List<AddSarbeModel>>(model.Data)!;
                return remittanceSarbData;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        /**
        * Description : Get Sarb data by refrence no
        * @author     : Rakibul
        * @since      : 13/01/2025      
        */
        public async Task<AddSarbeModel> GetSarbDataByRefrence(string refNo)
        {
            try
            {
                const string sp_name = "getbyrefrenceNo_sarb";
                string connectionString = _dbContext!.ConnectionString ?? throw new Exception("not get connection string");
                var list = new
                {
                    tx_txn_ref_no = refNo,
                };
                var jsonValue = JsonSerializer.Serialize(list);
                var resData = CallStoreProcedure(connectionString, sp_name, jsonValue);
                DatabaseResponse model = JsonSerializer.Deserialize<DatabaseResponse>(resData)!;
                AddSarbeModel remittanceSarbData = JsonSerializer.Deserialize<AddSarbeModel>(model.Data)!;
                return remittanceSarbData;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        /**
        * Description : Deserilize string to c# class
        * @author     : Rakibul
        * @since      : 15/01/2025      
        */
        public T DeserializeXml<T>(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringReader reader = new StringReader(xml))
            {
                return (T)serializer.Deserialize(reader);
            }
        }
    }
}
