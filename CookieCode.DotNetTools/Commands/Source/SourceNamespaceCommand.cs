using Spectre.Console.Cli;

using System;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace CookieCode.DotNetTools.Commands.Source
{
    [Description("Update class namespaces to match folder and file names")]
    public class SourceNamespaceCommand : Command<SourceNamespaceCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [CommandOption("-f|--folder")]
            [Description("Root folder to begin processing at;  Defaults to current working directory")]
            public required string RootFolder { get; set; }

            [CommandOption("-n|--namespace")]
            public required string RootNamespace { get; set; }
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            settings.RootFolder = settings.RootFolder ?? Directory.GetCurrentDirectory();
            if (!Directory.Exists(settings.RootFolder))
            {
                throw new DirectoryNotFoundException($"Directory not found [{settings.RootFolder}]");
            }

            var files = FindFilesToProcess(settings);
            ProcessFiles(settings, files);
            return 0;
        }

        private string[] FindFilesToProcess(Settings settings)
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
                .SelectMany(pattern => Directory.GetFiles(settings.RootFolder, pattern, options))
                .Distinct()
                .ToArray();

            Console.WriteLine($"{files.Length} found");

            return files;
        }

        private void ProcessFiles(Settings settings, string[] files)
        {
            for (var i = 0; i < files.Length; i++)
            {
                var file = files[i];
                var directory = Path.GetDirectoryName(file).ThrowIfNull();
                var relativePath = Path.GetRelativePath(settings.RootFolder, directory);
                var relativeNamespace = relativePath.Replace('\\', '.').Replace('/', '.');

                var lines = File.ReadAllLines(file);
                var index = Array.FindIndex(lines, line => line.StartsWith("namespace "));
                if (index > -1)
                {
                    var suffix = lines[index].EndsWith(';') ? ";" : string.Empty;
                    lines[index] = $"namespace {settings.RootNamespace}.{relativeNamespace}".TrimEnd('.') + suffix;
                    File.WriteAllLines(file, lines);
                }

                Console.Write($"\r{i + 1} of {files.Length}");
            }

            Console.WriteLine();
        }
    }
}
