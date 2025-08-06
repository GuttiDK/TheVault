namespace TheVault.App.Services
{
    public interface IEncryptionService
    {
        void EncryptFile(string inputPath, string outputPath);
        void DecryptFile(string inputPath, string outputPath);
    }
}
