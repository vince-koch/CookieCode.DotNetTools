using System;
using System.IO;
using System.Linq;

using CommandLine;

namespace CookieCode.DotNetTools.Commands
{
    [Verb("fix-namespaces", HelpText = "Update class namespaces to match folder and file names")]
    public class FixNamespacesCommand : ICommand
    {
        [Option('f', "folder", Required = true, HelpText = "Root folder to begin processing at")]
        public string RootFolder { get; set; }

        [Option('n', "namespace", Required = true, HelpText = "Root namespace to apply to the folder")]
        public string RootNamespace { get; set; }

        public void Execute()
        {
            var files = FindFilesToProcess();
            ProcessFiles(files);
        }

        private string[] FindFilesToProcess()
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
                .SelectMany(pattern => Directory.GetFiles(RootFolder, pattern, options))
                .Distinct()
                .ToArray();

            Console.WriteLine($"{files.Length} found");

            return files;
        }

        private void ProcessFiles(string[] files)
        {
            for (var i = 0; i < files.Length; i++)
            {
                var file = files[i];
                var directory = Path.GetDirectoryName(file);
                var relativePath = Path.GetRelativePath(RootFolder, directory);
                var relativeNamespace = relativePath.Replace('\\', '.').Replace('/', '.');

                var lines = File.ReadAllLines(file);
                var index = Array.FindIndex(lines, line => line.StartsWith("namespace "));
                if (index > -1)
                {
                    lines[index] = $"namespace {RootNamespace}.{relativeNamespace}".TrimEnd('.');
                    File.WriteAllLines(file, lines);
                }

                Console.Write($"\r{i + 1} of {files.Length}");
            }

            Console.WriteLine();
        }
    }
}
