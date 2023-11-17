using Assignment.DosProtection.DM.Enum;
using Assignment.DosProtection.DM.Interfaces;
using Assignment.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace Assignment.DosProtection.DM.Models
{
    public class DosProtectionClient : IDosProtectionClient
    {

        // This field keeps track of the number of requests made by the client.
        private int requestCounter = 0;
        private static int maxRequestsPerFrame;
        private static int timeFrameThreshold;

        // This field stores the timestamp of the client's last request.
        private DateTime requestTime;
        private ProtectionType _protectionType;
        private readonly IConfiguration _config;

        // This object is used for locking to ensure thread safety.
        private readonly object lockObject = new object();

        public DosProtectionClient(IConfiguration config)
        {
            _config = config;
            maxRequestsPerFrame = int.Parse(_config[Constants.MAX_REQUESTS_PER_FRAME]);
            timeFrameThreshold = int.Parse(_config[Constants.TIME_FRAME_TRESHOLD]);
        }

        public bool CheckRequestRate(ProtectionType protectionType)
        {
            // Lock the object to ensure thread safety.
            lock (lockObject)
            {
                // Get the current time.
                var now = DateTime.UtcNow;

                // If the client hasn't made any requests or the last request was more than 5 seconds ago,
                // start a new time frame.
                if (requestCounter == 0 || now - requestTime > TimeSpan.FromSeconds(15))
                {
                    requestTime = now;  // Update the last request time.
                    requestCounter = 1;   // Reset the request count.
                }
                else
                {
                    requestCounter++;

                    // If the client has made more than 5 requests within the time frame, return an error.
                    if (requestCounter > maxRequestsPerFrame)
                    {
                        // If the protection type is dynamic, update the last request time.
                        if (_protectionType == ProtectionType.Dynamic)
                        {
                            requestTime = now;
                        }
                        return false;
                    }
                }

                // The client is allowed to make another request.
                return true;
            }
        }
    }
}
