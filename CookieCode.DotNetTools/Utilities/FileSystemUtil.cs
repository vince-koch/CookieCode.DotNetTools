using System.IO;

namespace CookieCode.DotNetTools.Utilities
{
    public static class FileSystemUtil
    {
        public static void Delete(string path)
        {
            var file = new FileInfo(path);
            if (file.Exists)
            {
                file.Attributes = FileAttributes.Normal;
                file.Delete();
                return;
            }

            var directory = new DirectoryInfo(path);
            if (directory.Exists)
            {
                foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
                {
                    info.Attributes = FileAttributes.Normal;
                }

                directory.Attributes = FileAttributes.Normal;
                directory.Delete(true);
            }
        }
    }
}
