using CookieCode.DotNetTools.Commands.Alias;
using CookieCode.DotNetTools.Commands.Nvm;
using CookieCode.DotNetTools.Commands.Source;
using CookieCode.DotNetTools.Commands.Zip;

using Spectre.Console.Cli;

using System;
using System.Threading.Tasks;

namespace CookieCode.DotNetTools
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            try
            {
                var app = new CommandApp();

                app.Configure(config =>
                    {
                        config.AddBranch("alias", config =>
                        {
                            config.SetDescription("Manage aliases to executable applications");
                            config.SetDefaultCommand<AliasCreateCommand>();
                            config.AddCommand<AliasCreateCommand>("create");
                            config.AddCommand<AliasHomeCommand>("home");
                            config.AddCommand<AliasListCommand>("list");
                        });

                        config.AddBranch("nvm", config =>
                        {
                            config.SetDescription("Unofficial Node Version Manager for Windows which does not require admin rights");
                            config.AddCommand<NvmHomeCommand>("home");
                            config.AddCommand<NvmListCommand>("list");
                            config.AddCommand<NvmUseCommand>("use");
                        });

                        config.AddBranch("source", config =>
                        {
                            config.SetDescription("Some utilities to help manage and manipulate source code");
                            config.AddCommand<SourceCleanCommand>("clean");
                            config.AddCommand<SourceNamespaceCommand>("namespace").WithAlias("ns");
                            config.AddCommand<SourceZipCommand>("zip");
                            config.AddCommand<SourceZipBinCommand>("bin").WithAlias("zip-bin").WithAlias("bin-zip");
                        });

                        config.AddCommand<ZipCommand>("zip");
                        config.AddCommand<UnzipCommand>("unzip");
                    });

                var exitCode = await app.RunAsync(args);

                return exitCode;
            }
            catch (Exception thrown)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(thrown);
                return 99;
            }
            finally
            {
                Console.ResetColor();
            }
        }
    }
}
