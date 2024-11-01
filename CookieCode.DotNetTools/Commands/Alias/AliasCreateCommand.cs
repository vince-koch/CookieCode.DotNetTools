using CookieCode.DotNetTools.Utilities;

using Spectre.Console.Cli;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace CookieCode.DotNetTools.Commands.Alias
{
    [Description("Creates a bat file to be used as an alias")]
    internal class AliasCreateCommand : Command<AliasCreateCommand.Settings>
    {
        public class Settings : CommandSettings
        {
            [CommandArgument(0, "[alias]")]
            [Description("Name for the alias")]
            public string Alias { get; set; }

            [CommandArgument(1, "[executable]")]
            [Description("Path to the target executable")]
            public string? ExePath { get; set; }

            [CommandOption("--nowait")]
            [Description("Do not wait for program to complete; Primarily useful for launching GUI applications;  Default=false")]
            public bool IsNowait { get; set; } = false;

            [CommandOption("--here")]
            [Description("Should create the alias in the current working directory rather than in ALIAS_HOME; Default=false")]
            public bool ShouldCreateHere { get; set; } = false;
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(settings.Alias, nameof(settings.Alias));
            ArgumentException.ThrowIfNullOrWhiteSpace(settings.ExePath, nameof(settings.ExePath));

            var exePath = PathUtil.NormalizePath(settings.ExePath);
            if (!File.Exists(exePath))
            {
                throw new FileNotFoundException($"File does not exist: {exePath}");
            }

            var targetFolder = settings.ShouldCreateHere
                ? Directory.GetCurrentDirectory()
                : Env.Instance.ALIAS_HOME;

            if (string.IsNullOrWhiteSpace(Env.Instance.ALIAS_HOME))
            {
                return Exit.Error("ALIAS_HOME is not set");
            }

            if (!Directory.Exists(targetFolder))
            {
                throw new DirectoryNotFoundException($"Target folder does not exist: {targetFolder}");
            }

            GenerateFile(settings.Alias, targetFolder, exePath, settings.IsNowait);

            return 0;
        }

        private void GenerateFile(string alias, string folder, string exePath, bool isNoWait)
        {
            var lines = new List<string>();
            lines.Add($"@echo off");
            lines.Add($"setlocal");
            lines.Add($"");
            lines.Add($"set EXE={exePath}");
            lines.Add($"set SHOULD_WAIT={(isNoWait ? 0 : 1)}");
            lines.Add($"");
            lines.Add($"if %SHOULD_WAIT%==1 (");
            lines.Add($"    \"%EXE%\" %*");
            lines.Add($") else (");
            lines.Add($"    start \"\" \"%EXE%\" %*"); 
            lines.Add($")");
            lines.Add($"");
            lines.Add($"endlocal");

            var filename = !alias.EndsWith(".bat", StringComparison.OrdinalIgnoreCase) ? alias + ".bat" : alias;
            var path = Path.Combine(folder, filename);

            File.WriteAllLines(path, lines);            
        }
    }
}
