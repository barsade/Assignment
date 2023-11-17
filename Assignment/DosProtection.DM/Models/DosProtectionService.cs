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
        private readonly ConcurrentDictionary<string, DosClient> _dosClients;
        private readonly IMemoryCache _cache;

        public DosProtectionService(ConcurrentDictionary<string, DosClient> dosClients, DosClient dosClient,
            IMemoryCache cache)
        {
            _cache = cache;
            _dosClients = dosClients;
            _dosClient = dosClient;
        }

        // This method checks if the client identified by clientIdentifier can make another request.
        public bool CheckRequestRate(string clientId, string ipAddress, ProtectionType protectionType)
        {
            // Get or add a DosClient instance for the clientIdentifier.
            var dosClient = _dosClients.GetOrAdd(clientId, _dosClient);
            var cacheClient = _cache.GetOrCreate(clientId, entry => dosClient);

            // Call the CheckRequestRate method of the DosClient instance.
            return dosClient.CheckRequestRate(protectionType) && cacheClient.CheckIPAddress(ipAddress);
        }

        // This method releases the client identified by clientIdentifier.
        private bool ReleaseClient(string clientIdentifier)
        {
            return _dosClients.TryRemove(clientIdentifier, out _);
        }
    }
}
