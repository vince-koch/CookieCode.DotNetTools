using System;
using System.IO;
using System.Linq;

using CommandLine;

namespace DotNetFix
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                var result = Parser.Default.ParseArguments<ProgramArgs>(args);
                var arguments = (result as Parsed<ProgramArgs>)?.Value;
                if (arguments == null)
                {
                    // we can just exit - the parser has already written
                    // error messages and help info to the console
                    return 1;
                }

                var files = FindFilesToProcess(arguments);
                ProcessFiles(arguments, files);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine();
                Console.WriteLine("Success");
                return 0;
            }
            catch (Exception thrown)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine(thrown);
                return 1;
            }
            finally
            {
                Console.ResetColor();
            }
        }

        private static string[] FindFilesToProcess(ProgramArgs arguments)
        {
            Console.Write("Searching for files... ");

            string[] patterns = { "*.cs" };
            var options = new EnumerationOptions
            {
                MatchCasing = MatchCasing.CaseInsensitive,
                MatchType = MatchType.Win32,
                RecurseSubdirectories = true,
            };

            var files = patterns
                .SelectMany(pattern => Directory.GetFiles(arguments.RootFolder, pattern, options))
                .Distinct()
                .ToArray();

            Console.WriteLine($"{files.Length} found");

            return files;
        }

        private static void ProcessFiles(ProgramArgs arguments, string[] files)
        {
            for (var i = 0; i < files.Length; i++)
            {
                var file = files[i];
                var directory = Path.GetDirectoryName(file);
                var relativePath = Path.GetRelativePath(arguments.RootFolder, directory);
                var relativeNamespace = relativePath.Replace('\\', '.').Replace('/', '.');

                var lines = File.ReadAllLines(file);
                var index = Array.FindIndex(lines, line => line.StartsWith("namespace "));
                if (index > -1)
                {
                    lines[index] = $"namespace {arguments.RootNamespace}.{relativeNamespace}".TrimEnd('.');
                    File.WriteAllLines(file, lines);
                }

                Console.Write($"\r{i + 1} of {files.Length}");
            }

            Console.WriteLine();
        }
    }
}
