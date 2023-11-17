using System.Collections.Concurrent;
using Assignment.DosProtection.DM.Enum;
using Assignment.DosProtection.DM.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Assignment.DosProtection.DM.Models
{
    // This is the implementation of the DosProtectionService.
    public class DosProtectionService : IDosProtectionService
    {
        // This dictionary stores DosProtectionClient instances for each client identified by clientId.
        private readonly ConcurrentDictionary<string, IDosProtectionClient> _staticWindowClients = new ConcurrentDictionary<string, IDosProtectionClient>();
        private readonly ConcurrentDictionary<string, IDosProtectionClient> _dynamicWindowClients = new ConcurrentDictionary<string, IDosProtectionClient>();
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _config;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DosProtectionService> _logger;

        public DosProtectionService(IMemoryCache memoryCache, IServiceProvider serviceProvider,
            IConfiguration config, ILogger<DosProtectionService> logger)
        {
            _serviceProvider = serviceProvider;
            _cache = memoryCache;
            _config = config;
            _logger = logger;
        }

        /// <summary>
        /// Checks if the client can make another request within the specified time frame
        /// based on the client's IP address and client identifier.
        /// </summary>
        /// <returns>
        ///   True if the client is allowed to make another request within the defined limits; otherwise, false.
        /// </returns>
        public bool CheckRequestRate(string clientId, string clientIpAddress, ProtectionType protectionType)
        {
            _logger.LogDebug($"[DosProtectionService:CheckRequestRate] Starts processing the request of clientId: {clientId} with IP address: {clientIpAddress}.");
            var windowClients = protectionType == ProtectionType.Static ? _staticWindowClients : _dynamicWindowClients;

            // Get or add a DosProtectionClient instance for the clientId from the relevant concurrent dictionary.
            var dosClient = windowClients.GetOrAdd(clientId, entry => _serviceProvider.GetRequiredService<IDosProtectionClient>());

            // Get or add a DosProtectionClient instance for the client's IP address from cache.
            var dosClientIp = _cache.GetOrCreate(clientIpAddress, entry => _serviceProvider.GetRequiredService<IDosProtectionClient>());

            // Call the CheckRequestRate method of the DosProtectionClient instance.
            return dosClient.CheckRequestRate(protectionType) && dosClientIp.CheckRequestRate(protectionType);
        }
    }
}
