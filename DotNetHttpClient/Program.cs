using System;
using System.Net.Http;

using CommandLine;

namespace DotNetHttpClient
{
    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                int exitCode = 0;

                Parser.Default.ParseArguments<ProgramArgs>(args)
                    .WithParsed<ProgramArgs>(options => Process(options))
                    .WithNotParsed(errors => exitCode = -1);

                return exitCode;
            }
            catch (Exception thrown)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(thrown);
                return 1;
            }
            finally
            {
                Console.ResetColor();
            }
        }

        private static void Process(ProgramArgs  args)
        {
            var factory = new HttpRequestMessageFactory();
            var request = factory.LoadFile(args.InputPath);

            var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(args.TimeoutInSeconds);

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
