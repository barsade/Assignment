namespace Assignment.Utils
{
    public class Constants
    {
        private readonly IConfiguration _config;
        public Constants(IConfiguration config)
        {
            _config = config;
        }

        public static readonly string MAX_REQUESTS_PER_FRAME = "MaxRequestsAllowed";
        public static readonly string TIME_FRAME_THRESHOLD = "TimeFrameTreshold";
    }
}
