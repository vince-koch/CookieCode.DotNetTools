using Microsoft.Extensions.Logging;
using Terminal.Gui.App;
using Terminal.Gui.Configuration;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using CookieCode.DotNetTools.Commands.Editor.Options;
using CookieCode.DotNetTools.Commands.Editor.Services;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

namespace CookieCode.DotNetTools.Commands.Editor.Ui
{
    public sealed class MainWindow : Window
    {
        private readonly IApplication _app;
        private readonly ILogger _logger;
        private readonly EditorOptions _options;
        private readonly FrameView _sidebar;
        private FileTreeView _tree;
        private readonly FrameView _editorFrame;
        private readonly EditorView _editor;
        private readonly Label _position;
        private readonly Dictionary<Key, Action> _hotkeys;

        public MainWindow(IApplication app, StartupTarget target, IFileService files, EditorOptions options, ILogger logger)
        {
            _app = app;
            _logger = logger;
            _options = options;

            Title = "VEdit";
            BorderStyle = Terminal.Gui.Drawing.LineStyle.None;

            var menu = new MenuBar([
                new MenuBarItem("_File", [
                    Item("_New", Key.N.WithCtrl, NewFile),
                    Item("_Open File...", Key.O.WithCtrl, OpenFileDialog),
                    Item("Open _Folder...", Key.K.WithCtrl, OpenFolderDialog),
                    Item("_Save", Key.S.WithCtrl, () => Save()),
                    Item("Save _As...", null, () => SaveAs()),
                    new Line(),
                    Item("E_xit", Key.Q.WithCtrl, () => _app.RequestStop())
                ]),
                new MenuBarItem("_View", [
                    Item("Toggle _Sidebar", Key.B.WithCtrl, ToggleSidebar),
                    Item("Focus _Explorer", Key.E.WithCtrl, () => _tree!.SetFocus()),
                    Item("Focus E_ditor", Key.D.WithCtrl, () => _editor!.SetFocus()),
                    new Line(),
                    Item("_Theme", Key.T.WithCtrl, CycleTheme)
                ])
            ]);

            _tree = null!;
            _sidebar = new FrameView
            {
                X = 0,
                Y = Pos.Bottom(menu),
                Width = options.SidebarWidth,
                Height = Dim.Fill(1),
                Visible = options.ShowSidebar
            };

            SetRoot(target.RootDirectory);

            _editor = new EditorView(files, options.TabWidth, options.WordWrap)
            {
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            _editor.ModifiedChanged += (_, _) => UpdateTitle();
            _editor.CaretChanged += (_, _) => UpdateStatus();

            _editorFrame = new FrameView
            {
                X = Pos.Right(_sidebar),
                Y = Pos.Bottom(menu),
                Width = Dim.Fill(),
                Height = Dim.Fill(1)
            };
            _editorFrame.Add(_editor);

            _position = new Label { X = Pos.AnchorEnd(24), Y = Pos.AnchorEnd(1), Text = "" };
            var hints = new Label
            {
                X = 0,
                Y = Pos.AnchorEnd(1),
                Text = "Ctrl+O Open  Ctrl+K Folder  Ctrl+S Save  Ctrl+B Sidebar  Ctrl+E Explorer  Ctrl+D Editor  Ctrl+Q Exit"
            };

            _hotkeys = new Dictionary<Key, Action>
            {
                [Key.N.WithCtrl] = NewFile,
                [Key.O.WithCtrl] = OpenFileDialog,
                [Key.K.WithCtrl] = OpenFolderDialog,
                [Key.S.WithCtrl] = () => Save(),
                [Key.B.WithCtrl] = ToggleSidebar,
                [Key.E.WithCtrl] = () => _tree.SetFocus(),
                [Key.D.WithCtrl] = () => _editor.SetFocus(),
                [Key.T.WithCtrl] = CycleTheme,
                [Key.Q.WithCtrl] = () => _app.RequestStop()
            };

            // Handled before views see the key, so the editor/tree cannot swallow shortcuts.
            _app.Keyboard.KeyDown += (_, key) =>
            {
                if (_hotkeys.TryGetValue(key.NoShift.NoAlt, out var action) && key.IsCtrl)
                {
                    key.Handled = true;
                    _logger.LogDebug("Hotkey {Key}", key);
                    action();
                }
            };

            Add(menu, _sidebar, _editorFrame, hints, _position);

            if (target.FilePath is not null)
            {
                OpenFile(target.FilePath);
                _editor.SetFocus();
            }
            else
            {
                UpdateTitle();
                _tree.SetFocus();
            }
        }

        private static MenuItem Item(string title, Key? key, Action action) =>
            new() { Title = title, Key = key ?? Key.Empty, Action = action };

        private void SetRoot(string directory)
        {
            if (_tree is not null)
            {
                _sidebar.Remove(_tree);
                _tree.Dispose();
            }

            _tree = new FileTreeView(directory, _options.ShowHiddenFiles)
            {
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            _tree.Accepted += (_, _) => OnTreeAccepted();
            _sidebar.Title = Path.GetFileName(directory.TrimEnd(Path.DirectorySeparatorChar)) is { Length: > 0 } n ? n : directory;
            _sidebar.Add(_tree);
            _sidebar.Visible = true;
            if (_editorFrame is not null)
            {
                _editorFrame.X = Pos.Right(_sidebar);
            }
        }

        private string? PickPath(string title, OpenMode mode)
        {
            var dialog = new OpenDialog { Title = title, OpenMode = mode };
            _app.Run(dialog);
            var path = dialog.Canceled ? null : dialog.Path;
            dialog.Dispose();
            return string.IsNullOrWhiteSpace(path) ? null : path;
        }

        private void OpenFolderDialog()
        {
            if (PickPath("Open Folder", OpenMode.Directory) is { } dir && Directory.Exists(dir))
            {
                SetRoot(dir);
                _tree.SetFocus();
            }
        }

        private void OpenFileDialog()
        {
            if (PickPath("Open File", OpenMode.File) is { } file && File.Exists(file) && ConfirmDiscard())
            {
                OpenFile(file);
                _editor.SetFocus();
            }
        }

        private void OnTreeAccepted()
        {
            switch (_tree.SelectedObject)
            {
                case FileInfo file:
                    if (ConfirmDiscard())
                    {
                        OpenFile(file.FullName);
                        _editor.SetFocus();
                    }
                    break;
                case DirectoryInfo dir:
                    if (_tree.IsExpanded(dir)) _tree.Collapse(dir); else _tree.Expand(dir);
                    break;
            }
        }

        private void OpenFile(string path)
        {
            try
            {
                _editor.Open(path);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to open {Path}", path);
                MessageBox.ErrorQuery(_app, "Open failed", ex.Message, "OK");
                return;
            }

            UpdateTitle();
        }

        private bool ConfirmDiscard()
        {
            if (!_editor.IsModified)
            {
                return true;
            }

            var choice = MessageBox.Query(_app, "Unsaved changes", $"Save changes to {_editor.DisplayName}?", "Save", "Discard", "Cancel");
            switch (choice)
            {
                case 0:
                    return Save();
                case 1:
                    return true;
                default:
                    return false;
            }
        }

        private void NewFile()
        {
            if (!ConfirmDiscard()) return;
            _editor.New();
            UpdateTitle();
            _editor.SetFocus();
        }

        private bool Save()
        {
            if (_editor.FilePath is null)
            {
                return SaveAs();
            }

            return TrySave(_editor.FilePath);
        }

        private bool SaveAs()
        {
            var dialog = new SaveDialog { Title = "Save As" };
            _app.Run(dialog);
            var path = dialog.Path;
            var canceled = dialog.Canceled;
            dialog.Dispose();

            return !canceled && !string.IsNullOrWhiteSpace(path) && TrySave(path);
        }

        private bool TrySave(string path)
        {
            try
            {
                _editor.Save(path);
                UpdateTitle();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save {Path}", path);
                MessageBox.ErrorQuery(_app, "Save failed", ex.Message, "OK");
                return false;
            }
        }

        private void ToggleSidebar()
        {
            _sidebar.Visible = !_sidebar.Visible;
            _editorFrame.X = _sidebar.Visible ? Pos.Right(_sidebar) : 0;
            if (!_sidebar.Visible) _editor.SetFocus();
        }

        private void CycleTheme()
        {
            var names = ThemeManager.GetThemeNames().ToList();
            var next = names[(names.IndexOf(ThemeManager.GetCurrentThemeName()) + 1) % names.Count];
            ThemeManager.Theme = next;
            _logger.LogInformation("Theme changed to {Theme}", next);
            SetNeedsDraw();
        }

        private void UpdateTitle()
        {
            _editorFrame.Title = _editor.DisplayName + (_editor.IsModified ? " ●" : "");
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            var loc = _editor.Document!.GetLocation(_editor.CaretOffset);
            _position.Text = $"Ln {loc.Line}, Col {loc.Column}";
        }
    }
}
