using System;

namespace CookieCode.DotNetTools
{
    internal class Exit
    {
        public static int Success()
        {
            return 0;
        }

        public static int Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
            return 99;
        }
    }
}
