using Spectre.Console;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CookieCode.DotNetTools.Commands.Mongo
{
    internal class BrowserUtil
    {
        public static void OpenUrl(string url, string? username, string? password)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("URL cannot be null or empty.", nameof(url));
            }

            AnsiConsole.MarkupLine($"Opening url [cyan]{url}[/]");

            try
            {
                var builder = new UriBuilder(url);

                if (!string.IsNullOrWhiteSpace(username))
                {
                    builder.UserName = Uri.EscapeDataString(username);
                }

                if (!string.IsNullOrWhiteSpace(password))
                {
                    builder.Password = Uri.EscapeDataString(password);
                }

                // .NET 6+ can open URLs safely with UseShellExecute
                Process.Start(new ProcessStartInfo
                {
                    FileName = builder.Uri.AbsoluteUri,
                    UseShellExecute = true,
                });
            }
            catch
            {
                // fallback for older or sandboxed environments
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url.Replace("&", "^&")}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
