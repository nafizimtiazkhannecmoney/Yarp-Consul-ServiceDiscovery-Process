using Microsoft.AspNetCore.Mvc;
using System.Xml.Serialization;
using System.Xml.Linq;
using SARB_Reporting.Services;
using static SARB_Reporting.Models.SaReporting;
using SARB_Reporting.Models;
using SARB_Reporting.Models.Regular;
using Microsoft.AspNetCore.Authorization;

namespace SARB_Reporting.Controllers
{
    /**
    * Description : SARB data getting api
    * @author     : Rakibul
    * @since      : 11/01/2025      
    */
    //[Route("SARBDEX2/getmsgbysarbref")]
    [Route("api/[controller]")]
    //[Route("api/report")]
    [ApiController]
    [Tags("SARB-Data-Getting")]
    public class SarbController : ControllerBase
    {
        private readonly ILogger<SarbController> _logger;
        private readonly SarbDataService _SarbDataService;
        private readonly IConfiguration _configuration;

        public SarbController(ILogger<SarbController> logger, SarbDataService sarbDataService, IConfiguration configuration)
        {
            _logger = logger;
            _SarbDataService = sarbDataService;
            _configuration = configuration;
        }



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
                orders[i] =  "--TEST_SARB_SERVICE--" + prefix + suffix;
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
        * Description : Get sarb file by name
        * @author     : Rakibul
        * @since      : 01/02/2025      
        */
        [HttpGet("GetByFileName")]
        public async Task<IActionResult> GetByFileName(string fileName)
        {
            _logger.LogInformation("Action call started from GetByFileName");
            try
            {
                if (!fileName.Contains(".xml"))
                {
                    fileName = fileName + ".xml";
                }
                var url = _configuration.GetSection("GetUrl").GetValue<string>("ByFileName");
                var userName = _configuration.GetSection("GetUrl").GetValue<string>("Username");
                var password = _configuration.GetSection("GetUrl").GetValue<string>("Pwd");

                var resultOfRemitanceData = await _SarbDataService.GetSarbDataByFileName(fileName);

                if (fileName.Contains(".xml"))
                {
                    fileName = fileName.Replace(".xml", "");
                }
                
                // Combine URL and query parameters
                var requestUrl = $"{url}{fileName}?{userName}?{password}";

                // Initialize HttpClient
                using var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var xmlString = await response.Content.ReadAsStringAsync();
                Errors errors = new Errors();
                XElement root = XElement.Parse(xmlString);
                string rootElementName = root.Name.LocalName;

                if (rootElementName == "Errors")
                {
                    if (root.Element("Error") != null)
                    {
                        List<AddSarbeModel> addSarbeModels = new List<AddSarbeModel>();

                        ErrorsResponse errorResponse = _SarbDataService.DeserializeXml<ErrorsResponse>(xmlString);

                        foreach (var item in resultOfRemitanceData)
                        {
                            AddSarbeModel ot = new AddSarbeModel()
                            {
                                tx_txn_ref_no = item.tx_txn_ref_no,
                                tx_sarb_ref_no = errorResponse.ErrorDetails[0].Description,
                                tx_response = xmlString
                            };
                            addSarbeModels.Add(ot);
                        }
                        var res= await _SarbDataService.SaveRegular(addSarbeModels);
                        return Ok(new { responseCode = 200, fileRefrenceNumber = errorResponse.ErrorDetails[0].Description });
                    }
                    else
                    {
                        // Simple Error Response
                        SimpleErrorResponse simpleError = _SarbDataService.DeserializeXml<SimpleErrorResponse>(xmlString);
                        _logger.LogInformation($"❌ ERROR: {simpleError.Message}");
                        return NotFound(new { responseCode = 404, responseDescriotion = simpleError });
                    }
                }
                else if (rootElementName == "SARBDexResponse")
                {
                    Models.SARBDexResponse sarbResponse = _SarbDataService.DeserializeXml<Models.SARBDexResponse>(xmlString);
                    if (resultOfRemitanceData.Count() > 0)
                    {
                        List<AddSarbeModel> addSarbeModels = new List<AddSarbeModel>();

                        foreach (var item in resultOfRemitanceData)
                        {
                            AddSarbeModel ot = new AddSarbeModel()
                            {   
                                tx_txn_ref_no = item.tx_txn_ref_no,
                                tx_response = xmlString
                            };
                            addSarbeModels.Add(ot);
                        }
                        var res = await _SarbDataService.SaveRegular(addSarbeModels);
                    }
                    _logger.LogInformation($"⚠️ SARB RESPONSE: {sarbResponse.StatusDescription}, Status Code: {sarbResponse.StatusCode}, Message: {sarbResponse.Message}");
                    return Ok(new { responseCode = 202, fileRefrenceNumber = xmlString });
                }
                else
                {
                    _logger.LogInformation("❌ Unknown response format");
                    return NotFound(new { responseCode = 404, responseDescriotion = "Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                var res = new
                {
                    responseCode = 500,
                    responseDescription = "Internal server error"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, res);
            }
        }


        /**
        * Description : Get sarb file by reference
        * @author     : Rakibul
        * @since      : 11/02/2025      
        */
        [HttpGet("GetByReference")]
        public async Task<IActionResult> GetByReference(string sarbref)
        {
            _logger.LogInformation("Action call started from GetByReference");
            try
            {
                var url = _configuration.GetSection("GetUrl").GetValue<string>("ByReference");
                var userName = _configuration.GetSection("GetUrl").GetValue<string>("Username");
                var password = _configuration.GetSection("GetUrl").GetValue<string>("Pwd");

                var requestUrl = $"{url}{sarbref}?{userName}?{password}";
                using var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var xmlString = await response.Content.ReadAsStringAsync();
                Errors errors = new Errors();
                FINSURV finsurv = new FINSURV();

                XmlSerializer serializer = new XmlSerializer(typeof(FINSURV));
                using (StringReader reader = new StringReader(xmlString))
                {
                    finsurv = (FINSURV)serializer.Deserialize(reader)!;
                }
                List<AddSarbeModel> addSarbeModels = new List<AddSarbeModel>();
                
                foreach (var item in finsurv.FileReference.Transactions)
                {
                    string warnings=string.Empty;
                    string errorList=string.Empty;
                    foreach (var worning in item.Warnings)
                    {
                        warnings += worning.WarningDescription;
                    }
                    foreach (var error in item.Errors)
                    {
                        errorList += error.ErrorDescription;
                    }

                    AddSarbeModel ot = new AddSarbeModel()
                    {
                        tx_txn_ref_no=item.TrnReference,          
                        tx_retrieve_response = item.Status,       
                        tx_response_by_reference = xmlString,     
                        tx_warning_details = warnings ?? null,    
                        tx_sarb_err_desc = errorList ?? null      
                    };
                    addSarbeModels.Add(ot);
                }
                var res = await _SarbDataService.SaveRegular(addSarbeModels);
                if (res)
                {
                    return Ok(new { responseCode = 200, fileRefrenceNumber = xmlString });
                }
                else
                {
                    _logger.LogInformation("❌ Not Found");
                    return BadRequest(new { responseCode = 404, responseDescriotion = "Not Save on Database" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                var res = new
                {
                    responseCode = 500,
                    responseDescription = "Failed"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, res);
            }
        }
    }
}
