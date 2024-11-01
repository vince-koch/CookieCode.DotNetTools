using System.Linq;
using System.Xml.Linq;

namespace CookieCode.DotNetTools.Utilities
{
    public class CsProject
    {
        // <Project ToolsVersion="15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
        // <Project Sdk="Microsoft.NET.Sdk">

        public string Path { get; }

        private readonly XDocument _document;

        public string Name
        {
            get
            {
                if (IsDotNetCore)
                {
                    var name = System.IO.Path.GetFileNameWithoutExtension(Path);
                    return name;
                }
                else
                {
                    var name = _document.Descendants()
                        .Where(element => element.Name.LocalName == "AssemblyName")
                        .Select(element => element.Value)
                        .Single();

                    return name;
                }
            }
        }

        public bool IsDotNetCore
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Sdk))
                {
                    return true;
                }

                return false;
            }
        }

        public string? Sdk => _document.Root?.Element("Sdk")?.Value;

        public CsProject(string path)
        {
            Path = path;
            _document = XDocument.Load(Path);
        }
    }
}
