using CookieCode.DotNetTools.Utilities;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace CookieCode.DotNetTools.Commands.Source
{
    public static class ZipUtil
    {
        public static long CreateZipFile(string zipPath, List<string> files, Action<int, string>? progress)
        {
            int fileCount = 0;

            var zipRoot = FindCommonRoot(files);
            using (var stream = new FileStream(zipPath, FileMode.Create))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, true))
            {
                foreach (var file in files)
                {
                    fileCount++;
                    var relativePath = Path.GetRelativePath(zipRoot, file);
                    var shortPath = PathUtil.GetShortPath(relativePath);

                    progress?.Invoke(fileCount, shortPath);

                    archive.CreateEntryFromFile(file, relativePath);
                }
            }

            return fileCount;
        }

        public static string FindCommonRoot(List<string> paths)
        {
            if (paths == null || paths.Count == 0)
            {
                return string.Empty; // Return an empty string if the list is empty or null
            }

            // Convert each path to its absolute form and ensure they are in the same format
            var normalizedPaths = new List<string>();
            foreach (var path in paths)
            {
                normalizedPaths.Add(Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar));
            }

            // Start with the first path as the base for comparison
            string commonRoot = normalizedPaths[0];

            foreach (var path in normalizedPaths)
            {
                commonRoot = GetCommonPrefix(commonRoot, path);
                if (string.IsNullOrEmpty(commonRoot)) break;
            }

            return commonRoot;
        }

        private static string GetCommonPrefix(string path1, string path2)
        {
            var components1 = path1.Split(Path.DirectorySeparatorChar);
            var components2 = path2.Split(Path.DirectorySeparatorChar);
            int length = Math.Min(components1.Length, components2.Length);

            int commonLength = 0;
            for (int i = 0; i < length; i++)
            {
                if (components1[i] == components2[i])
                {
                    commonLength++;
                }
                else
                {
                    break;
                }
            }

            // Reconstruct the common path from the common components
            return string.Join(Path.DirectorySeparatorChar.ToString(), components1, 0, commonLength);
        }
    }
}
