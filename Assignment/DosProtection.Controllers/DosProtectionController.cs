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
            // Fetch the client's IP address.
            string clientIpAddress = HttpContext.Connection.RemoteIpAddress.ToString();

            bool result = await Task.Run(() => _dosProtectionService.CheckRequestRate(clientId, clientIpAddress, ProtectionType.Static));

            if (result)
            {
                return HttpStatusCode.OK;
            }
            else
            {
                return HttpStatusCode.ServiceUnavailable;
            }
        }

        [HttpGet("DynamicWindow/{clientId}")]
        public async Task<HttpStatusCode> DynamicWindow(string clientId)
        {
            // Fetch the client's IP address.
            string clientIpAddress = HttpContext.Connection.RemoteIpAddress.ToString();

            bool result = await Task.Run(() => _dosProtectionService.CheckRequestRate(clientId, clientIpAddress, ProtectionType.Dynamic));

            if (result)
            {
                return HttpStatusCode.OK;
            }
            else
            {
                return HttpStatusCode.ServiceUnavailable;
            }
        }
    }
}
