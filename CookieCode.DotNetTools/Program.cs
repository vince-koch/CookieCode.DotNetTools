using System;
using System.Linq;

using CommandLine;

using CookieCode.DotNetTools.Commands;

namespace CookieCode.DotNetTools
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var exitCode = 0;

            try
            {
                var commandTypes = typeof(Program).Assembly.GetTypes()
                    .Where(type => type.IsClass && !type.IsAbstract)
                    .Where(type => typeof(ICommand).IsAssignableFrom(type))
                    .ToArray();
                    
                Parser.Default.ParseArguments(args, commandTypes)
                    .WithParsed<ICommand>(command => command.Execute())
                    .WithNotParsed(errors => exitCode = 2);
            }
            catch (Exception thrown)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(thrown);
                exitCode = 1;
            }
            finally
            {
                Console.ResetColor();
            }

            return exitCode;
        }
    }
}
