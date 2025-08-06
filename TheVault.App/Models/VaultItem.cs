namespace TheVault.App.Models
{
    public class VaultItem
    {
        public required string FileName { get; set; }
        public required string EncryptedPath { get; set; }
    }
}