using System.Security.Cryptography;
using System.Text;
using TheVault.App.Models;

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

        public void EncryptFile(FileOperationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.InputPath) || !File.Exists(request.InputPath))
                throw new ArgumentException("Input file does not exist.", nameof(request.InputPath));

            string directory = request.OutputPath;
            if (string.IsNullOrWhiteSpace(directory))
                directory = Path.GetDirectoryName(request.InputPath) ?? Directory.GetCurrentDirectory();

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string fileName = !string.IsNullOrWhiteSpace(request.OutputFileName)
                ? request.OutputFileName
                : Path.GetFileName(request.InputPath) + ".enc";

            string finalOutputPath = Path.Combine(directory, fileName);

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            using var fsInput = new FileStream(request.InputPath, FileMode.Open, FileAccess.Read);
            using var fsOutput = new FileStream(finalOutputPath, FileMode.Create, FileAccess.Write);
            using var cryptoStream = new CryptoStream(fsOutput, aes.CreateEncryptor(), CryptoStreamMode.Write);
            fsInput.CopyTo(cryptoStream);
        }

        public void DecryptFile(FileOperationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.InputPath) || !File.Exists(request.InputPath))
                throw new ArgumentException("Input file does not exist.", nameof(request.InputPath));

            string directory = request.OutputPath;
            if (string.IsNullOrWhiteSpace(directory))
                directory = Path.GetDirectoryName(request.InputPath) ?? Directory.GetCurrentDirectory();

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string fileName;
            if (!string.IsNullOrWhiteSpace(request.OutputFileName))
            {
                fileName = request.OutputFileName;
            }
            else
            {
                fileName = Path.GetFileNameWithoutExtension(request.InputPath);
                if (Path.GetExtension(request.InputPath).Equals(".enc", StringComparison.OrdinalIgnoreCase))
                {
                    fileName = Path.GetFileNameWithoutExtension(request.InputPath);
                }
            }

            string finalOutputPath = Path.Combine(directory, fileName);

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            using var fsInput = new FileStream(request.InputPath, FileMode.Open, FileAccess.Read);
            using var fsOutput = new FileStream(finalOutputPath, FileMode.Create, FileAccess.Write);
            using var cryptoStream = new CryptoStream(fsInput, aes.CreateDecryptor(), CryptoStreamMode.Read);
            cryptoStream.CopyTo(fsOutput);
        }
    }
}
