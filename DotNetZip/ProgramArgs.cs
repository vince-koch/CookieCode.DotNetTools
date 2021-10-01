using System.Collections.Generic;

using CommandLine;

namespace DotNetZip
{
    public class ProgramArgs
    {
        [Option('d', "directory", Required = false, HelpText = "Set the starting folder")]
        public string Directory { get; set; }

        [Option('z', "zip", Required = false, HelpText = "Path of the zip file")]
        public string ZipPath { get; set; }

        [Option('r', "rules", Required = false, HelpText = "Add one or more exclude pattern rules")]
        public IEnumerable<string> Rules { get; set; }
    }
}
