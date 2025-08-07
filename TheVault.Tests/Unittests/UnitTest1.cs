using TheVault.App.Services;
using TheVault.App.Repositories;

namespace TheVault.Tests.Unittests
{
    public class HashServiceTests
    {
        [Fact]
        public void HashPassword_ShouldBeSameForSameInput()
        {
            var hashService = new HashService();
            var hash1 = hashService.HashPassword("test123");
            var hash2 = hashService.HashPassword("test123");
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void HashPassword_ShouldHandleMaxLengthString()
        {
            var hashService = new HashService();
            string longPassword = new string('a', 10000);
            var hash = hashService.HashPassword(longPassword);
            Assert.False(string.IsNullOrEmpty(hash));
        }

        [Fact]
        public void HashPassword_ShouldBeDifferentForDifferentInput()
        {
            var hashService = new HashService();
            var hash1 = hashService.HashPassword("password1");
            var hash2 = hashService.HashPassword("password2");
            Assert.NotEqual(hash1, hash2);
        }
    }

    public class VaultRepositoryTests : IDisposable
    {
        private readonly string _testConfigFile;
        private readonly VaultRepository _repo;

        public VaultRepositoryTests()
        {
            // Brug en unik test-fil for hver testinstans for at undgå filkonflikter
            _testConfigFile = $"vault_test_{Guid.NewGuid()}.json";
            _repo = new VaultRepository(new HashServiceForTest(), _testConfigFile);
        }

        [Fact]
        public void AddEncryptedFile_ShouldAddFilePath()
        {
            string filePath = $"testfile_{Guid.NewGuid()}.vault";
            _repo.AddEncryptedFile(filePath);

            Assert.Contains(filePath, _repo.GetEncryptedFiles());
        }

        [Fact]
        public void AddEncryptedFile_ShouldNotAddDuplicate()
        {
            string filePath = $"testfile_{Guid.NewGuid()}.vault";
            _repo.AddEncryptedFile(filePath);
            _repo.AddEncryptedFile(filePath);

            var files = _repo.GetEncryptedFiles().Where(f => f == filePath).ToList();
            Assert.Single(files);
        }

        [Fact]
        public void AddNote_ShouldAddNotePath()
        {
            string notePath = $"testnote_{Guid.NewGuid()}.note";
            _repo.AddNote(notePath);

            Assert.Contains(notePath, _repo.GetNotes());
        }

        [Fact]
        public void AddNote_ShouldNotAddDuplicate()
        {
            string notePath = $"testnote_{Guid.NewGuid()}.note";
            _repo.AddNote(notePath);
            _repo.AddNote(notePath);

            var notes = _repo.GetNotes().Where(n => n == notePath).ToList();
            Assert.Single(notes);
        }

        [Fact]
        public void SavePasswordHash_And_VerifyPassword_ShouldWork()
        {
            string password = "securePassword!";
            _repo.SavePasswordHash(password);

            Assert.True(_repo.VerifyPassword(password));
            Assert.False(_repo.VerifyPassword("wrongPassword"));
        }

        [Fact]
        public void GetEncryptedFiles_And_Notes_ShouldReturnEmptyList_WhenNoneAdded()
        {
            var repo = new VaultRepository(new HashServiceForTest(), $"empty_{Guid.NewGuid()}.json");
            Assert.Empty(repo.GetEncryptedFiles());
            Assert.Empty(repo.GetNotes());
        }

        public void Dispose()
        {
            if (File.Exists(_testConfigFile))
                File.Delete(_testConfigFile);
        }
    }

    // Dummy hashservice for hurtigere tests (ingen rigtig hashing)
    public class HashServiceForTest : IHashService
    {
        public string HashPassword(string password) => password + "_hash";
        public bool VerifyPassword(string password, string storedHash) => HashPassword(password) == storedHash;
    }
}