using System.Collections.Concurrent;
using Assignment.DosProtection.DM.Enum;
using Assignment.DosProtection.DM.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Assignment.DosProtection.DM.Models
{
    // This is the implementation of the DosProtectionService.
    public class DosProtectionService : IDosProtectionService
    {
        // This dictionary stores DosClient instances for each client identified by clientIdentifier.
        private readonly DosClient _dosClient;
        private readonly ConcurrentDictionary<string, DosClient> _staticWindowClients = new ConcurrentDictionary<string, DosClient>();
        private readonly ConcurrentDictionary<string, DosClient> _dynamicWindowClients = new ConcurrentDictionary<string, DosClient>();
        private readonly IMemoryCache _cache;

        public DosProtectionService(DosClient dosClient, IMemoryCache cache)
        {
            _cache = cache;
            _dosClient = dosClient;
        }

        // This method checks if the client identified by clientId can make another request.
        public bool CheckRequestRate(string clientId, string ipAddress, ProtectionType protectionType)
        {
            var windowClients = protectionType == ProtectionType.Static ? _staticWindowClients : _dynamicWindowClients;

            // Get or add a DosClient instance for the clientId.
            var dosClient = windowClients.GetOrAdd(clientId, _dosClient);

            // Call the CheckRequestRate method of the DosClient instance.
            return dosClient.CheckRequestRate(protectionType);
        }
    }
}
