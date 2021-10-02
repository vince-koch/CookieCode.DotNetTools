using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }
    }
}
