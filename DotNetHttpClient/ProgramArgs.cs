using CommandLine;

namespace DotNetHttpClient
{
    public class ProgramArgs
    {
        [Value(0)]
        [Option('i', "input", HelpText = "The path to the text file containing the HTTP request to send")]
        public string InputPath { get; set; }

        [Option('i', "input", Required = false, HelpText = "The timeout (in seconds) to wait, default is 60")]
        public int TimeoutInSeconds { get; set; } = 60;
    }
}