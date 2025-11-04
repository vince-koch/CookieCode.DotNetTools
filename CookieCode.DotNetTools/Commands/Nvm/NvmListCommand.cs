using Spectre.Console.Cli;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System;
using CliWrap;
using System.ComponentModel;
using System.Threading;

namespace CookieCode.DotNetTools.Commands.Nvm
{
    [Description("Lists the installed node versions")]
    internal class NvmListCommand : AsyncCommand<NvmListCommand.Settings>
    {
        public class Settings : CommandSettings
        {
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(Env.Instance.NVM_HOME))
            {
                return Exit.Error($"{nameof(Env.Instance.NVM_HOME)} has not been set");
            }

            if (!Directory.Exists(Env.Instance.NVM_HOME))
            {
                return Exit.Error($"{nameof(Env.Instance.NVM_HOME)} folder [{Env.Instance.NVM_HOME}] does not exist");
            }

            var listing = await GetInstallations();
            foreach (var installation in listing)
            {
                if (string.Equals(installation.Folder, Env.Instance.NVM_CURRENT, StringComparison.OrdinalIgnoreCase))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"  * {installation.Version}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"    {installation.Version}");
                }
            }

            return 0;
        }

        public static async Task<NodeInstallation[]> GetInstallations()
        {
            if (string.IsNullOrWhiteSpace(Env.Instance.NVM_HOME) || !Directory.Exists(Env.Instance.NVM_HOME))
            {
                return Array.Empty<NodeInstallation>();
            }

            var nodePaths = Directory.GetFiles(
                Env.Instance.NVM_HOME,
                "node.exe",
                new EnumerationOptions { RecurseSubdirectories = true, MaxRecursionDepth = 2 });

            var list = new List<NodeInstallation>();
            foreach (var nodePath in nodePaths)
            {
                var stdOutBuffer = new StringBuilder();

                var result = await Cli.Wrap(nodePath)
                    .WithArguments(["--version"])
                    .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
                    .ExecuteAsync();

                var version = stdOutBuffer.ToString().Trim();
                var installation = new NodeInstallation(nodePath, version);
                list.Add(installation);
            }

            return list.ToArray();
        }
    }
}
