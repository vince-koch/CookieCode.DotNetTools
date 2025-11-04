using CookieCode.DotNetTools.Commands.Alias;
using CookieCode.DotNetTools.Commands.Mongo;
using CookieCode.DotNetTools.Commands.Nvm;
using CookieCode.DotNetTools.Commands.Source;
using CookieCode.DotNetTools.Commands.Zip;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;

using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CookieCode.DotNetTools
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            try
            {
                var services = new ServiceCollection()
                    .AddScoped<IMongoUiCommand, MongoCompassCommand>()
                    .AddScoped<IMongoUiCommand, MongoExpressCommand>()
                    .AddScoped<IMongoUiCommand, MongokuCommand>();

                var registrar = new SpectreTypeRegistrar(services);

                var app = new CommandApp(registrar);

                app.Configure(config =>
                    {
                        config.PropagateExceptions();

                        config.AddBranch("alias", config =>
                        {
                            config.SetDescription("Manage aliases to executable applications");
                            config.SetDefaultCommand<AliasCreateCommand>();
                            config.AddCommand<AliasCreateCommand>("create").WithAlias("add");
                            config.AddCommand<AliasHomeCommand>("home");
                            config.AddCommand<AliasListCommand>("list");
                        });

                        config.AddBranch("mongo", config =>
                        {
                            config.AddCommand<MongoClientCommand>("client").WithAlias("ui");
                            config.AddCommand<MongoCompassCommand>("mongo-compass");
                            config.AddCommand<MongoExpressCommand>("mongo-express");
                            config.AddCommand<MongokuCommand>("mongoku");
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
                AnsiConsole.WriteException(thrown);
                Debug.WriteLine(thrown);
                return 99;
            }
        }
    }
}
