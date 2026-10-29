namespace CookieCode.DotNetTools.Commands.Editor.Options
{
    public sealed class EditorOptions
    {
        public const string SectionName = "Editor";

        public int SidebarWidth { get; set; } = 32;
        public bool ShowSidebar { get; set; } = true;
        public int TabWidth { get; set; } = 4;
        public bool WordWrap { get; set; }
        public bool ShowHiddenFiles { get; set; }
        public string? Theme { get; set; }
    }
}
