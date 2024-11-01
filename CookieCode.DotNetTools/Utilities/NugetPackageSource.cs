namespace CookieCode.DotNetTools.Utilities
{
    public class NugetPackageSource
    {
        public required string Name { get; set; }

        public required string Location { get; set; }

        public required int ProtocolVersion { get; set; }
    }
}
