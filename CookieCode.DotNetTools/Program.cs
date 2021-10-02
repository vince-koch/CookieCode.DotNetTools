using System;

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
                Parser.Default
                    .ParseArguments<
                        BumpVersionCommand,
                        CleanupCommand,
                        FixNamespacesCommand,
                        FixProjectRefsCommand,
                        PruneCommand,
                        SendHttpCommand,
                        UnzipCommand,
                        ZipCommand,
                        ZipPublishCommand,
                        ZipSourceCommand>(args)
                    .WithParsed<BumpVersionCommand>(command => command.Execute())
                    .WithParsed<CleanupCommand>(command => command.Execute())
                    .WithParsed<FixNamespacesCommand>(command => command.Execute())
                    .WithParsed<FixProjectRefsCommand>(command => command.Execute())
                    .WithParsed<PruneCommand>(command => command.Execute())
                    .WithParsed<SendHttpCommand>(command => command.Execute())
                    .WithParsed<UnzipCommand>(command => command.Execute())
                    .WithParsed<ZipCommand>(command => command.Execute())
                    .WithParsed<ZipPublishCommand>(command => command.Execute())
                    .WithParsed<ZipSourceCommand>(command => command.Execute())
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
