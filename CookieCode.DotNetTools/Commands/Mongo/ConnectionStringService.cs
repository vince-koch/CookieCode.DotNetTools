using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Spectre.Console;

namespace CookieCode.DotNetTools.Commands.Mongo
{
    internal class ConnectionStringFileSettings
    {
        public required string Path { get; set; }

        public string? Section { get; set; }
    }

    public class ConnectionStrings : ConcurrentDictionary<string, string>
    {
    }

    public class DriverConnectionStrings : ConcurrentDictionary<string, ConnectionStrings>
    {
    }

    internal class ConnectionStringService
    {
        private readonly IConfiguration _configuration;
        private readonly Lazy<Task<DriverConnectionStrings>> _driverConnectionStringsTask;

        public ConnectionStringService(IConfiguration configuration)
        {
            _configuration = configuration;
            _driverConnectionStringsTask = new Lazy<Task<DriverConnectionStrings>>(() => LoadConnectionFilesAsync(configuration));
        }

        public async Task<ConnectionStrings?> GetConnectionStringsAsync(string driver)
        {
            var driverConnectionStrings = await _driverConnectionStringsTask.Value;

            var connectionStrings = driverConnectionStrings.TryGetValue(driver, out ConnectionStrings? value)
                ? value
                : null;

            return connectionStrings;
        }

        public async Task<string?> GetConnectionStringAsync(string driver, string name)
        {
            var connectionStrings = await GetConnectionStringsAsync(driver);

            var connectionString = connectionStrings?.TryGetValue(name, out string? value) == true
                ? value
                : null;

            return connectionString;
        }

        public async Task EnsureConnectionStringAsync(string driver, MongoUiCommandSettings settings)
        {
            if (!string.IsNullOrWhiteSpace(settings.ConnectionString))
            {
                return;
            }

            var connections = await GetConnectionStringsAsync(driver);
            ArgumentNullException.ThrowIfNull(connections, nameof(connections));

            if (!string.IsNullOrWhiteSpace(settings.ConnectionName))
            {
                settings.ConnectionString = connections[settings.ConnectionName];
            }
            else
            {
                var selected = AnsiConsole.Prompt(
                    new SelectionPrompt<KeyValuePair<string, string>>()
                        .Title("[green]?[/] Select connection:")
                        .MoreChoicesText("[grey](Use ↑/↓ to navigate, Enter to select)[/]")
                        .PageSize(10)
                        .UseConverter(kv => kv.Key) // show the key in the menu
                        .AddChoices(connections.OrderBy(c => c.Key))
                );

                settings.ConnectionName = selected.Key;
                settings.ConnectionString = selected.Value;

                AnsiConsole.MarkupLine($"[green]?[/] Select connection: [cyan]{settings.ConnectionName}[/]");
            }
        }

        private static async Task<DriverConnectionStrings> LoadConnectionFilesAsync(IConfiguration configuration)
        {
            var section = configuration.GetRequiredSection("connectionStringFiles");
            var fileSettings = new Dictionary<string, ConnectionStringFileSettings[]>();
            section.Bind(fileSettings);

            var driverConnectionStrings = new DriverConnectionStrings();
            foreach (var pair in fileSettings)
            {
                ConnectionStrings target = driverConnectionStrings.GetOrAdd(pair.Key, key => new ConnectionStrings());

                var fileSettingsArray = pair.Value;
                foreach (ConnectionStringFileSettings settings in fileSettingsArray)
                {
                    // load connection string file, and merge
                    var source = await ReadConnectionStringsFileAsync(settings);                    
                    foreach (var item in source)
                    {
                        target.AddOrUpdate(item.Key, item.Value, (key, oldValue) => item.Value);
                    }
                }
            }

            return driverConnectionStrings;
        }

        private static async Task<ConnectionStrings> ReadConnectionStringsFileAsync(ConnectionStringFileSettings settings)
        {
            try
            {
                var path = Path.GetFullPath(Environment.ExpandEnvironmentVariables(settings.Path));

                using var stream = File.OpenRead(path);
                using var reader = new StreamReader(stream);
                string text = await reader.ReadToEndAsync();

                var documentOptions = new JsonDocumentOptions
                {
                    AllowTrailingCommas = true,
                    CommentHandling = JsonCommentHandling.Skip,
                };

                using JsonDocument document = JsonDocument.Parse(text, documentOptions);
                JsonElement root = document.RootElement;
                JsonElement section = !string.IsNullOrWhiteSpace(settings.Section)
                    ? root.GetProperty(settings.Section)
                    : root;

                var serializerOptions = new JsonSerializerOptions
                {
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                };

                var connections = section.Deserialize<ConnectionStrings>(serializerOptions) ?? throw new Exception("Unable to parse file or section");

                return connections;
            }
            catch (Exception thrown)
            {
                throw new Exception($"Error reading {settings.Path}", thrown);
            }
        }
    }
}
