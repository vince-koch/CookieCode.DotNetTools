using Spectre.Console.Cli;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CookieCode.DotNetTools.Commands.Nvm
{
    [Description("Uses an installed version of node")]
    internal class NvmUseCommand : AsyncCommand<NvmUseCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [CommandArgument(0, "<version>")]
            public string Version { get; set; } = string.Empty;
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(settings.Version))
            {
                return Exit.Error($"{nameof(settings.Version)} was not provided");
            }

            var requestedVersion = settings.Version.TrimStart('v', 'V');

            var listing = await NvmListCommand.GetInstallations();

            var selected = listing
                .Where(item => item.Version.TrimStart('v', 'V') == requestedVersion)
                .FirstOrDefault();

            if (selected == null)
            {
                return Exit.Error("No matching version of node was found");
            }

            Env.Instance.ChangeNodePath(selected);
            Console.WriteLine($"Now using node {selected.Version}");

            return Exit.Success();
        }
    }
}
