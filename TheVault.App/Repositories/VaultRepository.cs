using System.Text.Json;
using TheVault.App.Models;
using TheVault.App.Services;

namespace TheVault.App.Repositories
{
    public class VaultRepository
    {
        private readonly string _configFile;
        private readonly IHashService _hashService;
        private VaultConfig _config;

        public VaultRepository(IHashService hashService, string? configFile = null)
        {
            _hashService = hashService;
            _configFile = string.IsNullOrWhiteSpace(configFile) ? "vault.json" : configFile;
            _config = LoadConfig();
        }

        private VaultConfig LoadConfig()
        {
            if (!File.Exists(_configFile))
                return new VaultConfig();

            var json = File.ReadAllText(_configFile);
            return JsonSerializer.Deserialize<VaultConfig>(json) ?? new VaultConfig();
        }

        private void SaveConfig()
        {
            var json = JsonSerializer.Serialize(_config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_configFile, json);
        }

        public bool PasswordExists() => !string.IsNullOrWhiteSpace(_config.PasswordHash);

        public void SavePasswordHash(string password)
        {
            _config.PasswordHash = _hashService.HashPassword(password);
            SaveConfig();
        }

        public bool VerifyPassword(string password)
        {
            return _hashService.VerifyPassword(password, _config.PasswordHash);
        }

        public void AddEncryptedFile(string encryptedFilePath)
        {
            if (!_config.EncryptedFiles.Contains(encryptedFilePath))
            {
                _config.EncryptedFiles.Add(encryptedFilePath);
                SaveConfig();
            }
        }

        public List<string> GetEncryptedFiles() => _config.EncryptedFiles;

        public void AddNote(string encryptedNotePath)
        {
            if (!_config.Notes.Contains(encryptedNotePath))
            {
                _config.Notes.Add(encryptedNotePath);
                SaveConfig();
            }
        }

        public List<string> GetNotes() => _config.Notes;
    }
}
