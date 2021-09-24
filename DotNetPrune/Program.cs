using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DotNetPrune
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                var directory = args.Length == 0
                    ? Directory.GetCurrentDirectory()
                    : args[0];

                if (!Directory.Exists(directory))
                {
                    throw new DirectoryNotFoundException(directory);
                }

                var cleaned = CleanDirectory(directory, "bin", "obj", "node_modules");
                var pruned = PruneDirectories(directory);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{cleaned} folders cleaned, {pruned} folders pruned");

                return 0;
            }
            catch (Exception thrown)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(thrown.Message);
                return 1;
            }
            finally
            {
                Console.ResetColor();
            }
        }

        private static int CleanDirectory(string directory, params string[] patterns)
        {
            var count = 0;

            var matches = patterns
                .SelectMany(pattern => Directory.GetDirectories(directory, pattern, SearchOption.AllDirectories))
                .ToArray();

            foreach (var match in matches)
            {
                if (Directory.Exists(match))
                {
                    Console.WriteLine($"CLEAN: {match}");
                    Directory.Delete(match, true);
                    count++;
                }
            }

            return count;
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
