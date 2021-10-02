using System;

namespace CookieCode.DotNetTools.Utilities
{
    public static class AnsiUtil
    {
        public static string UserColor = Ansi.FGreen;

        public static bool Confirm(string confirmText, bool? defaultValue = null)
        {
            Console.Write(confirmText);

            while (true)
            {
                var key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.Y:
                        Console.WriteLine($"{UserColor}Y{Ansi.Reset}");
                        return true;

                    case ConsoleKey.N:
                        Console.WriteLine($"{UserColor}N{Ansi.Reset}");
                        return false;

                    case ConsoleKey.Enter:
                        if (defaultValue.HasValue)
                        {
                            var yesOrNo = defaultValue.Value ? "Y" : "N";
                            Console.WriteLine($"{UserColor}{yesOrNo}{Ansi.Reset}");
                            return defaultValue.Value;
                        }
                        else
                        {
                            Console.Beep();
                        }
                        break;

                    default:
                        Console.Beep();
                        break;
                }
            }
        }

        public static void WriteShortPathProgress(string path)
        {
            var shortPath = PathUtil.GetShortPath(path);
            WriteProgress(shortPath);
        }

        public static void WriteProgress(string text)
        {
            Console.WriteLine($"\r{text}{Ansi.ClearLineRight}");
        }
    }
}
