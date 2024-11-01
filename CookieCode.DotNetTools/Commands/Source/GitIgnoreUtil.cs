using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MAB.DotIgnore;

namespace CookieCode.DotNetTools.Commands.Source
{
    public static class GitIgnoreUtil
    {
        public static IgnoreList CreateDefaultIgnoreList()
        {
            var rules = new string[]
            {
                "[Bb]in/",
                "[Oo]bj/",
                "packages/",
                "node_modules/",
            };

            var ignoreList = new IgnoreList(rules);

            return ignoreList;
        }

        /// <summary>
        /// Gets non-ignored files
        /// </summary>
        public static List<string> GetFiles(IgnoreList ignoreList, string directory)
        {
            var notIgnored = new List<string>();
            ignoreList.Process(directory, notIgnored, null);
            return notIgnored;
        }

        public static List<string> GetIgnoredPaths(IgnoreList ignoreList, string directory)
        {
            var ignored = new List<string>();
            ignoreList.Process(directory, null, ignored);
            return ignored;
        }

        public static void Process(
            this IgnoreList ignoreList,
            string currentDirectory,
            List<string>? notIgnored,
            List<string>? ignored)
        {
            // append .gitignore files in child folders
            var gitIgnorePath = Path.Combine(currentDirectory, ".gitignore");
            if (File.Exists(gitIgnorePath))
            {
                var gitIgnoreLines = File.ReadAllLines(gitIgnorePath);

                ignoreList = ignoreList.Clone();
                ignoreList.AddRules(gitIgnoreLines);
            }

            // process folders
            var directories = Directory.GetDirectories(currentDirectory);
            foreach (var directory in directories)
            {
                if (ignoreList.IsIgnored(directory, pathIsDirectory: true))
                {
                    ignored?.Add(directory);
                }
                else
                {
                    ignoreList.Process(directory, notIgnored, ignored);
                }
            }

            // process files
            var files = Directory.GetFiles(currentDirectory);
            foreach (var file in files)
            {
                if (ignoreList.IsIgnored(file, pathIsDirectory: false))
                {
                    ignored?.Add(file);
                }
                else
                {
                    notIgnored?.Add(file);
                }
            }
        }

        public static List<string> RemoveGitFolder(List<string> source)
        {
            var target = source
                .Where(p => !p.Contains($"{Path.DirectorySeparatorChar}.git{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
                .Where(p => !p.Contains($"{Path.AltDirectorySeparatorChar}.git{Path.AltDirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
                .Where(p => !p.EndsWith($"{Path.DirectorySeparatorChar}.git", StringComparison.OrdinalIgnoreCase))
                .Where(p => !p.EndsWith($"{Path.AltDirectorySeparatorChar}.git", StringComparison.OrdinalIgnoreCase))
                .ToList();

            return target;
        }
    }
}
