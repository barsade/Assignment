using Assignment.DosProtection.DM;
using Assignment.DosProtection.DM.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        // Simulate receiving an HTTP request
        [HttpGet("shutdown/{key}")]
        public IActionResult Shutdown(string key)
        {
            _processEvent.OnHttpRequestReceived(key);
            return Ok();
        }
    }
}
