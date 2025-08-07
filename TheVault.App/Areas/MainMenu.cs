using TheVault.App.Models;
using TheVault.App.Services;
using TheVault.App.Repositories;

namespace TheVault.App.Areas
{
    public class MainMenu
    {
        private readonly EncryptionService _encryptionService;
        private readonly VaultRepository _repo;

        private readonly string[] _menuItems = new[]
        {
            "Tilføj fil",
            "Vis krypterede filer",
            "Dekrypter fil",
            "Tilføj note",
            "Vis noter",
            "Statistik",
            "Indstillinger",
            "Afslut"
        };

        public MainMenu(EncryptionService encryptionService, VaultRepository repo)
        {
            _encryptionService = encryptionService;
            _repo = repo;
        }

        public void Show()
        {
            int selected = 0;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Vælg en handling (brug pil op/ned og Enter):\n");
                for (int i = 0; i < _menuItems.Length; i++)
                {
                    if (i == selected)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine($"> {_menuItems[i]}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"  {_menuItems[i]}");
                    }
                }

                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.UpArrow)
                {
                    selected = (selected == 0) ? _menuItems.Length - 1 : selected - 1;
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    selected = (selected + 1) % _menuItems.Length;
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    switch (selected)
                    {
                        case 0: AddFile(); break;
                        case 1: ShowEncryptedFiles(); break;
                        case 2: DecryptFile(); break;
                        case 3: AddNote(); break;
                        case 4: ShowNotes(); break;
                        case 5: ShowStats(); break;
                        case 6: ShowSettings(); break;
                        case 7: Console.WriteLine("Farvel 👋"); return;
                    }
                }
            }
        }

        private void AddFile()
        {
            Console.Clear();
            Console.WriteLine("Eksempel på filsti: C:\\Users\\Christian\\Documents\\minfil.txt");
            Console.Write("Sti til fil der skal krypteres: ");
            var inputPath = Console.ReadLine();

            if (!File.Exists(inputPath))
            {
                Console.WriteLine("Filen blev ikke fundet.");
                Console.WriteLine("Tryk på en vilkårlig tast for at vende tilbage til menuen...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Eksempel på mappe:   C:\\Users\\Christian\\Documents\\Krypteret");
            Console.WriteLine("(Tryk Enter for at bruge samme mappe som inputfilen)");
            Console.Write("Hvor skal den krypterede fil gemmes? (mappe): ");
            var outputDir = Console.ReadLine();

            Console.WriteLine("Eksempel på filnavn: minfil.txt.enc");
            Console.WriteLine("(Tryk Enter for automatisk navn)");
            Console.Write("Navn på krypteret fil: ");
            var outputFileName = Console.ReadLine();

            var request = new FileOperationRequest
            {
                InputPath = inputPath,
                OutputPath = string.IsNullOrWhiteSpace(outputDir) ? null : outputDir,
                OutputFileName = string.IsNullOrWhiteSpace(outputFileName) ? null : outputFileName
            };

            _encryptionService.EncryptFile(request);

            // Save encrypted file path for stats
            string savedPath = System.IO.Path.Combine(
                request.OutputPath ?? System.IO.Path.GetDirectoryName(request.InputPath) ?? System.IO.Directory.GetCurrentDirectory(),
                request.OutputFileName ?? System.IO.Path.GetFileName(request.InputPath) + ".enc"
            );
            _repo.AddEncryptedFile(savedPath);

            Console.WriteLine("✅ Fil krypteret.");
            Console.WriteLine("Tryk på en vilkårlig tast for at vende tilbage til menuen...");
            Console.ReadKey();
        }

        private void ShowEncryptedFiles()
        {
            Console.Clear();
            var files = _repo.GetEncryptedFiles();
            if (files.Count == 0)
            {
                Console.WriteLine("Ingen krypterede filer fundet.");
                Console.WriteLine("Tryk på en vilkårlig tast for at vende tilbage til menuen...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Krypterede filer:");
            foreach (var file in files)
                Console.WriteLine($"- {System.IO.Path.GetFileName(file)}");

            Console.WriteLine("\nTryk på en vilkårlig tast for at vende tilbage til menuen...");
            Console.ReadKey();
        }

        private void DecryptFile()
        {
            Console.Clear();
            Console.WriteLine("Eksempel på filsti: C:\\Users\\Christian\\Documents\\minfil.txt.enc");
            Console.Write("Sti til krypteret fil: ");
            var inputPath = Console.ReadLine();
            if (!File.Exists(inputPath))
            {
                Console.WriteLine("Filen blev ikke fundet.");
                Console.WriteLine("Tryk på en vilkårlig tast for at vende tilbage til menuen...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Eksempel på mappe:   C:\\Users\\Christian\\Documents\\Dekrypteret");
            Console.WriteLine("(Tryk Enter for at bruge samme mappe som inputfilen)");
            Console.Write("Hvor skal den dekrypterede fil gemmes? (mappe): ");
            var outputDir = Console.ReadLine();

            Console.WriteLine("Eksempel på filnavn: minfil.txt");
            Console.WriteLine("(Tryk Enter for automatisk navn)");
            Console.Write("Navn på dekrypteret fil: ");
            var outputFileName = Console.ReadLine();

            var request = new FileOperationRequest
            {
                InputPath = inputPath,
                OutputPath = string.IsNullOrWhiteSpace(outputDir) ? null : outputDir,
                OutputFileName = string.IsNullOrWhiteSpace(outputFileName) ? null : outputFileName
            };

            _encryptionService.DecryptFile(request);
            Console.WriteLine("✅ Fil dekrypteret.");
            Console.WriteLine("Tryk på en vilkårlig tast for at vende tilbage til menuen...");
            Console.ReadKey();
        }

        private void AddNote()
        {
            Console.Clear();
            Console.Write("Skriv din note: ");
            var noteText = Console.ReadLine();

            Console.WriteLine("Eksempel på mappe:   C:\\Users\\Christian\\Documents\\Noter");
            Console.WriteLine("(Tryk Enter for at bruge standardmappe)");
            Console.Write("Hvor skal den krypterede note gemmes? (mappe): ");
            var noteDir = Console.ReadLine();

            Console.WriteLine("Eksempel på filnavn: minnote.txt.enc");
            Console.WriteLine("(Tryk Enter for automatisk navn)");
            Console.Write("Navn på krypteret note: ");
            var noteFileName = Console.ReadLine();

            string tempNotePath = System.IO.Path.GetTempFileName();
            System.IO.File.WriteAllText(tempNotePath, noteText);

            var noteRequest = new FileOperationRequest
            {
                InputPath = tempNotePath,
                OutputPath = string.IsNullOrWhiteSpace(noteDir) ? null : noteDir,
                OutputFileName = string.IsNullOrWhiteSpace(noteFileName) ? null : noteFileName
            };

            _encryptionService.EncryptFile(noteRequest);
            System.IO.File.Delete(tempNotePath);

            // Save the note path for stats
            string savedNotePath = noteRequest.OutputPath ?? System.IO.Path.GetDirectoryName(tempNotePath) ?? System.IO.Directory.GetCurrentDirectory();
            string savedNoteFile = noteRequest.OutputFileName ?? System.IO.Path.GetFileName(tempNotePath) + ".enc";
            _repo.AddNote(System.IO.Path.Combine(savedNotePath, savedNoteFile));

            Console.WriteLine("✅ Note krypteret og gemt.");
            Console.WriteLine("Tryk på en vilkårlig tast for at vende tilbage til menuen...");
            Console.ReadKey();
        }

        private void ShowNotes()
        {
            Console.Clear();
            var notes = _repo.GetNotes();
            if (notes.Count == 0)
            {
                Console.WriteLine("Ingen noter fundet.");
                Console.WriteLine("Tryk på en vilkårlig tast for at vende tilbage til menuen...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Dine noter:");
            foreach (var notePath in notes)
            {
                try
                {
                    string temp = System.IO.Path.GetTempFileName();
                    var request = new FileOperationRequest
                    {
                        InputPath = notePath,
                        OutputPath = System.IO.Path.GetDirectoryName(temp),
                        OutputFileName = System.IO.Path.GetFileName(temp)
                    };
                    _encryptionService.DecryptFile(request);
                    string content = System.IO.File.ReadAllText(temp);
                    System.IO.File.Delete(temp);

                    Console.WriteLine($"\n📄 {System.IO.Path.GetFileName(notePath)}:\n{content}");
                }
                catch
                {
                    Console.WriteLine($"⚠️ Kunne ikke læse note: {notePath}");
                }
            }
            Console.WriteLine("\nTryk på en vilkårlig tast for at vende tilbage til menuen...");
            Console.ReadKey();
        }

        private void ShowStats()
        {
            Console.Clear();
            var files = _repo.GetEncryptedFiles();
            var notes = _repo.GetNotes();
            Console.WriteLine("Statistik:");
            Console.WriteLine($"Antal krypterede filer: {files.Count}");
            Console.WriteLine($"Antal noter: {notes.Count}");
            Console.WriteLine("\nPlaceringer af krypterede filer:");
            foreach (var f in files) Console.WriteLine($"- {f}");
            Console.WriteLine("\nPlaceringer af noter:");
            foreach (var n in notes) Console.WriteLine($"- {n}");
            Console.WriteLine("\nTryk på en vilkårlig tast for at gå tilbage til menuen...");
            Console.ReadKey();
        }

        private void ShowSettings()
        {
            Console.Clear();
            Console.WriteLine("Indstillinger\n");
            Console.WriteLine("1. Skift hovedkodeord");
            Console.WriteLine("2. Tilbage til menuen");
            var key = Console.ReadKey(true);
            if (key.KeyChar == '1')
            {
                Console.Write("\nIndtast nyt hovedkodeord: ");
                var newPassword = Console.ReadLine();
                _repo.SavePasswordHash(newPassword);
                Console.WriteLine("✅ Hovedkodeordet er ændret.");
                Console.WriteLine("Tryk på en vilkårlig tast for at gå tilbage...");
                Console.ReadKey();
            }
        }
    }
}