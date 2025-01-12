using CookieCode.DotNetTools.Utilities;

using Spectre.Console.Cli;

using System.ComponentModel;
using System.IO;

namespace CookieCode.DotNetTools.Commands.Unused
{
    //[Verb("pack", HelpText = "")]
    [Description("Pack a folder, project or solution")]
    public class PackCommand : Command<PackCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            //[Value(0, HelpText = "Source folder, project, or solution")]
            [CommandArgument(0, "<source-path>")]
            [Description("Source folder, project, or solution")]
            public required string SourcePath { get; set; }
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            var sourcePath = settings.SourcePath ?? Directory.GetCurrentDirectory();

            if (Directory.Exists(sourcePath))
            {
                PackFolder(sourcePath);
            }
            ////else if (File.Exists(sourcePath) && Path.GetExtension(sourcePath) == ".sln")
            ////{
            ////    PackSolution(sourcePath);
            ////}
            else if (File.Exists(sourcePath) && Path.GetExtension(sourcePath) == ".csproj")
            {
                PackProject(sourcePath);
            }

            return 0;
        }

        private static void PackFolder(string folderPath)
        {
            var projectPaths = Directory.GetFiles(folderPath, "*.csproj", SearchOption.AllDirectories);
            foreach (var projectPath in projectPaths)
            {
                PackProject(projectPath);
            }
        }

        ////private static void PackSolution(string solutionPath)
        ////{
        ////}

        private static void PackProject(string projectPath)
        {
            var csproject = new CsProject(projectPath);

            var packageSources = NugetUtil.GetPackageSources();
            foreach (var packageSource in packageSources)
            {
                var result = NugetUtil.FindVersions(packageSource, csproject.Name);
            }
        }
    }
}
