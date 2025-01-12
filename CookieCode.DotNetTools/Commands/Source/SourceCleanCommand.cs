using CookieCode.DotNetTools.Utilities;

using MAB.DotIgnore;

using Spectre.Console.Cli;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

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

            var gitIgnorePath = Path.Combine(searchDirectory, ".gitignore");

            var ignoreList = File.Exists(gitIgnorePath)
                ? new IgnoreList(gitIgnorePath)
                : GitIgnoreUtil.CreateDefaultIgnoreList();

            ignoreList.AddRules(settings.ExcludePatterns);

            // get the list of ignored files, but don't look at anything in git
            var paths = GitIgnoreUtil.GetIgnoredPaths(ignoreList, searchDirectory);
            paths = GitIgnoreUtil.RemoveGitFolder(paths);

            if (settings.IsDryRun)
            {
                foreach (var path in paths)
                {
                    Console.WriteLine(path);
                }
            }

            var confirmText = $"Delete {Ansi.FCyan}{paths.Count}{Ansi.Reset} paths? [y]es, [N]o ";
            var isConfirmed = settings.IsConfirmed || AnsiUtil.Confirm(confirmText, false);
            if (isConfirmed)
            {
                foreach (var path in paths)
                {
                    FileSystemUtil.Delete(path);
                }
            }

            return 0;
        }
    }
}
