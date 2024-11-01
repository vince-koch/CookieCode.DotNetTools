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
    [Description("Zips a binary directory of files, ignoring config files")]
    public class SourceZipBinCommand : Command<SourceZipBinCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [CommandOption($"-s|--source <{nameof(SourcePath)}>")]
            [Description("Set the starting folder; Defaut is current working directory")]
            public string? SourcePath { get; set; }

            [CommandOption("-z|--zip")]
            [Description("Path of the zip file")]
            public string ZipPath { get; set; }

            [CommandOption("-r|--rule")]
            [Description("Add one or more exclude pattern rules")]
            public IEnumerable<string> Rules { get; set; }
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            var searchDirectory = PathUtil.NormalizePath(settings.SourcePath ?? Directory.GetCurrentDirectory());
            if (!Directory.Exists(searchDirectory))
            {
                throw new DirectoryNotFoundException($"Directory not found: {searchDirectory}");
            }

            var ignoreList = new IgnoreList();
            ignoreList.AddRule("appsettings.json");
            ignoreList.AddRule("appsettings.*.json");
            ignoreList.AddRule("config.json");
            ignoreList.AddRule("config.*.json");
            ignoreList.AddRules(settings.Rules ?? Array.Empty<string>());

            var files = GitIgnoreUtil.GetFiles(ignoreList, searchDirectory);

            // todo: a better job of zip file naming
            var searchDirectoryName = Path.GetFileName(searchDirectory);
            var zipFilePath = settings.ZipPath ?? Path.Combine(searchDirectory, searchDirectoryName + ".zip");
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
                    var progress = $"{fileCount}: {shortPath}";
                    AnsiUtil.WriteProgress(progress);

                    archive.CreateEntryFromFile(file, relativePath);
                }
            }

            AnsiUtil.WriteProgress($"{fileCount} files => {zipPath}");
            Console.WriteLine();
        }
    }
}
