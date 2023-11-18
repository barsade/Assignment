using System.Net;
using Microsoft.AspNetCore.Mvc;
using Assignment.DosProtection.DM.Enum;
using Assignment.DosProtection.DM.Interfaces;

namespace Assignment.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DosProtectionController : ControllerBase
    {
        private readonly IDosProtectionService _dosProtectionService;
        private readonly ILogger<DosProtectionController> _logger;

        public DosProtectionController(IDosProtectionService dosProtectionService,
            ILogger<DosProtectionController> logger)
        {
            _dosProtectionService = dosProtectionService;
            _logger = logger;
        }

        [HttpGet("StaticWindow/{clientId}")]
        public Task<HttpStatusCode> StaticWindow(string clientId) => HandleProtection(clientId, ProtectionType.Static);

        [HttpGet("DynamicWindow/{clientId}")]
        public Task<HttpStatusCode> DynamicWindow(string clientId) => HandleProtection(clientId, ProtectionType.Dynamic);

        private async Task<HttpStatusCode> HandleProtection(string clientId, ProtectionType protectionType)
        {
            try
            {
                _logger.LogInformation($"[DosProtectionController:{protectionType}Window] Starts validating if client ID: {clientId} is permitted.");

                if (HttpContext.Connection.RemoteIpAddress == null)
                {
                    throw new Exception("IP address is null.");
                }

                // Fetch the client's IP address.
                string clientIpAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                bool result = await Task.Run(() => _dosProtectionService.ProcessClientRequest(clientId, clientIpAddress, protectionType));

                if (result)
                {
                    _logger.LogInformation($"[DosProtectionController:{protectionType}Window] Client ID: {clientId} with IP address: {clientIpAddress} is permitted.");
                    return HttpStatusCode.OK;
                }
                else
                {
                    _logger.LogInformation($"[DosProtectionController:{protectionType}Window] Client ID: {clientId} with IP address: {clientIpAddress} is not permitted.");
                    return HttpStatusCode.ServiceUnavailable;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"[DosProtectionController:{protectionType}Window] An error occurred while validating if client ID: {clientId} is permitted. Error: {e}");
                return HttpStatusCode.InternalServerError;
            }
        }
    }
}
