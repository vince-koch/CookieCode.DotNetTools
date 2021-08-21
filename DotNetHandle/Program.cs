using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;

namespace DotNetHandle
{
    public static class Program
    {
        // https://rayanfam.com/topics/reversing-windows-internals-part1/
        public static int Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    throw new ArgumentException("No path was specified");
                }

                var path = args[0];
                if (!Directory.Exists(path) && !File.Exists(path))
                {
                    throw new ArgumentException($"Path [{path}] does not exist");
                }

                var processes = CheckLocks(path);
                while (processes.Any())
                {
                    // confirm before killing
                    Console.WriteLine();

                    var response = Prompt(string.Concat(
                        "Do you want to kill these processes (".DarkYellow(),
                        "R".Yellow() + "efresh".DarkYellow(),
                        " / ".DarkGray(),
                        "Y".Yellow() + "es".DarkYellow(),
                        " / ".DarkGray(),
                        "NO".Yellow(),
                        ")? ".DarkYellow()),
                        ConsoleKey.N,
                        ConsoleKey.Y,
                        ConsoleKey.R);

                    switch (response)
                    {
                        case ConsoleKey.Y:
                            Kill(processes);
                            return 0;

                        case ConsoleKey.N:
                            return 0;

                        case ConsoleKey.R:
                            processes = CheckLocks(path);
                            break;
                    }
                }

                return 0;
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

        private static Process[] CheckLocks(string path)
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

        private static Process[] GetProcesses(int[] pids)
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

        private static int[] ParseProcessIds(string content)
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

        private static string ExecuteHandleExe(string handleExe, string path)
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

        private static void DownloadHandleExe()
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

        private static void Kill(Process[] processes)
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

        private static ConsoleKey Prompt(string message, ConsoleKey defaultKey, params ConsoleKey[] validKeys)
        {
            Console.Write(message);

            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == defaultKey || key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine(defaultKey);
                    return defaultKey;
                }

                if (validKeys.Contains(key.Key))
                {
                    Console.WriteLine(key.Key);
                    return key.Key;
                }

                Console.Beep();
            }
        }
    }
}
