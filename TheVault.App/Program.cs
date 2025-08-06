using ConsoleVault.App.Services;
using ConsoleVault.App.Repositories;
using ConsoleVault.App.Models;

Console.WriteLine("Velkommen til ConsoleVault!");

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

Console.WriteLine("1. Tilføj fil\n2. Vis filer\n3. Dekrypter fil");
var valg = Console.ReadLine();

if (valg == "1")
{
    Console.Write("Sti til fil: ");
    var path = Console.ReadLine();
    var fileName = Path.GetFileName(path);
    encryptionService.EncryptFile(path, $"vault_files/{fileName}.vault");
    Console.WriteLine("Fil krypteret!");
}
else if (valg == "2")
{
    var files = Directory.GetFiles("vault_files");
    foreach (var file in files)
        Console.WriteLine(Path.GetFileName(file));
}
else if (valg == "3")
{
    Console.Write("Navn på fil: ");
    var fileName = Console.ReadLine();
    encryptionService.DecryptFile($"vault_files/{fileName}", $"decrypted_{fileName}");
    Console.WriteLine("Fil dekrypteret!");
}
