#define MAB_DotIgnore

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CookieCode.DotNetTools.Commands.Source
{
    public class GitIgnore
    {
        private readonly List<string> _rules = new List<string>();

        public GitIgnore()
        {
        }

        public GitIgnore(GitIgnore gitIgnore)
        {
            AddRules(gitIgnore._rules);
        }

        public GitIgnore AddDefaultRules()
        {
            var rules = new string[]
            {
                "[Bb]in/",
                "[Oo]bj/",
                "packages/",
                "node_modules/",
            };

            _rules.AddRange(rules);
            return this;
        }

        public GitIgnore AddRule(string rule)
        {
            _rules.Add(rule);
            return this;
        }

        public GitIgnore AddRules(IEnumerable<string> rules)
        {
            _rules.AddRange(rules);
            return this;
        }

        public GitIgnore AddRules(params string[] rules)
        {
            _rules.AddRange(rules.AsEnumerable());
            return this;
        }

        public GitIgnore AddRuleFile(string path)
        {
            var rules = ReadRulesFromFile(path);
            _rules.AddRange(rules);
            return this;
        }

        public record class GitIgnoreResult(string Path, bool IsDirectory, bool IsIgnored);

        public List<GitIgnoreResult> Process(string directory, bool isRecursive, bool canReadGitIgnores, Func<GitIgnoreResult, bool> filter)
        {
#if MAB_DotIgnore
            var ignore = new MAB.DotIgnore.IgnoreList();
            if (_rules.Any())
            {
                ignore.AddRules(_rules);
            }
#endif

            List<GitIgnoreResult> list = new List<GitIgnoreResult>();

            ProcessInternal(
                ignore,
                directory,
                isRecursive,
                canReadGitIgnores,
                (path, isDirectory, isIgnored) =>
                {
                    var item = new GitIgnoreResult(path, isDirectory, isIgnored);
                    if (filter(item))
                    {
                        list.Add(item);
                    }
                });

            return list;
        }

        private delegate void ProcessInternalCallback(string path, bool isDirectory, bool isIgnored);

#if MAB_DotIgnore
        private void ProcessInternal(MAB.DotIgnore.IgnoreList ignore, string currentDirectory, bool isRecursive, bool canReadGitIgnores, ProcessInternalCallback callback)
        {
            if (canReadGitIgnores)
            {
                // if we find a .gitignore file, recreate the ignore object so as to not bleed into sibling directories
                var gitIgnorePath = Path.Combine(currentDirectory, ".gitignore");
                if (File.Exists(gitIgnorePath))
                {
                    var gitIgnoreRules = ReadRulesFromFile(gitIgnorePath);
                    var reignore = ignore.Clone();
                    reignore.AddRules(gitIgnoreRules);
                    ignore = reignore;
                }
            }

            // process files
            var files = Directory.GetFiles(currentDirectory);
            foreach (var file in files)
            {
                var isIgnored = ignore.IsIgnored(file, pathIsDirectory: false);
                callback(file, isDirectory: false, isIgnored);
            }

            // process directories
            var directories = Directory.GetDirectories(currentDirectory);
            foreach (var directory in directories)
            {
                var isIgnored = ignore.IsIgnored(directory, pathIsDirectory: true);
                callback(directory, isDirectory: true, isIgnored);
                ProcessInternal(ignore, directory, isRecursive, canReadGitIgnores, callback);
            }
        }

#endif

        private string[] ReadRulesFromFile(string path)
        {
            var lines = File.ReadAllLines(path);

            var rules = lines
                .Select(line => line.IndexOf('#') > -1 ? line.Substring(0, line.IndexOf('#')) : line)
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToArray();

            return rules;
        }
    }
}
