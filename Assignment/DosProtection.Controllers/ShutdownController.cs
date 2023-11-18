using Assignment.DosProtection.DM;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
            _logger = logger;
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
            catch (Exception e)
            {
                _logger.LogError($"[ShutdownController:Shutdown] An error occurred while processing key signal: {e}");
                return HttpStatusCode.InternalServerError;
            }

        }
    }
}
