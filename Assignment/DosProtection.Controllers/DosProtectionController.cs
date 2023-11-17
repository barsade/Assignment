﻿using System.Net;
using Microsoft.AspNetCore.Mvc;
using Assignment.DosProtection.DM.Enum;
using Assignment.DosProtection.DM.Interfaces;
using Microsoft.AspNetCore.HttpOverrides;


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

        [HttpGet("StaticWindow/{clientld}")]
        public async Task<HttpStatusCode> StaticWindow(string clientld)
        {
            string clientIp = HttpContext.Connection.RemoteIpAddress.ToString();
            bool result = await Task.Run(() => _dosProtectionService.CheckRequestRate(clientld, clientIp, ProtectionType.Static));

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
            string clientIp = HttpContext.Connection.RemoteIpAddress.ToString();
            bool result = await Task.Run(() => _dosProtectionService.CheckRequestRate(clientId, clientIp, ProtectionType.Dynamic));

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
