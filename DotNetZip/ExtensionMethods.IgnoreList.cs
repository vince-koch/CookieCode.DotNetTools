using System.Collections.Generic;
using System.IO;

using MAB.DotIgnore;

namespace DotNetZip
{
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Gets non-ignored files
        /// </summary>
        public static string[] GetFiles(this IgnoreList ignoreList, string directory)
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
            var directories = Directory.GetDirectories(currentDirectory);
            foreach (var directory in directories)
            {
                if (!ignoreList.IsIgnored(directory, pathIsDirectory: true))
                {
                    Process(ignoreList, directory, list);
                }
            }

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
