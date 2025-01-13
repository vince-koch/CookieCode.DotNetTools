using CookieCode.DotNetTools.Utilities;

using Spectre.Console.Cli;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace CookieCode.DotNetTools.Commands.Source
{
    [Description("Zips a directory of source files, ignoring files specified by .gitignore")]
    public class SourceZipCommand : Command<SourceZipCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [CommandOption("-s|--source <SourcePath>")]
            [Description("Set the starting folder; if not provided the current directory will be selected")]
            public string? SourcePath { get; set; }

            [CommandOption("-z|--zip <ZipPath>")]
            [Description("Path of the zip file; if not provided a new zip will be placed in the source directory")]
            public string? ZipPath { get; set; }

            [CommandOption("-i|--ignore <IgnorePattern>")]
            [Description("Add one or more exclude pattern rules")]
            public IEnumerable<string>? IgnorePatterns { get; set; }
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            var searchDirectory = settings.SourcePath ?? Directory.GetCurrentDirectory();
            if (Directory.Exists(searchDirectory))
            {
                throw new DirectoryNotFoundException(searchDirectory);
            }

            // todo: a better job of zip file naming
            var zipFilePath = settings.ZipPath ?? Path.Combine(searchDirectory, Path.GetFileName(searchDirectory) + ".zip");

            var gitIgnore = new GitIgnore();
            gitIgnore.AddRule(".git/");
            gitIgnore.AddRules(settings.IgnorePatterns ?? Array.Empty<string>());

            var files = gitIgnore
                .Process(
                    searchDirectory,
                    isRecursive: true,
                    canReadGitIgnores: true,
                    record => !record.IsDirectory && !record.IsIgnored) // select only non ignored files
                .Select(record => record.Path)
                .ToList();

            var fileCount = ZipUtil.CreateZipFile(
                zipFilePath,
                files,
                (fileCount, filename) => AnsiUtil.WriteProgress($"{Ansi.Fg.Cyan}{fileCount}{Ansi.Reset}: {zipFilePath}"));

            AnsiUtil.WriteProgress($"{Ansi.Fg.Cyan}{fileCount}{Ansi.Reset} files => {Ansi.Fg.Magenta}{zipFilePath}{Ansi.Reset}");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success");
            Console.ResetColor();

            return 0;
        }
    }
}
