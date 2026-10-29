using CookieCode.DotNetTools.Commands.Editor.Services;
using CookieCode.DotNetTools.Commands.Editor.Ui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console.Cli;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CookieCode.DotNetTools.Commands.Editor
{
    [Description("Open a lightweight file explorer and editor")]
    internal class EditorCommand : AsyncCommand<EditorCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [CommandArgument(0, "[path]")]
            public string Path { get; set; } = Directory.GetCurrentDirectory();
        }

        private readonly IServiceProvider _serviceProvider;

        public static void AddServices(IHostApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IFileService, FileService>();
            builder.Services.AddSingleton<StartupTarget>();
            builder.Services.AddSingleton<IdeApplication>();
        }

        public EditorCommand(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            using var scope = _serviceProvider.CreateAsyncScope();
            
            var startupTarget = scope.ServiceProvider.GetRequiredService<StartupTarget>();
            if (File.Exists(settings.Path))
            {
                startupTarget.RootDirectory = Path.GetDirectoryName(settings.Path).ThrowIfNull();
                startupTarget.FilePath = Path.GetFileName(settings.Path);
            }
            else if (Directory.Exists(settings.Path))
            {
                startupTarget.RootDirectory = settings.Path;
                startupTarget.FilePath = null;
            }
            else
            {
                startupTarget.RootDirectory = Directory.GetCurrentDirectory();
                startupTarget.FilePath = null;
            }

            var application = scope.ServiceProvider.GetRequiredService<IdeApplication>();
            var exitCode = application.Run();

            return exitCode;
        }
    }
}
