using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

using CommandLine;
using MAB.DotIgnore;

using CookieCode.DotNetTools.Utilities;

namespace CookieCode.DotNetTools.Commands
{
    [Verb("zip-source", HelpText = "Zips a directory of source files, ignoring files specified by .gitignore")]
    public class ZipSourceCommand : ICommand
    {
        [Option('s', "source", Required = false, HelpText = "Set the starting folder")]
        public string SourcePath { get; set; }

        [Option('z', "zip", Required = false, HelpText = "Path of the zip file")]
        public string ZipPath { get; set; }

        [Option('r', "rules", Required = false, HelpText = "Add one or more exclude pattern rules")]
        public IEnumerable<string> Rules { get; set; }

        public void Execute()
        {
            var searchDirectory = SourcePath ?? Directory.GetCurrentDirectory();
            if (Directory.Exists(searchDirectory))
            {
                throw new DirectoryNotFoundException(searchDirectory);
            }

            var gitIgnorePath = Path.Combine(searchDirectory, ".gitignore");

            var ignoreList = File.Exists(gitIgnorePath)
                ? new IgnoreList(gitIgnorePath)
                : GitIgnoreUtil.CreateDefaultIgnoreList();

            ignoreList.AddRule(".git/");
            ignoreList.AddRules(Rules);

            var files = GitIgnoreUtil.GetFiles(ignoreList, searchDirectory);

            var zipFilePath = ZipPath ?? searchDirectory + ".zip";
            CreateZipFile(zipFilePath, searchDirectory, files);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success");
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
                    var relativePath = Path.GetRelativePath(searchDirectory, file);
                    AnsiUtil.WriteShortPathProgress(relativePath);

                    archive.CreateEntryFromFile(file, relativePath);
                    fileCount++;
                }
            }

            Console.WriteLine($"{fileCount} files zipped");
        }
    }
}
