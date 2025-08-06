using System.Security.Cryptography;
using System.Text;

namespace TheVault.App.Services
{
    public class EncryptionService : IEncryptionService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public EncryptionService(string password)
        {
            _key = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            _iv = [.. _key.Take(16)];
        }

        public void EncryptFile(string inputPath, string outputPath)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            using var fsInput = new FileStream(inputPath, FileMode.Open);
            using var fsOutput = new FileStream(outputPath, FileMode.Create);
            using var cryptoStream = new CryptoStream(fsOutput, aes.CreateEncryptor(), CryptoStreamMode.Write);
            fsInput.CopyTo(cryptoStream);
        }

        public void DecryptFile(string inputPath, string outputPath)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            using var fsInput = new FileStream(inputPath, FileMode.Open);
            using var fsOutput = new FileStream(outputPath, FileMode.Create);
            using var cryptoStream = new CryptoStream(fsInput, aes.CreateDecryptor(), CryptoStreamMode.Read);
            cryptoStream.CopyTo(fsOutput);
        }
    }
}
