using System;
using System.IO;
using System.Threading;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CookieCode.DotNetTools.Commands.Config
{
    internal class ConfigEditCommand : ConfigBaseCommand
    {
        public override int Execute(CommandContext context, ConfigCommandSettings settings, CancellationToken cancellationToken)
        {
            EnsurePath(settings);

            if (!File.Exists(settings.Path))
            {
                ArgumentNullException.ThrowIfNull(settings.Path, nameof(settings.Path));

                AnsiConsole.MarkupLine($"[red]File does not exist[/] {settings.Path}");
                var create = AnsiConsole.Prompt(
                    new ConfirmationPrompt("[green]?[/] Would you like to create file?"));

                if (create)
                {
                    var directory = Path.GetDirectoryName(settings.Path);
                    ArgumentNullException.ThrowIfNull(directory, nameof(directory));

                    Directory.CreateDirectory(directory);
                    File.WriteAllLines(settings.Path, new[] { "" });
                }
            }

            if (File.Exists(settings.Path))
            {
                OpenInDefaultEditor(settings.Path);
            }

            return 0;
        }
    }
}
