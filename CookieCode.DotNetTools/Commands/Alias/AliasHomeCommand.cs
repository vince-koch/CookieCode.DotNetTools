using CookieCode.DotNetTools.Utilities;

using Spectre.Console.Cli;

using System;
using System.ComponentModel;
using System.IO;

namespace CookieCode.DotNetTools.Commands.Alias
{
    [Description("Gets or sets the ALIAS_HOME folder and ensures it is added to the user's PATH environment variable")]
    internal class AliasHomeCommand : Command<AliasHomeCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [CommandArgument(0, "[folder]")]
            [Description("The alias to set ALIAS_HOME environment variable to")]
            public string? Folder { get; set; }
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            // handle get ALIAS_HOME
            if (string.IsNullOrWhiteSpace(settings.Folder))
            {
                if (string.IsNullOrWhiteSpace(Env.Instance.ALIAS_HOME))
                {
                    return Exit.Error("ALIAS_HOME is not set");
                }

                Console.WriteLine($"{nameof(Env.Instance.ALIAS_HOME)}={Env.Instance.ALIAS_HOME}");
                return Exit.Success();
            }

            // handle set NVM_HOME
            var normalizedPath = PathUtil.NormalizePath(settings.Folder);
            if (!Directory.Exists(normalizedPath))
            {
                return Exit.Error($"Folder [{settings.Folder}] does not exist");
            }

            if (!string.IsNullOrWhiteSpace(Env.Instance.ALIAS_HOME))
            {
                // remove the old ALIAS_HOME from the path
                UserPathUtil.RemovePath(Env.Instance.ALIAS_HOME);
            }

            Env.Instance.ALIAS_HOME = normalizedPath;
            UserPathUtil.AddPath(normalizedPath);

            Console.WriteLine($"{nameof(Env.Instance.ALIAS_HOME)}={Env.Instance.ALIAS_HOME}");
            return Exit.Success();
        }
    }
}
