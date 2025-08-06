using System.Text.Json;
using TheVault.App.Services;

namespace TheVault.App.Repositories
{
    public class VaultConfig
    {
        public string PasswordHash { get; set; } = "";
        public List<string> Notes { get; set; } = [];
    }

    public class VaultRepository
    {
        private const string ConfigFile = "vault.json";
        private readonly IHashService _hashService;
        private VaultConfig _config;

        public VaultRepository(IHashService hashService)
        {
            _hashService = hashService;
            _config = LoadConfig();
        }

        private VaultConfig LoadConfig()
        {
            if (!File.Exists(ConfigFile))
                return new VaultConfig();

            var json = File.ReadAllText(ConfigFile);
            return JsonSerializer.Deserialize<VaultConfig>(json) ?? new VaultConfig();
        }

        private void SaveConfig()
        {
            JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true };
            JsonSerializerOptions options = jsonSerializerOptions;
            string json = JsonSerializer.Serialize(_config, options);
            File.WriteAllText(ConfigFile, json);
        }

        public bool PasswordExists() => !string.IsNullOrEmpty(_config.PasswordHash);

        public void SavePasswordHash(string password)
        {
            _config.PasswordHash = _hashService.HashPassword(password);
            SaveConfig();
        }

        public bool VerifyPassword(string password)
        {
            return _hashService.VerifyPassword(password, _config.PasswordHash);
        }

        public void AddNote(string encryptedNotePath)
        {
            _config.Notes.Add(encryptedNotePath);
            SaveConfig();
        }

        public List<string> GetNotes() => _config.Notes;
    }
}
