using System;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;

using CommandLine;

using CookieCode.DotNetTools.Utilities;

namespace CookieCode.DotNetTools.Commands
{
    [Verb("bump-version", HelpText = "Bumps the specified portion of the project version")]
    public class BumpVersionCommand : ICommand
    {
        [Value(0, Required = true, HelpText = "Project file path")]
        public required string ProjectPath { get; set; }

        [Value(1, Required = false, Default = VersionPart.Build, HelpText = "Project file path")]
        public required VersionPart BumpPart { get; set; }

        public void Execute()
        {
            if (!File.Exists(ProjectPath))
            {
                throw new FileNotFoundException(ProjectPath);
            }

            switch (BumpPart)
            {
                case VersionPart.Major:
                    BumpVersion(ProjectPath, VersionUtil.BumpMajor);
                    break;

                case VersionPart.Minor:
                    BumpVersion(ProjectPath, VersionUtil.BumpMinor);
                    break;

                case VersionPart.Build:
                    BumpVersion(ProjectPath, VersionUtil.BumpBuild);
                    break;

                case VersionPart.Revision:
                    BumpVersion(ProjectPath, VersionUtil.BumpRevvision);
                    break;

                default:
                    throw new NotSupportedException($"{nameof(VersionPart)}.{BumpPart} is not supported");
            }
        }

        public static void BumpVersion(string projectPath, Func<Version, Version> bump)
        {
            var csproj = XDocument.Load(projectPath);
            var versionNode = csproj.XPathSelectElement("Project/PropertyGroup/Version");
            if (versionNode == null)
            {
                versionNode = new XElement("Version", "0.0");
                var propertyGroupNode = csproj.XPathSelectElement("Project/PropertyGroup");
                if (propertyGroupNode != null)
                {
                    propertyGroupNode.Add(versionNode);
                }
            }

            var startVersion = Version.Parse(versionNode.Value);
            var newVersion = bump(startVersion);
            versionNode.Value = newVersion.ToString();

            csproj.Save(projectPath);

            Console.WriteLine($"{startVersion} ==> {newVersion}");
        }
    }
}
