using CookieCode.DotNetTools.Utilities;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CookieCode.DotNetTools.Commands.Alias
{
    internal class UserPathUtil
    {
        private static string PATH
        {
            get => Environment.GetEnvironmentVariable(nameof(PATH), EnvironmentVariableTarget.User) ?? string.Empty;
            set => Environment.SetEnvironmentVariable(nameof(PATH), value, EnvironmentVariableTarget.User);
        }

        private static IEnumerable<string> Paths
        {
            get => (PATH ?? string.Empty).Split(';', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim());
            set => PATH = string.Join(';', value);
        }

        public static bool HasPath(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace("Path cannot be null or whitespace.", nameof(path));
            string normalizedPath = PathUtil.NormalizePath(path);
            bool isInPath = Paths.Any(p => string.Equals(PathUtil.NormalizePath(p), normalizedPath, StringComparison.OrdinalIgnoreCase));
            return isInPath;
        }

        public static void AddPath(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace("Path cannot be null or whitespace.", nameof(path));

            if (HasPath(path))
            {
                return;
            }

            string normalizedPath = PathUtil.NormalizePath(path);

            if (!Directory.Exists(normalizedPath))
            {
                throw new DirectoryNotFoundException($"Directory not found [{normalizedPath}]");
            }

            var paths = Paths.ToList();
            paths.Add(normalizedPath);
            Paths = paths;
        }

        public static void RemovePath(string path)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(path, nameof(path));
            
            string normalizedPath = PathUtil.NormalizePath(path);

            var paths = Paths.ToList();
            var count = paths.RemoveAll(p => string.Equals(PathUtil.NormalizePath(p), normalizedPath, StringComparison.OrdinalIgnoreCase));
            if (count > 0)
            {
                Paths = paths;
            }
        }
    }
}
