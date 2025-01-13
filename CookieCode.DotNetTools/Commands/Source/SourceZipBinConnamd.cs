using CookieCode.DotNetTools.Utilities;

using Spectre.Console.Cli;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace CookieCode.DotNetTools.Commands.Source
{
    [Description("Zips a binary directory of files, ignoring config files")]
    public class SourceZipBinCommand : Command<SourceZipBinCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [CommandOption($"-s|--source <path>")]
            [Description("Set the starting folder; Defaut is current working directory")]
            public string? SourcePath { get; set; }

            [CommandOption($"-o|--output <path>")]
            [Description("Path to a directory to place zip files in")]
            public string? OutputFolder { get; set; }

            [CommandOption($"-i|--ignore <path>")]
            [Description("Add one or more exclude pattern rules")]
            public IEnumerable<string>? IgnoreRules { get; set; }
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            var searchDirectory = PathUtil.NormalizePath(settings.SourcePath ?? Directory.GetCurrentDirectory());
            if (!Directory.Exists(searchDirectory))
            {
                throw new DirectoryNotFoundException($"Directory not found: {searchDirectory}");
            }

            var binFolders = Directory.GetDirectories(searchDirectory, "bin", SearchOption.AllDirectories);
            foreach (var binFolder in binFolders)
            {
                // todo: a better job of zip file naming
                var zipFilePath = Path.Combine(
                    PathUtil.NormalizePath(settings.OutputFolder ?? Directory.GetCurrentDirectory()),
                    Path.GetFileName(Path.GetDirectoryName(binFolder)) + ".zip");

                var gitIgnore = new GitIgnore();
                gitIgnore.AddRule("[Nn]upkg/"); // ignore nupkg/ folders
                gitIgnore.AddRule("!**/[Bb]in/**/appsettings.json"); // ignore config files
                gitIgnore.AddRule("!**/[Bb]in/**/appsettings.*.json"); // ignore config files
                gitIgnore.AddRule("!**/[Bb]in/**/config.json"); // ignore config files
                gitIgnore.AddRule("!**/[Bb]in/**/config.*.json"); // ignore config files
                gitIgnore.AddRules(settings.IgnoreRules ?? Array.Empty<string>());

                var files = gitIgnore
                    .Process(
                        binFolder,
                        isRecursive: true,
                        canReadGitIgnores: false,
                        record => !record.IsDirectory && !record.IsIgnored)
                    .Select(record => record.Path)
                    .ToList();

                var fileCount = ZipUtil.CreateZipFile(
                    zipFilePath,
                    files,
                    (fileCount, filename) => AnsiUtil.WriteProgress($"{Ansi.Fg.Cyan}{fileCount}{Ansi.Reset}: {zipFilePath}"));

                AnsiUtil.WriteProgress($"{Ansi.Fg.Cyan}{fileCount}{Ansi.Reset} files => {Ansi.Fg.Magenta}{zipFilePath}{Ansi.Reset}");
                Console.WriteLine();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success");
            Console.ResetColor();

            return 0;
        }
    }
}
