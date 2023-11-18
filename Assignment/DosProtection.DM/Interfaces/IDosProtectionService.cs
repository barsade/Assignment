using Assignment.DosProtection.DM.Enum;

namespace Assignment.DosProtection.DM.Interfaces
{
    // This interface defines the contract for the DosProtectionService.
    public interface IDosProtectionService
    {
        bool ProcessClientRequest(string clientIdentifier, string ipAddress, ProtectionType protectionType);
    }
}
