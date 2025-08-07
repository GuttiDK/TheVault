using TheVault.App.Models;
using TheVault.App.Services;
using TheVault.App.Repositories;

namespace TheVault.App.Areas
{
    public class MainMenu
    {
        private readonly EncryptionService _encryptionService;
        private readonly VaultRepository _repo;

        // Menu items for arrow navigation
        private readonly string[] _menuItems = new[]
        {
            "Tilføj fil",
            "Vis krypterede filer",
            "Dekrypter fil",
            "Tilføj note",
            "Vis noter",
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
                        case 5: ShowSettings(); break;
                        case 6: Console.WriteLine("Farvel 👋"); return;
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
            Console.WriteLine("✅ Fil krypteret.");
            Console.WriteLine("Tryk på en vilkårlig tast for at vende tilbage til menuen...");
            Console.ReadKey();
        }

        private void ShowEncryptedFiles()
        {
            Console.Clear();
            Console.Write("Indtast mappe hvor dine krypterede filer ligger: ");
            var folder = Console.ReadLine();
            if (!Directory.Exists(folder))
            {
                Console.WriteLine("Mappen findes ikke.");
                Console.WriteLine("Tryk på en vilkårlig tast for at vende tilbage til menuen...");
                Console.ReadKey();
                return;
            }
            var files = Directory.GetFiles(folder);
            if (files.Length == 0)
            {
                Console.WriteLine("Ingen krypterede filer fundet.");
                Console.WriteLine("Tryk på en vilkårlig tast for at vende tilbage til menuen...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Krypterede filer:");
            foreach (var file in files)
                Console.WriteLine($"- {Path.GetFileName(file)}");

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

            string tempNotePath = Path.GetTempFileName();
            File.WriteAllText(tempNotePath, noteText);

            var noteRequest = new FileOperationRequest
            {
                InputPath = tempNotePath,
                OutputPath = string.IsNullOrWhiteSpace(noteDir) ? null : noteDir,
                OutputFileName = string.IsNullOrWhiteSpace(noteFileName) ? null : noteFileName
            };

            _encryptionService.EncryptFile(noteRequest);
            File.Delete(tempNotePath);

            // Save the note path for later retrieval
            string savedNotePath = noteRequest.OutputPath ?? Path.GetDirectoryName(tempNotePath) ?? Directory.GetCurrentDirectory();
            string savedNoteFile = noteRequest.OutputFileName ?? Path.GetFileName(tempNotePath) + ".enc";
            _repo.AddNote(Path.Combine(savedNotePath, savedNoteFile));

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
                    string temp = Path.GetTempFileName();
                    var request = new FileOperationRequest
                    {
                        InputPath = notePath,
                        OutputPath = Path.GetDirectoryName(temp),
                        OutputFileName = Path.GetFileName(temp)
                    };
                    _encryptionService.DecryptFile(request);
                    string content = File.ReadAllText(temp);
                    File.Delete(temp);

                    Console.WriteLine($"\n📄 {Path.GetFileName(notePath)}:\n{content}");
                }
                catch
                {
                    Console.WriteLine($"⚠️ Kunne ikke læse note: {notePath}");
                }
            }
            Console.WriteLine("\nTryk på en vilkårlig tast for at vende tilbage til menuen...");
            Console.ReadKey();
        }

        private void ShowSettings()
        {
            Console.Clear();
            Console.WriteLine("Indstillinger\n");
            Console.WriteLine("Her kan du tilføje flere indstillinger senere.");
            Console.WriteLine("\nTryk på en vilkårlig tast for at gå tilbage til menuen...");
            Console.ReadKey();
        }
    }
}