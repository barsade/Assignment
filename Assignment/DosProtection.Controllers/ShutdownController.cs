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

        // This API endpoint serves as a mock to simulate a key press event pressed by the server,
        // since Web API is stateless and cannot get user input (only in the form of HTTP requests).
        // Besides the trigger of the key press (input event vs HTTP request), the logic implemented is the same
        // once the event is triggered.
        // The listener for the key press event is implemented in the Program.cs file in a separate task running
        // throughout the application's lifetime in the background.
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
