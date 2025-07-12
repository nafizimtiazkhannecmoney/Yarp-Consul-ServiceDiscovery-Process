using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaGoAMLReporting.Model;
using SaGoAMLReporting.Service;
using SaGoAMLReporting.Service.Interfaces;
using System.Text;
using System.Xml.Serialization;

namespace SaGoAMLReporting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoAMLController : ControllerBase
    {
        private IGoAMLDataService _goAMLDataService;
        private IValidateXML _validateXML;
        private ILogger<GoAMLController> _logger;

        public GoAMLController(IGoAMLDataService goAMLDataService, IValidateXML validateXML, ILogger<GoAMLController> logger)
        {
            _goAMLDataService = goAMLDataService;
            _validateXML = validateXML;
            _logger = logger;
        }



        //Test--------------------------------------------------------------------
        [Authorize(Roles = "OPERATOR,ADMIN")]
        [HttpGet("GetTestData")]
        public string[] GetTest()
        {
            var random = new Random();
            var orders = new string[3];

            for (int i = 0; i < orders.Length; i++)
            {
                // Generate a random string with "sarb" embedded
                int prefixLength = random.Next(0, 15);
                int suffixLength = random.Next(0, 15);
                string prefix = GenerateRandomString(prefixLength, random);
                string suffix = GenerateRandomString(suffixLength, random);
                string time = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                orders[i] = time + "--TEST_GoAML_SERVICE--" + prefix + suffix;
            }

            return orders;
        }
        private string GenerateRandomString(int length, Random random)
        {
            const string chars = "123456789123456789123456789123456789";
            char[] result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = chars[random.Next(chars.Length)];
            }
            return new string(result);
        }


        /**
         * inline documents
         * Post API - Generate validated xml file in base64 by type wise request
         * 
         * @author: Mohiuddin
         * @since: 01/01/2025      
         */
        [HttpPost("GenerateFileBase64")]
        public async Task<IActionResult> GenerateFileBase64([FromBody] XMLReportRequest request)
        {
            try
            {
                _logger.LogInformation("GoAML->GenerateFileBase64 : Api started at:" + DateTime.Now);

                (XmlModel? report, GoAMLModel? goAMLModel) = await _goAMLDataService.GetRemittanceSarbData(request);

                // Serialize the XmlModel object to a memory stream for byte[]               
                byte[] xmlBytes = await _goAMLDataService.GetXmlBytesOfRequestedData(report);

                // Validate the XML byte array                
                string validationResult = _validateXML.Validate(xmlBytes);

                if (validationResult == "XML is valid.")
                {
                    var response = new XMLReportResponse
                    {
                        Header = new Header
                        {
                            ActionName = request.Header?.ActionName,
                            ServiceName = request.Header?.ServiceName
                        },
                        Payload = new XMLReportPayloadResponse
                        {
                            Base64 = Convert.ToBase64String(xmlBytes),
                            FileName = report?.EntityReference + ".xml"
                        }
                    };
                    //Array.Sort(xmlBytes);
                    if (goAMLModel?.TxnIdList != null)
                    {
                        foreach (var item in goAMLModel.TxnIdList)
                        {
                            var existGoAml = await _goAMLDataService.GetExistFileInGoAML(item, goAMLModel.TxnType ?? "");
                            goAMLModel.TxnId = item;
                            goAMLModel.GoAmlId = existGoAml?.GoAmlId ?? 0;

                            if (existGoAml != null)
                            {
                                await _goAMLDataService.UpdateGoAML(goAMLModel);
                            }
                            else
                            {
                                await _goAMLDataService.InsertGoAML(goAMLModel);
                            }
                        }
                    }
                    // Return the response
                    _logger.LogInformation("GoAML->GenerateFileBase64 : Api completed successfully at:" + DateTime.Now);//.log file extension
                    return Ok(response);
                }
                else
                {
                    _logger.LogInformation("GoAML->GenerateFileBase64 : Xml is not valid :" + DateTime.Now);//.log file extension
                    return ValidationProblem(validationResult);
                }
                // Prepare the response model
            }
            catch (Exception ex)
            {
                _logger.LogError("GoAML->GenerateFileBase64 : Error occurred :" + ex.Message);
                throw;
            }
        }

        /**
         * inline documents
         * Post API - Generate validated xml file by type wise request
         * 
         * @author: Mohiuddin
         * @since: 01/01/2025      
         */
        [HttpPost("GenerateFile")]
        public async Task<IActionResult> GenerateFile([FromBody] XMLReportRequest request)
        {
            try
            {
                _logger.LogInformation("GoAML->GenerateFile : Api started at:" + DateTime.Now);
                (XmlModel? report, GoAMLModel? goAMLModel) = await _goAMLDataService.GetRemittanceSarbData(request);

                // Serialize the XmlModel object to a memory stream for byte[]               
                byte[] xmlBytes = await _goAMLDataService.GetXmlBytesOfRequestedData(report);

                // Validate the XML byte array
                string validationResult = _validateXML.Validate(xmlBytes);

                //Map Valid Data for Response
                if (validationResult == "XML is valid.")
                {
                    var response = new
                    {
                        Header = new
                        {
                            request.Header.ActionName,
                            request.Header.ServiceName
                        },
                        Payload = new
                        {
                            Base64 = Convert.ToBase64String(xmlBytes),
                            File = xmlBytes,
                            FileName = report?.EntityReference + ".xml"
                        }
                    };

                    if (goAMLModel?.TxnIdList != null)
                    {
                        foreach (var item in goAMLModel.TxnIdList)
                        {
                            var existGoAml = await _goAMLDataService.GetExistFileInGoAML(item, goAMLModel.TxnType ?? "");
                            goAMLModel.TxnId = item;
                            goAMLModel.GoAmlId = existGoAml?.GoAmlId ?? 0;

                            if (existGoAml != null)
                            {
                                await _goAMLDataService.UpdateGoAML(goAMLModel);
                            }
                            else
                            {
                                await _goAMLDataService.InsertGoAML(goAMLModel);
                            }
                        }
                    }

                    _logger.LogInformation("GoAML->GenerateFile : Api completed successfully at:" + DateTime.Now);
                    return File(xmlBytes, "application/octet-stream", response.Payload.FileName);
                }
                else
                {
                    _logger.LogInformation("GoAML->GenerateFile : Api completed successfully by returning validation ptoblem at:" + DateTime.Now);
                    return ValidationProblem(validationResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("GoAML->GenerateFile : Error occurred :" + ex.Message);
                throw;
            }
        }

        /**
         * inline documents
         * Post API - Generate STR Monthly Report File as Base64
         * 
         * @author: Mohiuddin
         * @since: 01/01/2025      
         */
        [HttpPost("GetSTRMonthlyReport")]
        public async Task<IActionResult> GetSTRMonthlyReport([FromBody] STR_Monthly_Report_Request_Body? request_Body, string FileType = "Pdf", string ContentType = "application/pdf")
        {
            try
            {
                _logger.LogInformation("GoAML->GetSTRMonthlyReport : Api started at:" + DateTime.Now);

                DateTime dTime = DateTime.Now;
                string strExelFileName = string.Format("STR_Monthly_Report_");
                string savePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\downloads\\";
                string pathToCheck = savePath + strExelFileName;
                var ExportDate = DateTime.Today.Date;
                strExelFileName += DateTime.Now.ToShortDateString();

                string _ReportName = "STR_Monthly_Report.rdlc";
                string reportPath = Path.GetFullPath(string.Format("Files\\Report\\{0}", _ReportName));

                if (request_Body?.Payload != null)
                    await _goAMLDataService.GetGeneratedAMLMonthlyReport(request_Body.Payload);

                byte[] renderedBytes = await _goAMLDataService.ExportReportFile(reportPath, strExelFileName, FileType, ContentType, _ReportName, null, request_Body);
                var response = new
                {
                    Header = new
                    {
                        UserId = "",
                        ApiKey = "",
                        request_Body?.Header?.ActionName,
                        request_Body?.Header?.ServiceName,
                    },
                    Payload = new[] { new { ReportBs64 = renderedBytes } }
                };

                _logger.LogInformation("GoAML->GetSTRMonthlyReport : Function ends successfully at:" + DateTime.Now);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("GoAML->GetSTRMonthlyReport : Error occurred :" + ex.Message);
                throw;
            }
        }

        /**
         * inline documents
         * Post API - Generate STR Monthly Report as File
         * 
         * @author: Mohiuddin
         * @since: 01/01/2025      
         */
        [HttpPost("GetSTRMonthlyReportAsFile")]
        public async Task<IActionResult> GetSTRMonthlyReportAsFile([FromBody] STR_Monthly_Report_Request_Body? request_Body, string FileType = "Pdf", string ContentType = "application/pdf")
        {
            try
            {
                _logger.LogInformation("GoAML->GetSTRMonthlyReport : Api started at:" + DateTime.Now);

                DateTime dTime = DateTime.Now;
                string strExelFileName = string.Format("STR_Monthly_Report_");
                string savePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\downloads\\";
                string pathToCheck = savePath + strExelFileName;
                var ExportDate = DateTime.Today.Date;
                strExelFileName += DateTime.Now.ToShortDateString();

                string _ReportName = "STR_Monthly_Report.rdlc";
                string reportPath = Path.GetFullPath(string.Format("Files\\Report\\{0}", _ReportName));

                if (request_Body?.Payload != null)
                    await _goAMLDataService.GetGeneratedAMLMonthlyReport(request_Body.Payload);

                byte[] renderedBytes = await _goAMLDataService.ExportReportFile(reportPath, strExelFileName, FileType, ContentType, _ReportName, null, request_Body);

                _logger.LogInformation("GoAML->GetSTRMonthlyReport : Function ends successfully at:" + DateTime.Now);
                return File(renderedBytes, ContentType, string.Format("{0}.{1}", strExelFileName, FileType));
            }
            catch (Exception ex)
            {
                _logger.LogError("GoAML->GetSTRMonthlyReport : Error occurred :" + ex.Message);
                throw;
            }
        }

        /**
         * inline documents
         * Post API - Remittance data for view list
         * 
         * @author: Mohiuddin
         * @since: 04/02/2025      
         */
        [HttpPost("GetRemittanceData")]
        public async Task<IActionResult> GetRemittanceData([FromBody] RemittanceViewDataRequest? request_Body)
        {
            try
            {
                _logger.LogInformation("GoAML->GetRemittanceData : Api started at:" + DateTime.Now);
                var response = await _goAMLDataService.GetRemittanceViewData(request_Body);

                // Return the response
                _logger.LogInformation("GoAML->GetRemittanceData : Api completed successfully at:" + DateTime.Now);//.log file extension
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("GoAML->GetRemittanceData : Error occurred :" + ex.Message);
                throw;
            }
        }

        /**
         * inline documents
         * Post API - Branch data list
         * 
         * @author: Mohiuddin
         * @since: 08/02/2025      
         */
        [HttpPost("GetBranchData")]
        public async Task<IActionResult> GetBranchData([FromBody] BranchRequest? request_Body)
        {
            try
            {
                _logger.LogInformation("GoAML->GetBranchData : Api started at:" + DateTime.Now);
                var response = await _goAMLDataService.GetBranchViewData(request_Body);

                // Return the response
                _logger.LogInformation("GoAML->GetBranchData : Api completed successfully at:" + DateTime.Now);//.log file extension
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("GoAML->GetBranchData : Error occurred :" + ex.Message);
                throw;
            }
        }        
    }
}
