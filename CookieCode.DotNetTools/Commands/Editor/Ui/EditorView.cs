using Terminal.Gui.Editor;
using Terminal.Gui.Editor.Highlighting;
using CookieCode.DotNetTools.Commands.Editor.Services;
using System;
using System.IO;

namespace CookieCode.DotNetTools.Commands.Editor.Ui
{
    /// <summary>A text editor bound to at most one file on disk.</summary>
    public sealed class EditorView : Terminal.Gui.Editor.Editor
    {
        private readonly IFileService _files;

        public EditorView(IFileService files, int tabWidth, bool wordWrap)
        {
            _files = files;
            IndentationSize = tabWidth;
            WordWrap = wordWrap;
            GutterOptions = GutterOptions.LineNumbers | GutterOptions.Folding;
        }

        public string? FilePath { get; private set; }

        public string DisplayName => FilePath is null ? "Untitled" : Path.GetFileName(FilePath);

        public void Open(string path)
        {
            Load(path, _files.ReadAllText(path) ?? "");
        }

        public void New() => Load(null, "");

        public void Save(string? path = null)
        {
            path ??= FilePath ?? throw new InvalidOperationException("No file path.");
            _files.WriteAllText(path, Text);
            if (!string.Equals(path, FilePath, StringComparison.OrdinalIgnoreCase))
            {
                ApplyHighlighting(path);
            }

            FilePath = path;
            Document!.UndoStack.MarkAsOriginalFile();
        }

        private void Load(string? path, string text)
        {
            FilePath = path;
            Text = text;
            CaretOffset = 0;
            Document!.UndoStack.ClearAll();
            Document!.UndoStack.MarkAsOriginalFile();
            ApplyHighlighting(path);
        }

        private void ApplyHighlighting(string? path)
        {
            HighlightingDefinition = path is null
                ? null
                : HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(path));
        }
    }
}
