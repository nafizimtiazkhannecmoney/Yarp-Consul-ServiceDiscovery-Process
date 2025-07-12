using Microsoft.AspNetCore.Mvc;
using Nec.Web.Utils;
using SARB_Reporting.Services;
using SARB_Reporting.Utils;
using SARB_Reporting.Models.Regular;

namespace SARB_Reporting.Controllers
{
    /**
    * Description : SARB reporting api
    * @author     : Rakibul
    * @since      : 11/01/2025      
    */

    [Route("api/[controller]")]
    [ApiController]
    [Tags("SARB-Reporting")]
    public class SarbReportController : ControllerBase
    {
        private readonly NecAppConfig _appConfig;
        private readonly SarbDataService _SarbDataService;
        private readonly ILogger<SarbReportController> _logger;

        public SarbReportController(ILogger<SarbReportController> logger,  NecAppConfig necAppConfig, SarbDataService sarbDataService)
        {
            _logger = logger;
            _appConfig = necAppConfig;
            _SarbDataService = sarbDataService;
        }


        /**
        * Description : Save Regular Transaction
        * @author     : Rakibul
        * @since      : 11/01/2025      
        */
        [HttpPost("Regular")]
        public async Task<IActionResult> Regular([FromBody] List<TransactionData> refrencrList)
        {
            _logger.LogInformation("Action call started from REGULAR");
            try
            {
                List<string> strings = new List<string>();
                foreach (TransactionData transactionData in refrencrList)
                {
                    if (transactionData.ReferenceNo.Contains("string"))
                    {
                        return BadRequest(new { responseCode = 400, responseDescriotion = transactionData.ReferenceNo+" "+ "Is not correct refrence no" });
                    }
                    strings.Add(transactionData.ReferenceNo);
                }
                var result= _SarbDataService.GetSarbDataForRegularByTrnReference(strings);
                var regularData = _SarbDataService.CovertXMLModel(result);
                var file = Helper.FileSave(regularData, _appConfig.REGULARFilelocation!);

                var saveFile = await _SarbDataService.UploadFile(file.FileLocation!, file.FileName!);
                if (saveFile.IsSuccessStatusCode)
                {
                    List<AddSarbeModel> originalTransactionREport = new List<AddSarbeModel>();
                    foreach (var transaction in result)
                    {
                        AddSarbeModel ot = new AddSarbeModel()
                        {
                            tx_file_name = file.FileName,
                            tx_txn_ref_no = transaction.TrnReference,
                            tx_txn_type = "REMITTANCE",
                            is_active = 1,
                            id_env_key = 100000,
                            id_user_mod_key = -9999,
                            tx_response=saveFile.Message,
                        };
                        originalTransactionREport.Add(ot);
                    }
                    bool res = await _SarbDataService.SaveRegular(originalTransactionREport);
                    if (res)
                    {
                        return Ok(new { responseCode = 200, responseDescriotion = file.FileName });
                    }
                    else
                    {
                        return BadRequest(new { responseCode = 404, responseDescriotion = "File Uploaded on sarb but not save on database" });
                    }
                }
                return BadRequest(new { responseCode = 200, responseDescriotion = "File Not Upload on sarb" });
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


        /**
        * Description : Save Cancel data in file
        * @author     : Rakibul
        * @since      : 11/01/2025      
        */
        [HttpPost("Cancel")]
        public async Task<IActionResult> Cancel([FromBody] TransactionData refrencrList)
        {
            _logger.LogInformation("Action call started from CANCEL");
            try
            {
                var val = await _SarbDataService.CancelSarbRequest(refrencrList.ReferenceNo);
                return Ok(new { responseCode = 200, responseDescriotion = "Transaction Canceled Successfully" });
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


        /**
        * Description : Save Replace File
        * @author     : Rakibul
        * @since      : 11/01/2025      
        */
        [HttpPost("Replace")]
        public async Task<IActionResult> Replace([FromBody] TransactionData refrencr)
        {
            _logger.LogInformation("Action call started from REPLACE");
            try
            {
                var val = await _SarbDataService.ReplaceSarbRequest(refrencr.ReferenceNo);
                return Ok(new { responseCode = 200, responseDescriotion = "Replacement Successful" });
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
        
        
        /**
        * Description : Save refund file
        * @author     : Rakibul
        * @since      : 11/01/2025      
        */
        [HttpPost("Refund")]
        public async Task<IActionResult> Refund([FromBody] TransactionData refrencrList)
        {
            _logger.LogInformation("Action call started from REFUND");
            string notificationData = string.Empty;
            try
            {
                var val = await _SarbDataService.RefundSarbRequest(refrencrList.ReferenceNo);
                return Ok(new { responseCode = 200, responseDescriotion = "Refund Processed Successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, notificationData);
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
