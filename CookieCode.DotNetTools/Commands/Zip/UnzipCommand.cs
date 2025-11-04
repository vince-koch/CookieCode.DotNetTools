using Spectre.Console.Cli;

using System.ComponentModel;
using System.IO.Compression;
using System.Threading;

namespace CookieCode.DotNetTools.Commands.Zip
{
    [Description("Unzips a zip archive")]
    public class UnzipCommand : Command<UnzipCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [CommandOption("-z|--zip <ZipPath>")]
            [Description("Source zip archive")]
            public required string ZipPath { get; set; }

            [CommandOption("-t|--target <TargetPath>")]
            [Description("Target folder")]
            public required string TargetPath { get; set; }

            [CommandOption("-o|--overwrite")]
            [Description("Overwrite files")]
            public bool OverwriteFiles { get; set; } = true;
        }

        public override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
        {
            ZipFile.ExtractToDirectory(
                settings.ZipPath,
                settings.TargetPath,
                settings.OverwriteFiles);

            return 0;
        }
    }
}
