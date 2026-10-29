namespace CookieCode.DotNetTools.Commands.Editor.Services
{
    public interface IFileService
    {
        /// <summary>Returns null when the file does not exist.</summary>
        string? ReadAllText(string path);
        void WriteAllText(string path, string text);
    }
}
