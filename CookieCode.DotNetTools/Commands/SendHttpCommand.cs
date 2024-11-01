using System;
using System.Net.Http;

using CommandLine;

using CookieCode.DotNetTools.Utilities;

namespace CookieCode.DotNetTools.Commands
{
    [Verb("send-http", HelpText = "Sends a raw http request")]
    public class SendHttpCommand : ICommand
    {
        [Value(0)]
        [Option('i', "input", HelpText = "The path to the text file containing the HTTP request to send")]
        public required string InputPath { get; set; }

        [Option('t', "timeout", Required = false, HelpText = "The timeout (in seconds) to wait, default is 60")]
        public required int TimeoutInSeconds { get; set; } = 60;

        public void Execute()
        {
            var factory = new HttpRequestMessageFactory();
            var request = factory.LoadFile(InputPath);

            var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(TimeoutInSeconds);

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(request.Method);
            Console.WriteLine(' ');
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(request.RequestUri);

            var response = httpClient.Send(request);

            Console.ForegroundColor = response.IsSuccessStatusCode
                ? ConsoleColor.Green
                : ConsoleColor.Red;

            Console.WriteLine($"{(int)response.StatusCode}: {response.ReasonPhrase}");

            // todo: output whole response to console & optionally file
        }
    }
}
