using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terminal.Gui;
using Terminal.Gui.Views;

namespace CookieCode.DotNetTools.Commands.Editor.Ui
{
    /// <summary>Lazy file-system tree rooted at a folder: folders first, then files, alphabetical.</summary>
    public sealed class FileTreeView : TreeView<FileSystemInfo>
    {
        private readonly bool _showHidden;

        public FileTreeView(string rootDirectory, bool showHidden)
        {
            _showHidden = showHidden;
            TreeBuilder = new DelegateTreeBuilder<FileSystemInfo>(ListChildren, o => o is DirectoryInfo);
            AspectGetter = o => o.Name;
            Root = new DirectoryInfo(rootDirectory);
            AddObject(Root);
            Expand(Root);
            SelectedObject = Root;
        }

        public DirectoryInfo Root { get; }

        private IEnumerable<FileSystemInfo> ListChildren(FileSystemInfo node)
        {
            if (node is not DirectoryInfo dir)
            {
                return [];
            }

            try
            {
                return dir.EnumerateFileSystemInfos()
                    .Where(i => _showHidden || !i.Attributes.HasFlag(FileAttributes.Hidden) && !i.Name.StartsWith('.'))
                    .OrderByDescending(i => i is DirectoryInfo)
                    .ThenBy(i => i.Name, StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
            {
                return [];
            }
        }
    }
}
