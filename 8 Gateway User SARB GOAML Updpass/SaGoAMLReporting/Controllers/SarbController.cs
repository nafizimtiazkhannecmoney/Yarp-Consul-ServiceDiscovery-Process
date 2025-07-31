using Microsoft.AspNetCore.Mvc;
using SaGoAMLReporting.Service.Interfaces;
using SaGoAMLReporting.Service;
using SaGoAMLReporting.Model;
using System.Text;
using System.Data.SqlTypes;
using SaGoAMLReporting.Helper;

namespace SaGoAMLReporting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SarbController : Controller
    {
        private ISarbDataService SarbDataService;
        private ILogger<SarbController> _logger;

        public SarbController(ISarbDataService SarbDataService,ILogger<SarbController> logger)
        {
            this.SarbDataService = SarbDataService;
            _logger = logger;
        }

        /**
         * inline documents
         * Post API - Get Remittance Sarb Data List
         * 
         * @author: Mohiuddin
         * @since: 11/02/2025      
         */
        [HttpPost("GetRemittanceData")]
        public async Task<IActionResult> GetRemittanceData([FromBody] RemittanceSarbDataRequest? request_Body)
        {
            try
            {
                _logger.LogInformation("Sarb->GetRemittanceData : Api started at:" + DateTime.Now);
                var response = await SarbDataService.GetRemittanceSarbViewData(request_Body);

                // Return the response
                _logger.LogInformation("Sarb->GetRemittanceData : Api completed successfully at:" + DateTime.Now);//.log file extension
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Sarb->GetRemittanceData : Error occurred :" + ex.Message);
                throw;
            }
        }

        /**
         * inline documents
         * Post API - get trandaction sarb data list
         * 
         * @author: Mohiuddin
         * @since: 12/02/2025      
         */
        [HttpPost("GetTransactionData")]
        public async Task<IActionResult> GetTransactionData([FromBody] RemittanceSarbDataRequest? request_Body)
        {
            try
            {
                _logger.LogInformation("Sarb->GetTransactionData : Api started at:" + DateTime.Now);
                var response = await SarbDataService.GetTransactionSarbViewData(request_Body);

                // Return the response
                _logger.LogInformation("Sarb->GetTransactionData : Api completed successfully at:" + DateTime.Now);//.log file extension
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Sarb->GetTransactionData : Error occurred :" + ex.Message);
                throw;
            }
        }

        /**
         * inline documents
         * Post API - get rejected sarb data list
         * 
         * @author: Mohiuddin
         * @since: 12/02/2025      
         */
        [HttpPost("GetRejectedData")]
        public async Task<IActionResult> GetRejectedData([FromBody] RemittanceSarbDataRequest? request_Body)
        {
            try
            {
                _logger.LogInformation("Sarb->GetRejectedData : Api started at:" + DateTime.Now);
                var response = await SarbDataService.GetRejectedSarbViewData(request_Body);

                // Return the response
                _logger.LogInformation("Sarb->GetRejectedData : Api completed successfully at:" + DateTime.Now);//.log file extension
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Sarb->GetRejectedData : Error occurred :" + ex.Message);
                throw;
            }
        }

        /**
         * inline documents
         * Post API - get rejected sarb data list
         * 
         * @author: Mohiuddin
         * @since: 13/02/2025      
         */
        [HttpPost("GetStatusTimelineData")]
        public async Task<IActionResult> GetStatusTimelineData([FromBody] RemittanceSarbDataRequest? request_Body)
        {
            try
            {
                _logger.LogInformation("Sarb->GetStatusTimelineData : Api started at:" + DateTime.Now);
                var response = await SarbDataService.GetSarbStatusViewData(request_Body);

                // Return the response
                _logger.LogInformation("Sarb->GetStatusTimelineData : Api completed successfully at:" + DateTime.Now);//.log file extension
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Sarb->GetStatusTimelineData : Error occurred :" + ex.Message);
                throw;
            }
        }

        /**
         * inline documents
         * Post API - get sarb status excel data api
         * 
         * @author: Mohiuddin
         * @since: 13/02/2025      
         */
        [HttpPost("GetSarbStatusTimelineExcelData")]
        public async Task<IActionResult> GetStatusTimelineExcelData([FromBody] RemittanceSarbDataRequest? request_Body)
        {
            try
            {
                _logger.LogInformation("Sarb->GetStatusTimelineExcelData : Api started at:" + DateTime.Now);
                var data = await SarbDataService.GetSarbStatusViewData(request_Body);

                string csvContent = SarbDataService.GenerateSarbStatusTimelineCSV(data?.Payload);
                var bytes = Encoding.UTF8.GetBytes(csvContent);

                var response = new
                {
                    Header = new
                    {
                        UserId = request_Body?.Header?.UserModifidId,
                        request_Body?.Header?.ActionName,
                        request_Body?.Header?.ServiceName
                    },
                    Payload = new
                    {
                        Base64 = Convert.ToBase64String(bytes),
                    }
                };

                // Return the response
                _logger.LogInformation("Sarb->GetStatusTimelineExcelData : Api completed successfully at:" + DateTime.Now);//.log file extension
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Sarb->GetStatusTimelineExcelData : Error occurred :" + ex.Message);
                throw;
            }
        }

        /**
         * inline documents
         * Post API - get sarb status excel data api as file
         * 
         * @author: Mohiuddin
         * @since: 14/02/2025      
         */
        [HttpPost("GetStatusExcelDataAsFile")]
        public async Task<IActionResult> GetStatusExcelDataAsFile([FromBody] RemittanceSarbDataRequest? request_Body)
        {
            try
            {
                _logger.LogInformation("Sarb->GetStatusExcelDataAsFile : Api started at:" + DateTime.Now);
                var data = await SarbDataService.GetSarbStatusViewData(request_Body);                

                string csvContent = SarbDataService.GenerateSarbStatusTimelineCSV(data?.Payload);
                var bytes = Encoding.UTF8.GetBytes(csvContent);

                // Return the response
                _logger.LogInformation("Sarb->GetStatusExcelDataAsFile : Api completed successfully at:" + DateTime.Now);//.log file extension
                return File(bytes, "text/csv", "SarbStatusExcel_" + DateTime.Now.ToString("yyyyMMddHHmmssff") +".csv");
            }
            catch (Exception ex)
            {
                _logger.LogError("Sarb->GetStatusExcelDataAsFile : Error occurred :" + ex.Message);
                throw;
            }
        }

        /**
         * inline documents
         * Post API - get sarb status excel data api as file
         * 
         * @author: Mohiuddin
         * @since: 14/02/2025      
         */
        [HttpPost("GetTransactionSarbExcelData")]
        public async Task<IActionResult> GetTransactionSarbExcelData([FromBody] RemittanceSarbDataRequest? request_Body)
        {
            try
            {
                _logger.LogInformation("Sarb->GetRemittanceSarbExcelData : Api started at:" + DateTime.Now);
                var data = await SarbDataService.GetTransactionSarbExcelData(request_Body);

                string csvContent = SarbDataService.GenerateTransactionSarbCSV(data?.Payload);
                var bytes = Encoding.UTF8.GetBytes(csvContent);

                // Return the response
                _logger.LogInformation("Sarb->GetRemittanceSarbExcelData : Api completed successfully at:" + DateTime.Now);//.log file extension
                return File(bytes, "text/csv", "SarbStatusExcel_" + DateTime.Now.ToString("yyyyMMddHHmmssff") + ".csv");
            }
            catch (Exception ex)
            {
                _logger.LogError("Sarb->GetRemittanceSarbExcelData : Error occurred :" + ex.Message);
                throw;
            }
        }

        /**
         * inline documents
         * Post API - get remittance outward data api as file
         * 
         * @author: Mohiuddin
         * @since: 22/02/2025      
         */
        [HttpPost("GetRemittanceOutwardViewData")]
        public async Task<dynamic?> GetRemittanceOutwardViewData(DateTime from, DateTime to)
        {
            try
            {
                _logger.LogInformation("Sarb->GetRemittanceOutwardViewData : Api started at:" + DateTime.Now);
                var data = await SarbDataService.GetRemittanceOutwardViewData(from, to);
                _logger.LogInformation("Sarb->GetRemittanceOutwardViewData : Api completed successfully at:" + DateTime.Now);//.log file extension
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError("Sarb->GetRemittanceOutwardViewData : Error occurred :" + ex.Message);

                throw;
            }
        }
    }
}
