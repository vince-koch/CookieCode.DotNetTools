using System.IO.Compression;

using CommandLine;

namespace CookieCode.DotNetTools.Commands
{
    [Verb("unzip", HelpText = "Unzips a zip archive")]
    public class UnzipCommand : ICommand
    {
        [Option('z', "zip", Required = true, HelpText = "Source zip archive")]
        public string ZipPath { get; set; }

        [Option('t', "target", Required = true, HelpText = "Target folder")]
        public string TargetPath { get; set; }

        [Option('o', "overwrite", Default = true, HelpText = "Overwrite files")]
        public bool OverwriteFiles { get; set; }

        public void Execute()
        {
            ZipFile.ExtractToDirectory(ZipPath, TargetPath, OverwriteFiles);
        }
    }
}
