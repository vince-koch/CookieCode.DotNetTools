using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

using CommandLine;

using CookieCode.DotNetTools.Utilities;

namespace CookieCode.DotNetTools.Commands
{
    [Verb("zip", HelpText = "Zips a set of source files and folders")]
    public class ZipCommand : ICommand
    {
        [Option('s', "source", Required = true, HelpText = "Source paths")]
        public IEnumerable<string> SourcePaths { get; set; }

        [Option('z', "zip", Required = true, HelpText = "Zip archive path")]
        public string ZipPath { get; set; }

        public void Execute()
        {
            long fileCount = 0;

            using (var stream = new FileStream(ZipPath, FileMode.Create))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, true))
            {
                foreach (var sourcePath in SourcePaths)
                {
                    if (Directory.Exists(sourcePath))
                    {
                        var files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
                        foreach (var file in files)
                        {
                            var filename = Path.GetRelativePath(sourcePath, file);
                            AnsiUtil.WriteShortPathProgress(filename);
                            archive.CreateEntryFromFile(file, filename);
                            fileCount++;
                        }
                    }
                    else if (File.Exists(sourcePath))
                    {
                        var filename = Path.GetFileName(sourcePath);
                        AnsiUtil.WriteShortPathProgress(filename);
                        archive.CreateEntryFromFile(sourcePath, filename);
                        fileCount++;
                    }
                }
            }

            Console.WriteLine($"{fileCount} files zipped");
        }
    }
}
