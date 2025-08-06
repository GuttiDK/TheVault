using TheVault.App.Services;

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
    }
}