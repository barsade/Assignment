using Assignment.DosProtection.DM.Enum;
using Assignment.Utils;

namespace Assignment.DosProtection.DM.Models
{
    // This is an internal class representing the state of a specific client.
    public class DosClient
    {
        // This field keeps track of the number of requests made by the client.
        private int requestCounter = 0;
        private int maxRequestsPerFrame = 5;
        private int timeFrameInSeconds = 5;

        // This field stores the timestamp of the client's last request.
        private DateTime requestTime;

        // This object is used for locking to ensure thread safety.
        private readonly object lockObject = new object();

        public DosClient()
        {

        }

        public bool CheckIPAddress(string ipAddress)
        {
            return true;
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
                if (requestCounter == 0 || now - requestTime > TimeSpan.FromSeconds(timeFrameInSeconds))
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
                        return false;
                    }
                    else if (protectionType == ProtectionType.Static)
                    {
                        // Is intended for dealing with static window, where unlike dynamic window,
                        // the static window does not change the time frame until it ends.
                        // Do nothing.
                    }
                    // Dynamically slide the time frame.
                    else
                    {
                        requestTime = now;
                    }
                }

                // The client is allowed to make another request.
                return true;
            }
        }
    }
}
