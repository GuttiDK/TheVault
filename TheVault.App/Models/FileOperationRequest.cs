namespace TheVault.App.Models
{
    public class FileOperationRequest
    {
        public string InputPath { get; set; } = string.Empty;
        public string? OutputPath { get; set; }
        public string? OutputFileName { get; set; }
    }
}