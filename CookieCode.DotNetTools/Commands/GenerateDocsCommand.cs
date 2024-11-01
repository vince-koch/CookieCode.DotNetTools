using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using CommandLine;

namespace CookieCode.DotNetTools.Commands
{
    [Verb("generate-docs", HelpText = "Document an assembly")]
    public class GenerateDocsCommand : ICommand
    {
        [Value(0, Required = true, HelpText = "Target document path")]
        [Option('s', "source", Required = true, HelpText = "Source assembly path")]
        public required string AssemblyPath { get; set; }

        [Value(1, Required = true, HelpText = "Target document path")]
        [Option('t', "target", Required = true, HelpText = "Target document path")]
        public required string DocumentPath { get; set; }

        public void Execute()
        {
            var assembly = Assembly.LoadFrom(AssemblyPath);

            var filename = Path.GetFileNameWithoutExtension(assembly.Location);

            var commandTypes = assembly.GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract)
                .Where(type => typeof(ICommand).IsAssignableFrom(type))
                .Where(type => type.GetCustomAttribute<VerbAttribute>() != null)
                .ToArray();

            var builder = new StringBuilder()
                .AppendLine($"# {filename}")
                .AppendLine()
                .AppendLine(GetCommandTableOfContents(commandTypes))
                .AppendLine($"Generated at {DateTime.Now}")
                .AppendLine();

            foreach (var commandType in commandTypes)
            {
                var verb = commandType.GetCustomAttribute<VerbAttribute>().ThrowIfNull();
                builder.AppendLine(GetCommandHelp(commandType, verb));
            }

            var path = Path.GetFullPath("ReadMe.md");
            File.WriteAllText(path, builder.ToString());
        }

        private string GetCommandTableOfContents(Type[] commandTypes)
        {
            var builder = new StringBuilder()
                .AppendLine("### Command List")
                .AppendLine();

            foreach (var type in commandTypes)
            {
                var verb = type.GetCustomAttribute<VerbAttribute>();
                if (verb != null)
                {
					builder.AppendLine($"* [{verb.Name}](#{verb.Name})");
				}
            }

            return builder.ToString();
        }

        private string GetCommandHelp(Type commandType, VerbAttribute verb)
        {
            var builder = new StringBuilder()
                .AppendLine($"### {verb.Name}")
                .AppendLine()
                .AppendLine(verb.HelpText);

            var properties = commandType.GetProperties()
                .Where(property => property.GetCustomAttribute<ValueAttribute>() != null
                    || property.GetCustomAttribute<OptionAttribute>() != null)
                .ToArray();

            if (properties.Any())
            {
                builder.AppendLine()
                    .AppendLine("|Arguments |Description |")
                    .AppendLine("|----------|------------|");

                foreach (var property in properties)
                {
                    var value = property.GetCustomAttribute<ValueAttribute>();
                    var option = property.GetCustomAttribute<OptionAttribute>();

                    var arguments = GetArguments(property, value, option);
                    string description = GetDescriptions(property, value, option);
                    builder.AppendLine($"|{arguments}|{description}|");
                }
            }

            return builder.ToString();
        }

        private string GetArguments(PropertyInfo property, ValueAttribute? value, OptionAttribute? option)
        {
            var arguments = new List<string>();

            if (value != null && value.Index > -1)
            {
                arguments.Add($"Position {value.Index}");
            }

            if (option != null && !string.IsNullOrWhiteSpace(option.ShortName))
            {
                arguments.Add($"-{option.ShortName}");
            }

            if (option != null && !string.IsNullOrWhiteSpace(option.LongName))
            {
                arguments.Add($"--{option.LongName}");
            }

            return string.Join("<br/>", arguments);
        }

        private string GetDescriptions(PropertyInfo property, ValueAttribute? value, OptionAttribute? option)
        {
            var description = new List<string>();

            if ((value != null && !string.IsNullOrWhiteSpace(value.HelpText)
                || (option != null && !string.IsNullOrWhiteSpace(option.HelpText))))
            {
                description.Add(option?.HelpText ?? value?.HelpText ?? throw new ArgumentNullException());
            }

            if ((value != null && value.Required)
                || (option != null && option.Required))
            {
                description.Add("Required");
            }

            if ((value != null && value.Default != null)
                || (option != null && option.Default != null))
            {
                description.Add($"Default: {value?.Default ?? option?.Default}");
            }

            return string.Join("<br/>", description);
        }
    }
}
