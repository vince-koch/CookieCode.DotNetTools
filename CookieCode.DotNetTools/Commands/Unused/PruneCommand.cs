using Spectre.Console.Cli;

using System;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace CookieCode.DotNetTools.Commands.Unused
{
    //[Verb("prune", HelpText = "Remove empty folders")]
    [Description("Remove empty folders")]
    public class PruneCommand : Command<PruneCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            //[Value(0, HelpText = "Starting directory")]
            [CommandArgument(0, "[start-directory]")]
            [Description("The directory to start pruning from, or the current directory if not specified")]
            public string? DirectoryPath { get; set; }
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            var directory = settings.DirectoryPath ?? Directory.GetCurrentDirectory();

            if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException(directory);
            }

            var pruned = PruneDirectories(directory);
            Console.WriteLine($"{pruned} folders pruned");

            return 0;
        }

        private static int PruneDirectories(string directory)
        {
            var count = 0;

            foreach (var child in Directory.GetDirectories(directory))
            {
                count += PruneDirectories(child);

                if (!Directory.EnumerateFileSystemEntries(child).Any())
                {
                    Console.WriteLine($"PRUNE: {child}");
                    Directory.Delete(child);
                    count++;
                }
            }

            return count;
        }
    }
}
