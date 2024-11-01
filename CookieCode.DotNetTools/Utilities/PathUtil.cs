using System.IO;
using System;
using System.Text.RegularExpressions;

namespace CookieCode.DotNetTools.Utilities
{
    public static class PathUtil
    {
        private const string _shortPathPattern = @"^(w+:|)([^]+[^]+).*([^]+[^]+)$";
        private const string _shortPathReplacement = "$1$2...$3";
        private static readonly Regex _shortPathRegex = new Regex(_shortPathPattern);

        public static string GetShortPath(string path)
        {
            var result = _shortPathRegex.IsMatch(path)
                ? _shortPathRegex.Replace(path, _shortPathReplacement)
                : path;

            return result;
        }

        public static string NormalizePath(string path)
        {
            try
            {
                return Path.GetFullPath(path)
                    .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                    .ToLowerInvariant();
            }
            catch (Exception thrown)
            {
                throw new ArgumentException($"Invalid path: {path}", thrown);
            }
        }
    }
}
