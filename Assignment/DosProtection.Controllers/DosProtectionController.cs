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
        public Task<HttpStatusCode> StaticWindow(string clientId) => HandleProtection(clientId, ProtectionType.Static);

        [HttpGet("DynamicWindow/{clientId}")]
        public Task<HttpStatusCode> DynamicWindow(string clientId) => HandleProtection(clientId, ProtectionType.Dynamic);

        private async Task<HttpStatusCode> HandleProtection(string clientId, ProtectionType protectionType)
        {
            try
            {
                _logger.LogInformation($"[DosProtectionController:{protectionType}Window] Starts validating if clientId: {clientId} is permitted.");

                // Fetch the client's IP address.
                string clientIpAddress = HttpContext.Connection.RemoteIpAddress.ToString();

                bool result = await Task.Run(() => _dosProtectionService.ProcessClientRequest(clientId, clientIpAddress, protectionType));

                if (result)
                {
                    _logger.LogInformation($"[DosProtectionController:{protectionType}Window] Client: {clientId} with IP address: {clientIpAddress} is permitted.");
                    return HttpStatusCode.OK;
                }
                else
                {
                    _logger.LogInformation($"[DosProtectionController:{protectionType}Window] Client: {clientId} with IP address: {clientIpAddress} is not permitted.");
                    return HttpStatusCode.ServiceUnavailable;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[DosProtectionController:{protectionType}Window] Error occurred while validating if clientId: {clientId} is permitted. Error: {ex.Message}");
                return HttpStatusCode.InternalServerError;
            }
        }
    }
}
