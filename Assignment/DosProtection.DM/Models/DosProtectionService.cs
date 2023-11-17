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
        private readonly IDosProtectionClient _dosProtectionClient;
        private readonly IServiceProvider _serviceProvider;

        public DosProtectionService(IMemoryCache memoryCache, IServiceProvider serviceProvider,
            IConfiguration config)
        {
            _serviceProvider = serviceProvider;
            //_dosProtectionClient = dosProtectionClient;
            _cache = memoryCache;
            _config = config;
        }

        // This method checks if the client identified by clientId can make another request.
        // This method also checks if the client's IP address can make another request.
        public bool CheckRequestRate(string clientId, string clientIpAddress, ProtectionType protectionType)
        {
            var windowClients = protectionType == ProtectionType.Static ? _staticWindowClients : _dynamicWindowClients;

            // Get or add a DosProtectionClient instance for the clientId.
            var dosClient = windowClients.GetOrAdd(clientId, entry => _serviceProvider.GetRequiredService<IDosProtectionClient>());
            var dosClientIp = _cache.GetOrCreate(clientIpAddress, entry => _serviceProvider.GetRequiredService<IDosProtectionClient>());

            // Call the CheckRequestRate method of the DosProtectionClient instance.
            return dosClient.CheckRequestRate(protectionType) && dosClientIp.CheckRequestRate(protectionType);
        }

        private IDosProtectionClient CreateDosProtectionClient(ProtectionType protectionType)
        {
            // You can use a factory method or dependency injection to create instances of IDosProtectionClient.
            return new DosProtectionClient(_config);
        }
    }
}
