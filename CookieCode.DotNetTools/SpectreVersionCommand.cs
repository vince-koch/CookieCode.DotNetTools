using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CookieCode.DotNetTools
{
    [Description("Writes version information")]
    internal class SpectreVersionCommand : Command<SpectreVersionCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [CommandOption("-a|--all")]
            [Description("Include versions for all assemblies (optional).")]
            public bool All { get; set; }
        }

        public override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            ArgumentNullException.ThrowIfNull(entryAssembly, nameof(entryAssembly));
            
            AnsiConsole.MarkupLine($"[green]{entryAssembly.GetName().Name}[/] [yellow]{GetAssemblyVersion(entryAssembly)}[/]");

            if (settings.All)
            {
                AnsiConsole.WriteLine();

                var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(assembly => assembly != entryAssembly)
                    .OrderBy(assembly => assembly.GetName().Name)
                    .ToList();

                foreach (var assembly in assemblies)
                {
                    AnsiConsole.MarkupLine($"{assembly.GetName().Name} [yellow]{GetAssemblyVersion(assembly)}[/]");
                }
            }

            return 0;
        }

        private string GetAssemblyVersion(Assembly assembly)
        {
            var version = assembly.GetCustomAttribute<AssemblyVersionAttribute>()?.Version
                ?? assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                ?? "unknown";
            
            return version;
        }
    }
}
