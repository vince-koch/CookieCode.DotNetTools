using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;

namespace CookieCode.DotNetTools.Commands.Editor.Services
{
    public sealed class FileService(ILogger<FileService> logger) : IFileService
    {
        private static readonly UTF8Encoding Utf8NoBom = new(false);

        public string? ReadAllText(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            logger.LogInformation("Reading {Path}", path);
            return File.ReadAllText(path);
        }

        public void WriteAllText(string path, string text)
        {
            logger.LogInformation("Writing {Path}", path);
            File.WriteAllText(path, text, Utf8NoBom);
        }
    }
}
