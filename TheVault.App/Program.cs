using TheVault.App.Repositories;
using TheVault.App.Services;
using TheVault.App.Areas;

Console.WriteLine("Velkommen til TheVault!");

Console.Write("Indtast master password: ");
var password = Console.ReadLine();

var hashService = new HashService();
var repo = new VaultRepository(hashService);

if (!repo.PasswordExists())
{
    Console.WriteLine("Intet password fundet. Gemmer nyt hash...");
    repo.SavePasswordHash(password);
}
else if (!repo.VerifyPassword(password))
{
    Console.WriteLine("Forkert password!");
    return;
}

var encryptionService = new EncryptionService(password);
var menu = new MainMenu(encryptionService, repo);
menu.Show();
