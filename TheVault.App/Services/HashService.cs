using System.Security.Cryptography;
using System.Text;

namespace TheVault.App.Services
{
    public class HashService : IHashService
    {
        // Hashes a password using SHA256 and returns the hash as a hex string
        public string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }

        // Verifies a password against a stored hash (hex string)
        public bool VerifyPassword(string password, string storedHash)
        {
            var hashOfInput = HashPassword(password);
            // Use OrdinalIgnoreCase to match Convert.ToHexString output
            return string.Equals(hashOfInput, storedHash, StringComparison.OrdinalIgnoreCase);
        }
    }
}
