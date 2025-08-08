using System.Security.Cryptography;
using System.Text;
using TheVault.App.Models;

namespace TheVault.App.Services
{
    /// <summary>
    /// EncryptionService provides AES-based file encryption and decryption.
    /// The encryption key and IV are derived from the user's password.
    /// </summary>
    public class EncryptionService : IEncryptionService
    {
        // AES key (256-bit) derived from the user's password using SHA256.
        private readonly byte[] _key;
        // AES IV (128-bit) is the first 16 bytes of the key.
        private readonly byte[] _iv;

        /// <summary>
        /// Initializes the encryption service by deriving a key and IV from the password.
        /// </summary>
        /// <param name="password">The user's password (used for key derivation).</param>
        public EncryptionService(string password)
        {
            // Hash the password to get a 256-bit key.
            _key = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            // Use the first 16 bytes of the key as the IV (not recommended for production, but simple for this use case).
            _iv = [.. _key.Take(16)];
        }

        /// <summary>
        /// Encrypts a file using AES and writes the encrypted output to the specified location.
        /// </summary>
        /// <param name="request">FileOperationRequest with input/output paths and file name.</param>
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

            // Create and configure AES for encryption
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            using var fsInput = new FileStream(request.InputPath, FileMode.Open, FileAccess.Read);
            using var fsOutput = new FileStream(finalOutputPath, FileMode.Create, FileAccess.Write);
            // CryptoStream encrypts data as it is written to the output file
            using var cryptoStream = new CryptoStream(fsOutput, aes.CreateEncryptor(), CryptoStreamMode.Write);
            fsInput.CopyTo(cryptoStream);
        }

        /// <summary>
        /// Decrypts an AES-encrypted file and writes the decrypted output to the specified location.
        /// </summary>
        /// <param name="request">FileOperationRequest with input/output paths and file name.</param>
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

            // Create and configure AES for decryption
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            using var fsInput = new FileStream(request.InputPath, FileMode.Open, FileAccess.Read);
            using var fsOutput = new FileStream(finalOutputPath, FileMode.Create, FileAccess.Write);
            // CryptoStream decrypts data as it is read from the input file
            using var cryptoStream = new CryptoStream(fsInput, aes.CreateDecryptor(), CryptoStreamMode.Read);
            cryptoStream.CopyTo(fsOutput);
        }
    }
}
