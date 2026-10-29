using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Terminal.Gui.App;
using Terminal.Gui.Configuration;
using Terminal.Gui.ViewBase;
using CookieCode.DotNetTools.Commands.Editor.Options;
using CookieCode.DotNetTools.Commands.Editor.Services;

namespace CookieCode.DotNetTools.Commands.Editor.Ui
{
    public sealed class IdeApplication(
        StartupTarget target,
        IFileService files,
        IOptions<EditorOptions> options,
        ILogger<IdeApplication> logger)
    {
        public int Run()
        {
            logger.LogInformation("Starting in {Root}, file {File}", target.RootDirectory, target.FilePath);

            using IApplication app = Application.Create();
            app.Init();

            if (!string.IsNullOrWhiteSpace(options.Value.Theme))
            {
                ThemeManager.Theme = options.Value.Theme;
            }

            using var window = new MainWindow(app, target, files, options.Value, logger);
            app.Run(window);
            return 0;
        }
    }
}
