using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using CookieCode.DotNetTools.Utilities;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CookieCode.DotNetTools.Commands.Config
{
    internal abstract class ConfigBaseCommand : Command<ConfigCommandSettings>
    {
        protected static void EnsurePath(ConfigCommandSettings settings)
        {
            TestPathOrAlias(settings.PathOrAlias, settings);

            while (string.IsNullOrWhiteSpace(settings.Path))
            {
                var pathOrAlias = AnsiConsole.Prompt(
                    new TextPrompt<string>("[green]?[/] Enter config path or alias?")
                );

                TestPathOrAlias(pathOrAlias, settings);
            }
        }

        protected static void TestPathOrAlias(string? pathOrAlias, ConfigCommandSettings settings)
        {
            if (!string.IsNullOrWhiteSpace(pathOrAlias)
                && File.Exists(pathOrAlias))
            {
                settings.Path = pathOrAlias;
                return;
            }

            if (!string.IsNullOrWhiteSpace(pathOrAlias))
            {
                var resolvedPath = ResolveAlias(pathOrAlias);
                if (!string.IsNullOrWhiteSpace(resolvedPath))
                {
                    settings.Alias = pathOrAlias;
                    settings.Path = resolvedPath;
                    return;
                }
            }
        }

        protected static string? ResolveAlias(string? alias)
        {
            if (string.IsNullOrWhiteSpace(alias))
            {
                return alias;
            }

            string? rawPath = alias.ToLower() switch
            {
                "aws" => "%UserProfile%\\.aws\\credentials",
                "aws credentials" => "%UserProfile%\\.aws\\credentials",
                "aws config" => "%UserProfile%\\.aws\\config",

                "nuget" => "%AppData%\\.nuget\\NuGet.Config",

                _ => PathUtil.GetUserSecretsPath(typeof(Program).Assembly),
            };

            string? resolvedPath = !string.IsNullOrWhiteSpace(rawPath)
                ? Environment.ExpandEnvironmentVariables(rawPath)
                : null;

            return resolvedPath;
        }

        protected static void OpenInDefaultEditor(string filePath)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Windows: UseShellExecute = true automatically uses default app
                Process.Start(new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // macOS: `open` uses the default registered application
                Process.Start("open", filePath);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Linux: `xdg-open` is the freedesktop standard for default apps
                Process.Start("xdg-open", filePath);
            }
            else
            {
                throw new PlatformNotSupportedException("Unsupported OS platform");
            }
        }
    }
}
