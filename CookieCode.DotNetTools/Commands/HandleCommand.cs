using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;

using CommandLine;

using CookieCode.DotNetTools.Utilities;

namespace CookieCode.DotNetTools.Commands
{
    [Verb("handle", HelpText = "A tool to identify which process has a file or directory handle locked, and optionally kill the process.  Note that this tool works by downloading Microsoft SysInternals handle.exe and parsing it's output.")]
    public class HandleCommand : ICommand
    {
        [Value(0, HelpText = "File or directory to look for")]
        public string FileOrDirectory { get; set; }

        public void Execute()
        {
            var processes = CheckLocks(FileOrDirectory);
            while (processes.Any())
            {
                Console.WriteLine();

                var choice = AnsiUtil.Ask(
                    "Do you want to kill these processes? [No], [r]efresh, [y]es ",
                    ConsoleKey.Y,
                    ConsoleKey.N,
                    ConsoleKey.R,
                    ConsoleKey.Enter);

                switch (choice)
                {
                    case ConsoleKey.Y:
                        Kill(processes);
                        break;

                    case ConsoleKey.R:
                        processes = CheckLocks(FileOrDirectory);
                        break;

                    default:
                        return;
                }
            }
        }

        private Process[] CheckLocks(string path)
        {
            var handleExe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "handle.exe");
            if (!File.Exists(handleExe))
            {
                DownloadHandleExe();
            }

            var content = ExecuteHandleExe(handleExe, path);
            var pids = ParseProcessIds(content);
            var processes = GetProcesses(pids);

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{processes.Length} processes are holding the requested handle");
            Console.ResetColor();

            foreach (var process in processes)
            {
                Console.WriteLine(
                    "{0,5}   {1}",
                    process.Id,
                    !process.HasExited ? process.ProcessName : "*** exited ***");
            }

            return processes;
        }

        private Process[] GetProcesses(int[] pids)
        {
            List<Process> list = new List<Process>();

            foreach (var pid in pids)
            {
                try
                {
                    var process = Process.GetProcessById(pid);
                    list.Add(process);
                }
                catch (ArgumentException thrown)
                {
                    if (thrown.Message != $"Process with an Id of {pid} is not running.")
                    {
                        throw;
                    }
                }
            }

            return list.ToArray();
        }

        private int[] ParseProcessIds(string content)
        {
            const string PidToken = " pid: ";
            const string TypeToken = " type: ";

            var pids = content.Split(Environment.NewLine)
                .Where(line => line.IndexOf(PidToken) > -1)
                .Select(line =>
                {
                    var start = line.IndexOf(PidToken) + PidToken.Length;
                    var end = line.IndexOf(TypeToken, start);
                    var value = line.Substring(start, end - start);
                    var pid = int.Parse(value);
                    return pid;
                })
                .Distinct()
                .ToArray();

            return pids;
        }

        private string ExecuteHandleExe(string handleExe, string path)
        {
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = handleExe;
            startInfo.ArgumentList.Add(path);
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;

            using (var process = Process.Start(startInfo))
            {
                process.WaitForExit();

                var content = process.StandardOutput.ReadToEnd();
                Debug.WriteLine(content);

                return content;
            }
        }

        private void DownloadHandleExe()
        {
            using (var client = new WebClient())
            {
                var handleZip = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "handle.zip");

                Console.WriteLine("Downloading sysinternals handle.exe");
                client.DownloadFile("https://download.sysinternals.com/files/Handle.zip", handleZip);

                Console.WriteLine("Extracing sysinternals handle.exe");
                ZipFile.ExtractToDirectory(handleZip, AppDomain.CurrentDomain.BaseDirectory);

                File.Delete(handleZip);
            }
        }

        private void Kill(Process[] processes)
        {
            // kill any process that has not already exited
            foreach (var process in processes)
            {
                if (!process.HasExited)
                {
                    process.Kill(false);
                }
            }

            // if we just killed off explorer then restart it
            if (processes.Any(p => string.Equals(
                Path.GetFileName(p.MainModule.FileName),
                "explorer.exe",
                StringComparison.OrdinalIgnoreCase)))
            {
                Process.Start("explorer.exe");
            }
        }
    }
}
