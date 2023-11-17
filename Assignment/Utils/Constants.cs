namespace Assignment.Utils
{
    public class Constants
    {
        private readonly IConfiguration _config;
        public Constants(IConfiguration config)
        {
            _config = config;
        }
        public static readonly string STATIC_REQUESTS_ALLOWED = "";
        public static readonly string DYNAMIC_REQUESTS_ALLOWED = "Dynamic_Requests_Allowed";
    }
}
