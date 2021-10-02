using System;
using System.IO;
using System.Linq;

using CommandLine;

namespace CookieCode.DotNetTools.Commands
{
    [Verb("prune", HelpText = "Remove empty folders")]
    public class PruneCommand : ICommand
    {
        [Value(0, HelpText = "Starting directory")]
        public string DirectoryPath { get; set; }

        public void Execute()
        {
            var directory = DirectoryPath ?? Directory.GetCurrentDirectory();

            if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException(directory);
            }

            var pruned = PruneDirectories(directory);
            Console.WriteLine($"{pruned} folders pruned");
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
