using Spectre.Console.Cli;

using System;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace CookieCode.DotNetTools.Commands.Nvm
{
    [Description("Gets or sets the NVM_HOME folder, where all node installations will be placed")]
    internal class NvmHomeCommand : Command<NvmHomeCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [CommandArgument(0, "[folder]")]
            public string? Folder { get; set; }
        }

        public override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
        {
            // handle get NVM_HOME
            if (string.IsNullOrWhiteSpace(settings.Folder))
            {
                if (string.IsNullOrWhiteSpace(Env.Instance.NVM_HOME))
                {
                    return Exit.Error("NVM_HOME is not set");
                }

                Console.WriteLine($"{nameof(Env.Instance.NVM_HOME)}={Env.Instance.NVM_HOME}");
                return Exit.Success();
            }

            // handle set NVM_HOME
            if (!Directory.Exists(settings.Folder))
            {
                return Exit.Error($"Folder [{settings.Folder}] does not exist");
            }

            Env.Instance.NVM_HOME = settings.Folder;
            return Exit.Success();
        }
    }
}
