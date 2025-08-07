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

    public class VaultRepositoryTests
    {
        [Fact]
        public void AddNote_ShouldAddNotePath()
        {
            var hashService = new HashService();
            var repo = new VaultRepository(hashService);

            string notePath = "vault_notes/testnote.note";
            repo.AddNote(notePath);

            Assert.Contains(notePath, repo.GetNotes());
        }

        [Fact]
        public void SavePasswordHash_And_VerifyPassword_ShouldWork()
        {
            var hashService = new HashService();
            var repo = new VaultRepository(hashService);

            string password = "securePassword!";
            repo.SavePasswordHash(password);

            Assert.True(repo.VerifyPassword(password));
            Assert.False(repo.VerifyPassword("wrongPassword"));
        }

        [Fact]
        public void GetNotes_ShouldReturnEmptyList_WhenNoNotes()
        {
            var hashService = new HashService();
            var repo = new VaultRepository(hashService);

            var notes = repo.GetNotes();
            Assert.NotNull(notes);
            Assert.Empty(notes);
        }
    }

    public class EncryptionServiceTests
    {
        [Fact]
        public void EncryptAndDecryptFile_ShouldReturnOriginalContent()
        {
            var password = "testPassword";
            var encryptionService = new EncryptionService(password);

            string originalText = "Sensitive data!";
            string inputPath = "test_input.txt";
            string encryptedPath = "test_encrypted.vault";
            string decryptedPath = "test_decrypted.txt";

            File.WriteAllText(inputPath, originalText);

            encryptionService.EncryptFile(inputPath, encryptedPath);
            encryptionService.DecryptFile(encryptedPath, decryptedPath);

            string decryptedText = File.ReadAllText(decryptedPath);

            Assert.Equal(originalText, decryptedText);

            File.Delete(inputPath);
            File.Delete(encryptedPath);
            File.Delete(decryptedPath);
        }
    }
}