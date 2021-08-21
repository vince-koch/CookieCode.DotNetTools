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

                var count = PruneDirectories(directory);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{count} folders were pruned");

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

        private static int PruneDirectories(string directory)
        {
            var count = 0;

            foreach (var child in Directory.GetDirectories(directory))
            {
                count += PruneDirectories(child);

                if (!Directory.EnumerateFileSystemEntries(child).Any())
                {
                    Console.WriteLine(child);
                    Directory.Delete(child);
                    count++;
                }
            }

            return count;
        }
    }
}
