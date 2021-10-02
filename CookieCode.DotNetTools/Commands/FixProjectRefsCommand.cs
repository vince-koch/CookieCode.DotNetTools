﻿using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using CommandLine;

namespace CookieCode.DotNetTools.Commands
{
    [Verb("fix-project-refs", HelpText = "Attempts to fix project references")]
    public class FixProjectRefsCommand : ICommand
    {
        [Value(0, HelpText = "Source folder")]
        public string SourceFolder { get; set; }

        public void Execute()
        {
            var sourceFolder = SourceFolder ?? Directory.GetCurrentDirectory();

            var csprojMap = Directory
                .GetFiles(sourceFolder, "*.csproj", SearchOption.AllDirectories)
                .ToDictionary(
                    file => Path.GetFileName(file),
                    file => file);

            foreach (var pair in csprojMap)
            {
                var isDirty = false;

                var projectPath = pair.Value;
                var projectFolder = Path.GetDirectoryName(projectPath);

                var csproj = XDocument.Load(projectPath);

                var projectReferences = csproj.Descendants()
                    .Where(element => element.Name.LocalName == "ProjectReference")
                    .ToArray();

                WriteLine();
                WriteLine(ConsoleColor.White, pair.Key);
                foreach (var projectReference in projectReferences)
                {
                    var includeAttribute = projectReference.Attribute("Include");
                    var currentRelativePath = includeAttribute.Value;
                    var currentProjectName = Path.GetFileName(currentRelativePath);

                    Write(ConsoleColor.Gray, $"    {currentProjectName} ==> ");
                    var currentFullPath = Path.GetFullPath(currentRelativePath, projectFolder);
                    if (File.Exists(currentFullPath))
                    {
                        WriteLine(ConsoleColor.White, "ok");
                        continue;
                    }

                    string lookupFullPath;
                    var filename = Path.GetFileName(currentRelativePath);
                    if (csprojMap.TryGetValue(filename, out lookupFullPath))
                    {
                        var lookupRelativePath = Path.GetRelativePath(projectFolder, lookupFullPath);
                        includeAttribute.Value = lookupRelativePath;
                        isDirty = true;

                        WriteLine(ConsoleColor.Green, lookupRelativePath);
                    }
                    else
                    {
                        WriteLine(ConsoleColor.Red, "no match");
                    }
                }

                if (isDirty)
                {
                    WriteLine(ConsoleColor.Yellow, "Updates saved");
                    csproj.Save(projectPath);
                }
            }
        }

        private static void Write(ConsoleColor color, string text)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
        }

        private static void WriteLine(ConsoleColor color, string text)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
        }

        private static void WriteLine()
        {
            Console.WriteLine();
        }
    }
}
