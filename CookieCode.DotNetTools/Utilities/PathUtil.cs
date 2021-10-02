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
    }
}
