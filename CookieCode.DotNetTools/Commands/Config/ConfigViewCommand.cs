using System.IO;
using System.Threading;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CookieCode.DotNetTools.Commands.Config
{
    internal partial class ConfigViewCommand : ConfigBaseCommand
    {
        public override int Execute(CommandContext context, ConfigCommandSettings settings, CancellationToken cancellationToken)
        {
            EnsurePath(settings);

            if (!File.Exists(settings.Path))
            {
                AnsiConsole.MarkupLine($"[red]File does not exist[/] {settings.Path}");
                return 1;
            }

            var lines = File.ReadAllLines(settings.Path);

            AnsiConsole.MarkupLine($"[cyan]{settings.Path}[/]");
            foreach (var line in lines)
            {
                AnsiConsole.WriteLine(line);
            }
            
            return 0;
        }
    }
}
