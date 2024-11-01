using Spectre.Console.Cli;

using System;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace CookieCode.DotNetTools.Commands.Alias
{
    [Description("Lists the aliases which exist in the ALIAS_HOME directory")]
    internal class AliasListCommand : Command<AliasListCommand.Settings>
    {
        public class Settings : CommandSettings
        {
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            if (string.IsNullOrWhiteSpace(Env.Instance.ALIAS_HOME))
            {
                return Exit.Error("ALIAS_HOME is not set");
            }

            var files = Directory
                .GetFiles(Env.Instance.ALIAS_HOME, "*.*", SearchOption.TopDirectoryOnly)
                .Select(file => Path.GetFileName(file))
                .OrderBy(file => file)
                .ToList();

            files.ForEach(file => Console.WriteLine($"    {file}"));

            return 0;
        }
    }
}
