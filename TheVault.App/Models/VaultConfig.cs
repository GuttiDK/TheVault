using System.Collections.Generic;

namespace TheVault.App.Models
{
    public class VaultConfig
    {
        public string PasswordHash { get; set; } = string.Empty;
        public List<string> EncryptedFiles { get; set; } = new();
        public List<string> Notes { get; set; } = new();
    }
}