using TheVault.App.Models;

namespace TheVault.App.Services
{
    public interface IEncryptionService
    {
        void EncryptFile(FileOperationRequest request);
        void DecryptFile(FileOperationRequest request);
    }
}
