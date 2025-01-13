using CookieCode.DotNetTools.Utilities;

using Spectre.Console.Cli;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace CookieCode.DotNetTools.Commands.Source
{
    [Description("Cleans a directory of source files, removing files ignored by .gitignore")]
    public class SourceCleanCommand : Command<SourceCleanCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [CommandOption("-s|--source <starting-folder>")]
            [Description("Set the starting folder")]
            public required string SourcePath { get; set; }

            [CommandOption("-e|--exclude")]
            [Description("Add one or more exclude pattern rules")]
            public IEnumerable<string>? ExcludePatterns { get; set; }

            [CommandOption("-d|--dry")]
            [Description("List paths that will be removed")]
            public bool IsDryRun { get; set; }

            [CommandOption("-y|--yes")]
            [Description("Assumes confirmation and does not confirm with user")]
            public bool IsConfirmed { get; set; }
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            var searchDirectory = settings.SourcePath ?? Directory.GetCurrentDirectory();
            if (!Directory.Exists(searchDirectory))
            {
                throw new DirectoryNotFoundException(searchDirectory);
            }

            var gitIgnore = new GitIgnore();
            gitIgnore.AddRule(".git/"); // ignore the .git folder
            gitIgnore.AddRules(settings.ExcludePatterns ?? Array.Empty<string>());

            List<string> deletePaths = gitIgnore
                .Process(
                    searchDirectory,
                    isRecursive: true,
                    canReadGitIgnores: true,
                    info => info.IsIgnored)
                .Select(info => info.Path)
                .ToList();

            deletePaths.ForEach(path => Console.WriteLine(path));

            var confirmText = $"Delete {Ansi.Fg.Cyan}{deletePaths.Count}{Ansi.Reset} paths? [y]es, [N]o ";
            var isConfirmed = settings.IsConfirmed || AnsiUtil.Confirm(confirmText, false);
            if (isConfirmed)
            {
                foreach (var path in deletePaths)
                {
                    FileSystemUtil.Delete(path);
                }
            }

            return 0;
        }
    }
}
