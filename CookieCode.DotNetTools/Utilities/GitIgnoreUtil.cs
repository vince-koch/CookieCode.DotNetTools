using System.Collections.Generic;
using System.IO;
using System.Linq;

using MAB.DotIgnore;

namespace CookieCode.DotNetTools.Utilities
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
        public static string[] GetFiles(IgnoreList ignoreList, string directory)
        {
            var list = new List<string>();
            Process(ignoreList, directory, list);
            return list.ToArray();
        }

        public static string[] GetIgnoredFiles(IgnoreList ignoreList, string directory)
        {
            var list = new List<string>();
            Process(ignoreList, directory, list);
            return list.ToArray();
        }

        private static void Process(
            this IgnoreList ignoreList, 
            string currentDirectory,
            List<string> list)
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
                if (!ignoreList.IsIgnored(directory, pathIsDirectory: true))
                {
                    Process(ignoreList, directory, list);
                }
            }

            // process files
            var files = Directory.GetFiles(currentDirectory);
            foreach (var file in files)
            {
                if (!ignoreList.IsIgnored(file, pathIsDirectory: false))
                {
                    list.Add(file);
                }
            }
        }
    }
}
