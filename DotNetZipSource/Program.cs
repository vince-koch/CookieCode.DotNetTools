using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

using CommandLine;

using MAB.DotIgnore;

namespace DotNetZipSource
{
    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                int exitCode = 0;

                Parser.Default.ParseArguments<ProgramArgs>(args)
                    .WithParsed<ProgramArgs>(options => Process(options))
                    .WithNotParsed(errors => exitCode = -1);

                return exitCode;
            }
            catch (Exception thrown)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(thrown);
                return 1;
            }
            finally
            {
                Console.ResetColor();
            }
        }

        private static void Process(ProgramArgs args)
        {
            var searchDirectory = args.Directory ?? Directory.GetCurrentDirectory();

            var gitIgnorePath = Path.Combine(searchDirectory, ".gitignore");

            var ignoreList = File.Exists(gitIgnorePath)
                ? new IgnoreList(gitIgnorePath)
                : CreateDefaultIgnoreList();

            ignoreList.AddRule(".git/");
            ignoreList.AddRules(args.Rules);
  
            var files = ignoreList.GetFiles(searchDirectory);
            
            var zipFilePath = args.ZipPath ?? searchDirectory + ".zip";
            CreateZipFile(zipFilePath, searchDirectory, files);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success");
        }

        private static void CreateZipFile(string zipPath, string searchDirectory, string[] files)
        {
            if (!files.Any())
            {
                return;
            }

            using (var stream = new FileStream(zipPath, FileMode.Create))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, true))
            {
                foreach (var file in files)
                {
                    var relativePath = Path.GetRelativePath(searchDirectory, file);
                    Console.WriteLine(relativePath);

                    var zipArchiveEntry = archive.CreateEntry(relativePath, CompressionLevel.Fastest);
                    
                    using (var fileStream = File.OpenRead(file))
                    using (var zipStream = zipArchiveEntry.Open())
                    {
                        fileStream.CopyTo(zipStream);
                    }
                }
            }
        }

        private static IgnoreList CreateDefaultIgnoreList()
        {
            var rules = new string[]
            {
                "[Bb]in/",
                "[Oo]bj/",
                "packages/",
                "node_modules/",
            };

            var ignoreList = new IgnoreList(rules);

            return ignoreList;
        }
    }
}