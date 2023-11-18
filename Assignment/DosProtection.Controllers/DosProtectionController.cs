using System.Net;
using Microsoft.AspNetCore.Mvc;
using Assignment.DosProtection.DM.Enum;
using Assignment.DosProtection.DM.Interfaces;
using Microsoft.AspNetCore.Http.Features;

namespace Assignment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DosProtectionController : ControllerBase
    {
        private readonly IDosProtectionService _dosProtectionService;
        private readonly IConfiguration _config;
        private readonly ILogger<DosProtectionController> _logger;

        public DosProtectionController(IDosProtectionService dosProtectionService, IConfiguration config,
            ILogger<DosProtectionController> logger)
        {
            _dosProtectionService = dosProtectionService;
            _config = config;
            _logger = logger;
        }

        [HttpGet("StaticWindow/{clientId}")]
        public async Task<HttpStatusCode> StaticWindow(string clientId)
        {
            try
            {
                _logger.LogInformation($"[DosProtectionController:StaticWindow] Starts validating if clientId: {clientId} is permitted.");
                // Fetch the client's IP address.
                string clientIpAddress = HttpContext.Connection.RemoteIpAddress.ToString();

                if(await Task.Run(() => _dosProtectionService.ProcessClientRequest(clientId, clientIpAddress, ProtectionType.Static)))
                {
                    _logger.LogInformation($"[DosProtectionController:StaticWindow] Client: {clientId} with IP address: {clientIpAddress} is permitted.");
                    return HttpStatusCode.OK;
                }
                else
                {
                    _logger.LogInformation($"[DosProtectionController:StaticWindow] Client: {clientId} with IP address: {clientIpAddress} is not permitted.");
                    return HttpStatusCode.ServiceUnavailable;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[DosProtectionController:StaticWindow] Error occurred while validating if clientId: {clientId} is permitted. Error: {ex.Message}");
                return HttpStatusCode.InternalServerError;
            }
        }

        [HttpGet("DynamicWindow/{clientId}")]
        public async Task<HttpStatusCode> DynamicWindow(string clientId)
        {
            try
            {
                _logger.LogInformation($"[DosProtectionController:StaticWindow] Starts validating if clientId: {clientId} is permitted.");
                // Fetch the client's IP address.
                string clientIpAddress = HttpContext.Connection.RemoteIpAddress.ToString();

                bool result = await Task.Run(() => _dosProtectionService.ProcessClientRequest(clientId, clientIpAddress, ProtectionType.Dynamic));
                if (result)
                {
                    _logger.LogInformation($"[DosProtectionController:DynamicWindow] ClientId: {clientId} with IP address: {clientIpAddress} is permitted.");
                    return HttpStatusCode.OK;
                }
                else
                {
                    _logger.LogInformation($"[DosProtectionController:DynamicWindow] ClientId: {clientId} with IP address: {clientIpAddress} is not permitted.");
                    return HttpStatusCode.ServiceUnavailable;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[DosProtectionController:DynamicWindow] Error occurred while validating if clientId: {clientId} is permitted. Error: {ex.Message}");
                return HttpStatusCode.InternalServerError;
            }
        }
    }
}
