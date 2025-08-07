using TheVault.App.Models;
using TheVault.App.Repositories;
using TheVault.App.Services;

Directory.CreateDirectory("vault_files");
Directory.CreateDirectory("vault_notes");

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

while (true)
{
    Console.WriteLine("\nVælg en handling:");
    Console.WriteLine("1. Tilføj fil");
    Console.WriteLine("2. Vis krypterede filer");
    Console.WriteLine("3. Dekrypter fil");
    Console.WriteLine("4. Tilføj note");
    Console.WriteLine("5. Vis noter");
    Console.WriteLine("6. Afslut");

    Console.Write("Valg: ");
    var valg = Console.ReadLine();

    if (valg == "1")
    {
        Console.Write("Sti til fil: ");
        var path = Console.ReadLine();

        if (!File.Exists(path))
        {
            Console.WriteLine("Filen blev ikke fundet.");
            continue;
        }

        string fileName = Path.GetFileName(path);
        string encryptedPath = $"vault_files/{fileName}.vault";
        encryptionService.EncryptFile(path, encryptedPath);
        Console.WriteLine($"✅ Fil krypteret og gemt som: {encryptedPath}");
    }
    else if (valg == "2")
    {
        var files = Directory.GetFiles("vault_files");
        if (files.Length == 0)
        {
            Console.WriteLine("Ingen krypterede filer fundet.");
            continue;
        }

        Console.WriteLine("Krypterede filer:");
        foreach (var file in files)
            Console.WriteLine($"- {Path.GetFileName(file)}");
    }
    else if (valg == "3")
    {
        Console.Write("Navn på krypteret fil (uden sti): ");
        var fileName = Console.ReadLine();
        string inputPath = $"vault_files/{fileName}";
        if (!File.Exists(inputPath))
        {
            Console.WriteLine("Filen blev ikke fundet.");
            continue;
        }

        string outputPath = $"decrypted_{fileName.Replace(".vault", "")}";
        encryptionService.DecryptFile(inputPath, outputPath);
        Console.WriteLine($"✅ Fil dekrypteret til: {outputPath}");
    }
    else if (valg == "4")
    {
        Console.Write("Skriv din note: ");
        var noteText = Console.ReadLine();
        string noteFileName = $"note_{DateTime.Now:yyyyMMddHHmmss}.note";
        string notePath = $"vault_notes/{noteFileName}";

        File.WriteAllText("temp_note.txt", noteText);
        encryptionService.EncryptFile("temp_note.txt", notePath);
        File.Delete("temp_note.txt");

        repo.AddNote(notePath);
        Console.WriteLine("✅ Note krypteret og gemt.");
    }
    else if (valg == "5")
    {
        var notes = repo.GetNotes();
        if (notes.Count == 0)
        {
            Console.WriteLine("Ingen noter fundet.");
            continue;
        }

        Console.WriteLine("Dine noter:");
        foreach (var notePath in notes)
        {
            try
            {
                string temp = "temp_decrypted_note.txt";
                encryptionService.DecryptFile(notePath, temp);
                string content = File.ReadAllText(temp);
                File.Delete(temp);

                Console.WriteLine($"\n📄 {Path.GetFileName(notePath)}:\n{content}");
            }
            catch
            {
                Console.WriteLine($"⚠️ Kunne ikke læse note: {notePath}");
            }
        }
    }
    else if (valg == "6")
    {
        Console.WriteLine("Farvel 👋");
        break;
    }
    else
    {
        Console.WriteLine("Ugyldigt valg. Prøv igen.");
    }
}
