using System;
using System.IO;

namespace CookieCode.DotNetTools.Commands.Nvm
{
    internal record NodeInstallation(string ExePath, string Version)
    {
        public string Folder => Path.GetDirectoryName(ExePath) ?? throw new NullReferenceException();
    }
}
