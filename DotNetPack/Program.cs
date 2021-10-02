using System;
using System.Diagnostics;
using System.IO;

namespace DotNetPack
{
    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    var current = Directory.GetCurrentDirectory();
                    PackFolder(current);
                }
                else if (args.Length == 1)
                {
                    if (Directory.Exists(args[0]))
                    {
                        PackFolder(args[0]);
                    }
                    ////else if (File.Exists(args[0]) && Path.GetExtension(args[0]) == ".sln")
                    ////{
                    ////    PackSolution(args[0]);
                    ////}
                    else if (File.Exists(args[0]) && Path.GetExtension(args[0]) == ".csproj")
                    {
                        PackProject(args[0]);
                    }
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Success");
                return 0;
            }
            catch (Exception thrown)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(thrown);
                Debug.WriteLine(thrown);

                return -1;
            }
            finally
            {
                Console.ResetColor();
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
            ////var csproject = new CsProject(projectPath);

            ////var packageSources = Nuget.GetPackageSources();
            ////foreach (var packageSource in packageSources)
            ////{
            ////    var result = Nuget.Find(csproject.Name);
            ////}
        }
    }
}
