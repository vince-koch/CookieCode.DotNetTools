using CookieCode.DotNetTools.Utilities;

using Spectre.Console.Cli;

using System;
using System.ComponentModel;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CookieCode.DotNetTools.Commands.Unused
{
    public enum VersionPart
    {
        Major,
        Minor,
        Build,
        Revision
    }

    [Description("Bumps the specified portion of the project version")]
    public class BumpVersionCommand : Command<BumpVersionCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            // required
            [CommandArgument(0, "<project-path>")]
            [Description("Project file path")]
            public required string ProjectPath { get; set; }

            // optional
            [CommandArgument(1, "[version-part]")]
            [Description("major / minor / build (default) / revision")]
            [DefaultValue(VersionPart.Build)]
            public VersionPart BumpPart { get; set; } = VersionPart.Build;
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            if (!File.Exists(settings.ProjectPath))
            {
                throw new FileNotFoundException(settings.ProjectPath);
            }

            switch (settings.BumpPart)
            {
                case VersionPart.Major:
                    return BumpVersion(settings.ProjectPath, VersionUtil.BumpMajor);

                case VersionPart.Minor:
                    return BumpVersion(settings.ProjectPath, VersionUtil.BumpMinor);

                case VersionPart.Build:
                    return BumpVersion(settings.ProjectPath, VersionUtil.BumpBuild);

                case VersionPart.Revision:
                    return BumpVersion(settings.ProjectPath, VersionUtil.BumpRevvision);

                default:
                    throw new NotSupportedException($"{nameof(VersionPart)}.{settings.BumpPart} is not supported");
            }
        }

        public static int BumpVersion(string projectPath, Func<Version, Version> bump)
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

            return 0;
        }
    }
}
