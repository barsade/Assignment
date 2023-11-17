using System.Collections.Concurrent;
using Assignment.DosProtection.DM.Enum;
using Assignment.DosProtection.DM.Interfaces;

namespace Assignment.DosProtection.DM.Models
{
    // This is the implementation of the DosProtectionService.
    public class DosProtectionService : IDosProtectionService
    {
        // This dictionary stores DosClient instances for each client identified by clientIdentifier.
        private readonly DosClient _dosClient;
        private readonly ConcurrentDictionary<string, DosClient> _dosClients;

        public DosProtectionService(ConcurrentDictionary<string, DosClient> dosClients, DosClient dosClient)
        {
            _dosClients = dosClients;
            _dosClient = dosClient;
        }

        // This method checks if the client identified by clientIdentifier can make another request.
        public bool CheckRequestRate(string clientIdentifier, ProtectionType protectionType)
        {
            // Get or add a DosClient instance for the clientIdentifier.
            var dosClient = _dosClients.GetOrAdd(clientIdentifier, _dosClient);

            // Call the CheckRequestRate method of the DosClient instance.
            return dosClient.CheckRequestRate(protectionType);
        }
    }
}
