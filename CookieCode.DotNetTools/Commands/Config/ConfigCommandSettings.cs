using Spectre.Console.Cli;

namespace CookieCode.DotNetTools.Commands.Config
{
    internal class ConfigCommandSettings : CommandSettings
    {
        [CommandArgument(0, "<PATH_OR_ALIAS>")]
        public string? PathOrAlias { get; set; }

        internal string? Alias { get; set; }

        internal string? Path { get; set; }
    }
}
