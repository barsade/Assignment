using Assignment.DosProtection.DM.Enum;

namespace Assignment.DosProtection.DM.Interfaces
{
    public interface IDosProtectionClient
    {
        public bool CheckRequestRate();
    }
}
