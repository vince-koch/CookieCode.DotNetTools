using Spectre.Console;
using Spectre.Console.Cli;

using System;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace CookieCode.DotNetTools.Commands.Alias
{
    [Description("Lists the aliases which exist in the ALIAS_HOME directory")]
    internal class AliasListCommand : Command<AliasListCommand.Settings>
    {
        public class Settings : CommandSettings
        {
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            if (string.IsNullOrWhiteSpace(Env.Instance.ALIAS_HOME))
            {
                return Exit.Error("ALIAS_HOME is not set");
            }

            var files = Directory
                .GetFiles(Env.Instance.ALIAS_HOME, "*.*", SearchOption.TopDirectoryOnly)
                .OrderBy(file => file)
                .ToList();

			//files.ForEach(file => Console.WriteLine($"    {Path.GetFileName(file)}"));

			var exePathPrefix = $"set {AliasCreateCommand.EXE_PATH}=";
            var exeWaitPrefix = $"set {AliasCreateCommand.EXE_WAIT}=";

            var table = new Table();
            table.AddColumn("Alias");
			table.AddColumn("Exe Path");
			table.AddColumn("Wait");

			foreach (var file in files)
			{
				var lines = File.ReadAllLines(file);

				var filename = Path.GetFileName(file);

                var exePath = lines
                    .Where(line => line.StartsWith(exePathPrefix))
                    .Select(line => line.Substring(exePathPrefix.Length))
                    .Select(line => line.Trim('\"'))
                    .SingleOrDefault();

                var exeWait = lines
					.Where(line => line.StartsWith(exeWaitPrefix))
					.Select(line => line.Substring(exeWaitPrefix.Length))
                    .Select(line => line == "1" ? true : false)
					.SingleOrDefault();

                if (!string.IsNullOrWhiteSpace(exePath))
                {
                    if (!File.Exists(exePath))
                    {
                        var escaped = exePath.Replace("[", "[[").Replace("]", "]]");
						exePath = $"[red]{escaped}[/]";
                    }
                }

				table.AddRow(
                    filename,
                    exePath ?? string.Empty,
                    exeWait.ToString());
			}

            AnsiConsole.Write(table);

            return 0;
        }
    }
}
