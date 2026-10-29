using System.IO;
using System.Linq;

namespace CookieCode.DotNetTools.Commands.Editor.Services
{
    /// <summary>What the command line asked us to open: a root folder and, optionally, a file in it.</summary>
    public class StartupTarget
    {
        public string RootDirectory { get; set; } = string.Empty;
        public string? FilePath { get; set; }
    }
}
