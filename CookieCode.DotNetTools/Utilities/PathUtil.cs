using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration.UserSecrets;

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

        public static string? GetUserSecretsPath(Assembly assembly)
        {
            // Get the UserSecretsId from assembly
            var id = assembly.GetCustomAttribute<UserSecretsIdAttribute>()?.UserSecretsId;
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            // Get the base path depending on OS
            string root = Environment.OSVersion.Platform == PlatformID.Win32NT
                ? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                : Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            string path = Environment.OSVersion.Platform == PlatformID.Win32NT
                ? Path.Combine(root, "Microsoft", "UserSecrets", id, "secrets.json")
                : Path.Combine(root, ".microsoft", "usersecrets", id, "secrets.json");

            return path;
        }
    }
}
