using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine;

using CookieCode.DotNetTools.Utilities;

namespace CookieCode.DotNetTools.Commands
{
    [Verb("pack", HelpText = "")]
    public class PackCommand : ICommand
    {
        [Value(0, HelpText = "Source folder, project, or solution")]
        public string SourcePath { get; set; }

        public void Execute()
        {
            var sourcePath = SourcePath ?? Directory.GetCurrentDirectory();

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
