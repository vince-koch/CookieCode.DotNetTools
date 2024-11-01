using CookieCode.DotNetTools.Utilities;
using Spectre.Console;
using Spectre.Console.Cli;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace CookieCode.DotNetTools.Commands.Zip
{
    [Description("Zips a set of source files and folders")]
    public class ZipCommand : Command<ZipCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [CommandOption("-s|--source <SourcePath>")]
			[Description("One or more source paths (at least one required)")]
			public required IEnumerable<string> SourcePaths { get; set; } = Array.Empty<string>();

			[CommandOption("-z|--zip <ZipPath>")]
            [Description("Zip archive path")]
            public required string ZipPath { get; set; }

			public override ValidationResult Validate()
			{
				return SourcePaths.Any()
					? ValidationResult.Success()
					: ValidationResult.Error("At least one source path is required.");
			}
		}

        public override int Execute(CommandContext context, Settings settings)
        {
            long fileCount = 0;

            using (var stream = new FileStream(settings.ZipPath, FileMode.Create))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, true))
            {
                foreach (var sourcePath in settings.SourcePaths)
                {
                    if (Directory.Exists(sourcePath))
                    {
                        var files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
                        foreach (var file in files)
                        {
                            fileCount++;
                            var relativePath = Path.GetRelativePath(sourcePath, file);
                            var shortPath = PathUtil.GetShortPath(relativePath);
                            AnsiUtil.WriteProgress($"{fileCount}: {shortPath}");
                            archive.CreateEntryFromFile(file, relativePath);

                        }
                    }
                    else if (File.Exists(sourcePath))
                    {
                        fileCount++;
                        var filename = Path.GetFileName(sourcePath);
                        AnsiUtil.WriteProgress($"{fileCount}: {filename}");
                        archive.CreateEntryFromFile(sourcePath, filename);
                    }
                }
            }

            AnsiUtil.WriteProgress($"{fileCount} files => {settings.ZipPath}");
            Console.WriteLine();

            return 0;
        }
    }
}
