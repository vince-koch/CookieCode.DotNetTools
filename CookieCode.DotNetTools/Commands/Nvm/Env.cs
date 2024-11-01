using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CookieCode.DotNetTools.Commands.Nvm
{
    internal class Env
    {
        public static Env Instance = new Env();

        public string? NVM_HOME
        {
            get
            {
                var value = Environment.GetEnvironmentVariable(nameof(NVM_HOME), EnvironmentVariableTarget.User);
                return value;
            }

            set
            {
                if (!Directory.Exists(value))
                {
                    throw new DirectoryNotFoundException($"Can not set {nameof(NVM_HOME)} because folder [{value}] does not exist");
                }

                Environment.SetEnvironmentVariable(nameof(NVM_HOME), value, EnvironmentVariableTarget.User);
            }
        }

        public string? NVM_CURRENT
        {
            get
            {
                var value = Environment.GetEnvironmentVariable(nameof(NVM_CURRENT), EnvironmentVariableTarget.User);
                return value;
            }

            set
            {
                if (!Directory.Exists(value))
                {
                    throw new DirectoryNotFoundException($"Can not set {nameof(NVM_CURRENT)} because folder [{value}] does not exist");
                }

                Environment.SetEnvironmentVariable(nameof(NVM_CURRENT), value, EnvironmentVariableTarget.User);
            }
        }

        public void ChangeNodePath(NodeInstallation installation)
        {
            RemoveNodePath();

            var paths = GetPaths();
            paths.Add(installation.Folder);

            SetPaths(paths);

            NVM_CURRENT = installation.Folder;
        }

        public void RemoveNodePath()
        {
            var paths = GetPaths();
            paths.RemoveAll(p => p.StartsWith(NVM_HOME ?? string.Empty));
            SetPaths(paths);
        }

        private List<string> GetPaths()
        {
            var path = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User) ?? string.Empty;
            var paths = path.Split(Path.PathSeparator).ToList();
            return paths;
        }

        private void SetPaths(IEnumerable<string> paths)
        {
            var path = string.Join(Path.PathSeparator, paths);
            Environment.SetEnvironmentVariable("PATH", path, EnvironmentVariableTarget.User);
        }
    }
}
