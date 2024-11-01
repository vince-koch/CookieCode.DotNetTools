using CookieCode.DotNetTools.Utilities;

using MAB.DotIgnore;

using Spectre.Console.Cli;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace CookieCode.DotNetTools.Commands.Source
{
    [Description("Zips a directory of source files, ignoring files specified by .gitignore")]
    public class SourceZipCommand : Command<SourceZipCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [CommandOption("-s|--source <SourcePath>")]
            [Description("Set the starting folder")]
            public required string? SourcePath { get; set; }

            [CommandOption("-z|--zip <ZipPath>")]
            [Description("Path of the zip file")]
            public required string ZipPath { get; set; }

            [CommandOption("-r|--rule <Rule>")]
            [Description("Add one or more exclude pattern rules")]
            public required IEnumerable<string> Rules { get; set; }
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            var searchDirectory = settings.SourcePath ?? Directory.GetCurrentDirectory();
            if (Directory.Exists(searchDirectory))
            {
                throw new DirectoryNotFoundException(searchDirectory);
            }

            var gitIgnorePath = Path.Combine(searchDirectory, ".gitignore");

            var ignoreList = File.Exists(gitIgnorePath)
                ? new IgnoreList(gitIgnorePath)
                : GitIgnoreUtil.CreateDefaultIgnoreList();

            ignoreList.AddRule(".git/");
            ignoreList.AddRules(settings.Rules);

            var files = GitIgnoreUtil.GetFiles(ignoreList, searchDirectory);

            var zipFilePath = settings.ZipPath ?? searchDirectory + ".zip";
            CreateZipFile(zipFilePath, searchDirectory, files);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success");

            return 0;
        }

        private void CreateZipFile(string zipPath, string searchDirectory, List<string> files)
        {
            if (!files.Any())
            {
                return;
            }

            long fileCount = 0;

            using (var stream = new FileStream(zipPath, FileMode.Create))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, true))
            {
                foreach (var file in files)
                {
                    fileCount++;
                    var relativePath = Path.GetRelativePath(searchDirectory, file);
                    var shortPath = PathUtil.GetShortPath(relativePath);
                    AnsiUtil.WriteProgress($"{fileCount}: {shortPath}");

                    archive.CreateEntryFromFile(file, relativePath);
                }
            }

            AnsiUtil.WriteProgress($"{fileCount} files => {zipPath}");
            Console.WriteLine();
        }
    }
}
