using System.Net;
using Microsoft.AspNetCore.Mvc;
using Assignment.DosProtection.DM.Enum;
using Assignment.DosProtection.DM.Interfaces;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Http.Features;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.Connections.Features;

namespace Assignment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DosProtectionController : ControllerBase
    {
        private readonly IDosProtectionService _dosProtectionService;

        public DosProtectionController(IDosProtectionService dosProtectionService)
        {
            _dosProtectionService = dosProtectionService;
        }
        
        [HttpGet("StaticWindow/{clientIdentifier}")]
        public async Task<HttpStatusCode> StaticWindow(string clientIdentifier)
        {
            string clientIpAddress = HttpContext.Connection.RemoteIpAddress.ToString();

            if (string.IsNullOrEmpty(clientIdentifier))
            {
                return HttpStatusCode.BadRequest;
            }
            bool result = await Task.Run(() => _dosProtectionService.CheckRequestRate(clientIdentifier, "", ProtectionType.Static));

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
            var connectionFeature = HttpContext.Features.Get<IHttpConnectionFeature>();
            string clientIpAddress = connectionFeature?.RemoteIpAddress.ToString();

            bool result = await Task.Run(() => _dosProtectionService.CheckRequestRate(clientId, "", ProtectionType.Dynamic));

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
