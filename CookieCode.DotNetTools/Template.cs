using System.Collections.Generic;
using System.Linq;

namespace CookieCode.DotNetTools
{
    public static class Template
    {
        public static string Argument(string value, bool isRequired)
        {
            var valueTemplate = Value(value, isRequired);
            return valueTemplate;
        }

        public static string Option(List<string> shortNames, List<string> longNames, string value, bool isRequired)
        {
            var shortNameTemplate = string.Join("|", shortNames.Select(name => name.TrimStart('-')).Select(name => $"-{name}"));
            var longNameTemplate = string.Join("|", longNames.Select(name => name.TrimStart('-')).Select(name => $"--{name}"));
            var nameTemplate = string.Join("|", shortNameTemplate, longNameTemplate);
            var valueTemplate = Value(value, isRequired);
            var template = string.Join(" ", nameTemplate, valueTemplate);
            return template;
        }

        private static string Value(string value, bool isRequired)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }
            else
            {
                return isRequired ? $"<{value}>" : $"[{value}]";
            }
        }
    }
}
