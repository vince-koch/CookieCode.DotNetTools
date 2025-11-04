using Spectre.Console;
using Spectre.Console.Cli;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookieCode.DotNetTools.Commands.Mongo
{
    internal class MongoUiCommandSettings : CommandSettings
    {
        [CommandOption("--connection-name <CONNECTION_NAME>")]
        public string? ConnectionName { get; set; }

        [CommandOption("--connection-string <CONNECTION_STRING>")]
        public string? ConnectionString { get; set; }

        public static async Task EnsureConnectionStringAsync(MongoUiCommandSettings settings)
        {
            if (!string.IsNullOrWhiteSpace(settings.ConnectionString))
            {
                return;
            }

            var connections = await ConnectionFile.ReadConnectionFileAsync(
                dotFolder: ".cookiecode",
                filename: "connections.json",
                configSection: "mongo");

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
    }
}
