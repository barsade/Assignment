using Assignment.DosProtection.DM;
using Assignment.DosProtection.DM.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static System.Net.Mime.MediaTypeNames;

namespace Assignment.DosProtection.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ShutdownController : ControllerBase
    {
        private readonly KeySignalEvent _processEvent;
        private readonly ILogger<ShutdownController> _logger;

        public ShutdownController(KeySignalEvent processEvent, ILogger<ShutdownController> logger)
        {
            _processEvent = processEvent;
        }

        // HTTP endpoint that mocks the key press signal.
        [HttpGet("shutdown/{key}")]
        public HttpStatusCode Shutdown(string key)
        {
            _logger.LogDebug($"[ShutdownController:Shutdown] Key signal received: {key}");
            try
            {
                _processEvent.OnHttpRequestReceived(key);
                return HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ShutdownController:Shutdown] Error: {ex.Message}");
                return HttpStatusCode.InternalServerError;
            }

        }
    }
}
