using CommandLine;

namespace DotNetFix
{
    public class ProgramArgs
    {
        [Option('f', "folder", Required = true, HelpText = "Root folder to begin processing at")]
        public string RootFolder { get; set; }

        [Option('n', "namespace", Required = true, HelpText = "Root namespace to apply to the folder")]
        public string RootNamespace { get; set; }
    }
}
