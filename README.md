# TheVault

TheVault is a simple, console-based vault application for securely storing encrypted files and notes. It uses password-based encryption and hashing to protect your data.

## Features

- Password-protected access
- File encryption and decryption (AES-256)
- Secure note storage (encrypted notes)
- Simple console menu interface
- All data stored locally

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Build and Run

1. Clone the repository.
2. Open a terminal in the project directory.
3. Build the project:
dotnet build
4. Run the application:
dotnet run --project TheVault.App

### Usage

1. On first run, set a master password.
2. Use the menu to:
- Add and encrypt files
- List encrypted files
- Decrypt files
- Add encrypted notes
- View decrypted notes

All encrypted files are stored in the `vault_files` directory, and notes in `vault_notes`. Configuration (including the password hash and note paths) is stored in `vault.json`.

## Project Structure

- `TheVault.App/Program.cs` - Main application logic and menu
- `TheVault.App/Services/EncryptionService.cs` - AES encryption/decryption
- `TheVault.App/Services/HashService.cs` - Password hashing (SHA-256)
- `TheVault.App/Repositories/VaultRepository.cs` - Vault configuration and note management

## Security

- Passwords are hashed using SHA-256.
- Files and notes are encrypted using AES with a key derived from your password.
- The master password is never stored in plain text.

## Testing

Unit tests are located in the `TheVault.Tests` project. Run tests with:
dotnet test

## License

MIT License