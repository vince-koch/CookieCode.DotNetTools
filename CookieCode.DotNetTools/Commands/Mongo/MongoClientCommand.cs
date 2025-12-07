using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CookieCode.DotNetTools.Commands.Mongo
{
    [Description("Starts an instance of a mongo ui client")]
    internal class MongoClientCommand : AsyncCommand<MongoClientCommand.Settings>
    {
        internal class Settings : MongoUiCommandSettings
        {
            [CommandOption("--mongo-client <MONGO_CLIENT>")]
            [Description("mongo-compass / mongo-express / mongoku")]
            public string? MongoClient { get; set; }
        }

        private readonly IMongoUiCommand[] _mongoUiCommands;
        private readonly ConnectionStringService _connectionStringService;

        public MongoClientCommand(
            IEnumerable<IMongoUiCommand> mongoUiCommands,
            ConnectionStringService connectionStringService)
        {
            _mongoUiCommands = mongoUiCommands
                .OrderBy(c => c.Name)
                .ToArray();

            _connectionStringService = connectionStringService;
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
        {
            await _connectionStringService.EnsureConnectionStringAsync("mongo", settings);
            var mongoClientCommand = SelectMongoClient(settings);
            await mongoClientCommand.ExecuteAsync(context, settings, cancellationToken);            

            return 0;
        }

        private IMongoUiCommand SelectMongoClient(Settings settings)
        {
            if (!string.IsNullOrWhiteSpace(settings.MongoClient))
            {
                var mongoClientCommand = _mongoUiCommands
                    .Where(command => string.Equals(command.Name, settings.MongoClient, StringComparison.OrdinalIgnoreCase))
                    .SingleOrDefault();

                if (mongoClientCommand != null)
                {
                    return mongoClientCommand;
                }
            }

            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<IMongoUiCommand>()
                    .Title("[green]?[/] Select mongo client:")
                    .MoreChoicesText("[grey](Use ↑/↓ to navigate, Enter to select)[/]")
                    .PageSize(10)
                    .AddChoices(_mongoUiCommands)
                    .UseConverter(command => command.Name)
            );

            settings.MongoClient = selected.Name;

            AnsiConsole.MarkupLine($"[green]?[/] Select mongo client: [cyan]{settings.MongoClient}[/]");

            return selected;
        }
    }
}
