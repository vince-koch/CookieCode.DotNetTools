using System;
using System.Collections.Generic;
using System.IO;

using CommandLine;
using MAB.DotIgnore;

using CookieCode.DotNetTools.Utilities;

namespace CookieCode.DotNetTools.Commands
{
    [Verb("cleanup", HelpText = "Zips a directory of source files, ignoring files specified by .gitignore")]
    public class CleanupCommand : ICommand
    {
        [Option('s', "source", Required = false, HelpText = "Set the starting folder")]
        public string SourcePath { get; set; }

        [Option('r', "rules", Required = false, HelpText = "Add one or more exclude pattern rules")]
        public IEnumerable<string> Rules { get; set; }

        [Option("is-dry-run", Required = false, HelpText = "List paths that will be removed")]
        public bool IsDryRun { get; set; }

        [Option("is-confirmed", Required = false, HelpText = "Assumes confirmation and does not confirm with user")]
        public bool IsConfirmed { get; set; }

        public void Execute()
        {
            var searchDirectory = SourcePath ?? Directory.GetCurrentDirectory();
            if (!Directory.Exists(searchDirectory))
            {
                throw new DirectoryNotFoundException(searchDirectory);
            }

            var gitIgnorePath = Path.Combine(searchDirectory, ".gitignore");

            var ignoreList = File.Exists(gitIgnorePath)
                ? new IgnoreList(gitIgnorePath)
                : GitIgnoreUtil.CreateDefaultIgnoreList();

            ignoreList.AddRules(Rules);

            // get the list of ignored files, but don't look at anything in git
            var paths = GitIgnoreUtil.GetIgnoredPaths(ignoreList, searchDirectory);
            paths = GitIgnoreUtil.RemoveGitFolder(paths);

            if (IsDryRun)
            {
                foreach (var path in paths)
                {
                    Console.WriteLine(path);
                }
            }

            var confirmText = $"Delete {Ansi.FCyan}{paths.Count}{Ansi.Reset} paths? [y/N] ";
            var isConfirmed = IsConfirmed || AnsiUtil.Confirm(confirmText, false);
            if (isConfirmed)
            {
                foreach (var path in paths)
                {
                    FileSystemUtil.Delete(path);
                }
            }
        }
    }
}
