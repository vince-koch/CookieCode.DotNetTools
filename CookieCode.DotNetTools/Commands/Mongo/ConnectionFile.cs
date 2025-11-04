using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CookieCode.DotNetTools.Commands.Mongo
{
    internal class ConnectionFile
    {
        public static async Task<Dictionary<string, string>> ReadConnectionFileAsync(string? dotFolder, string filename, string? configSection)
        {
            string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            string[] parts = new string?[] { home, dotFolder, filename }
                .Where(part => !string.IsNullOrWhiteSpace(part))
                .OfType<string>()
                .ToArray();

            string path = Path.GetFullPath(Path.Combine(parts));

            using var stream = File.OpenRead(path);
            using var reader = new StreamReader(stream);
            string text = await reader.ReadToEndAsync();

            using JsonDocument document = JsonDocument.Parse(text);
            JsonElement root = document.RootElement;
            JsonElement section = configSection != null
                ? root.GetProperty(configSection)
                : root;

            var connections = section.Deserialize<Dictionary<string, string>>() ?? throw new Exception("Unable to parse file or section");

            return connections;
        }
    }
}
