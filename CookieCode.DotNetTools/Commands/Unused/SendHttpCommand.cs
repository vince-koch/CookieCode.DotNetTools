using CookieCode.DotNetTools.Utilities;

using Spectre.Console.Cli;

using System;
using System.ComponentModel;
using System.Net.Http;

namespace CookieCode.DotNetTools.Commands.Unused
{
    //[Verb("send-http", HelpText = "Sends a raw http request")]
    [Description("Sends a raw http request")]
    public class SendHttpCommand : Command<SendHttpCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            //[Value(0)]
            //[Option('i', "input", HelpText = "The path to the text file containing the HTTP request to send")]
            [CommandArgument(0, "<input-path>")]
            [Description("The path to the text file containing the HTTP request to send")]
            public required string InputPath { get; set; }

            //[Option('t', "timeout", Required = false, HelpText = "The timeout (in seconds) to wait, default is 60")]
            [CommandOption("-t|--timeout <timeout>")]
            [Description("The timeout (in seconds) to wait, default is 60")]
            public int TimeoutInSeconds { get; set; } = 60;
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            var factory = new HttpRequestMessageFactory();
            var request = factory.LoadFile(settings.InputPath);

            var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(settings.TimeoutInSeconds);

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

            return 0;
        }
    }
}
