using System.Collections.Concurrent;
using Assignment.DosProtection.DM.Enum;
using Assignment.DosProtection.DM.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Assignment.DosProtection.DM.Models
{
    // This is the implementation of the DosProtectionService.
    public class DosProtectionService : IDosProtectionService
    {
        // This dictionary stores DosProtectionClient instances for each client identified by clientId.
        private readonly ConcurrentDictionary<string, IDosProtectionClient> _staticWindowClients = new ConcurrentDictionary<string, IDosProtectionClient>();
        private readonly ConcurrentDictionary<string, IDosProtectionClient> _dynamicWindowClients = new ConcurrentDictionary<string, IDosProtectionClient>();
        private readonly IMemoryCache _cache;

        public DosProtectionService(IMemoryCache cache, IDosProtectionClient dosProtectionClient)
        {
            _cache = cache;
        }

        // This method checks if the client identified by clientId can make another request.
        public bool CheckRequestRate(string clientId, string ipAddress, ProtectionType protectionType)
        {
            var windowClients = protectionType == ProtectionType.Static ? _staticWindowClients : _dynamicWindowClients;

            // Get or add a DosProtectionClient instance for the clientId.
            var dosClient = windowClients.GetOrAdd(clientId, new DosProtectionClient());

            // Call the CheckRequestRate method of the DosProtectionClient instance.
            return dosClient.CheckRequestRate(protectionType);
        }
    }
}
