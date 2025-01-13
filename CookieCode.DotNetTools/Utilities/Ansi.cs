namespace CookieCode.DotNetTools.Utilities
{
    /// <summary>
    /// Helper class for generating VT100 Ansi escape sequences.
    /// 
    /// http://www.climagic.org/mirrors/VT100_Escape_Codes.html
    /// https://en.wikipedia.org/wiki/ANSI_escape_code
    /// </summary>
    public static class Ansi
	{
		/// <summary>
		/// Whether we should *actually* emit ANSI escape codes or not.
		/// Useful when we want to output to a log file, for example.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;

		// Solution on how to output ANSI escape codes in C# from here:
		// https://www.jerriepelser.com/blog/using-ansi-color-codes-in-net-console-apps
		public static string Reset => IsEnabled ? "\u001b[0m" : "";
		public static string HiCol => IsEnabled ? "\u001b[1m" : "";
		public static string LoCol => IsEnabled ? "\u001b[2m" : "";
		public static string Underline => IsEnabled ? "\u001b[4m" : "";
		public static string Inverse => IsEnabled ? "\u001b[7m" : "";

		public static class Fg
		{
            public static string Black => IsEnabled ? "\u001b[30m" : "";
            public static string Red => IsEnabled ? "\u001b[31m" : "";
            public static string Green => IsEnabled ? "\u001b[32m" : "";
            public static string Yellow => IsEnabled ? "\u001b[33m" : "";
            public static string Blue => IsEnabled ? "\u001b[34m" : "";
            public static string Magenta => IsEnabled ? "\u001b[35m" : "";
            public static string Cyan => IsEnabled ? "\u001b[36m" : "";
            public static string White => IsEnabled ? "\u001b[37m" : "";
        }

		public static class Bg
		{
            public static string Black => IsEnabled ? "\u001b[40m" : "";
            public static string Red => IsEnabled ? "\u001b[41m" : "";
            public static string Green => IsEnabled ? "\u001b[42m" : "";
            public static string Yellow => IsEnabled ? "\u001b[43m" : "";
            public static string Blue => IsEnabled ? "\u001b[44m" : "";
            public static string Magenta => IsEnabled ? "\u001b[45m" : "";
            public static string Cyan => IsEnabled ? "\u001b[46m" : "";
            public static string White => IsEnabled ? "\u001b[47m" : "";
        }

		public static class Cursor
		{
			// Thanks to http://ascii-table.com/ansi-escape-sequences.php for the following ANSI escape sequences
			public static string Up(int lines = 1) => IsEnabled ? $"\u001b[{lines}A" : "";
			public static string Down(int lines = 1) => IsEnabled ? $"\u001b[{lines}B" : "";
			public static string Right(int lines = 1) => IsEnabled ? $"\u001b[{lines}C" : "";
			public static string Left(int lines = 1) => IsEnabled ? $"\u001b[{lines}D" : "";

			//public static string JumpTo(Vector2 pos) => $"\u001b[{pos.Y};{pos.X}H" : "";

			public static class Position
			{
				public static string Save => IsEnabled ? $"\u001b[s" : "";
				public static string Restore => IsEnabled ? $"\u001b[u" : "";
			}
        }

        public static class Clear
		{
			public static string LineRight => IsEnabled ? $"\u001b[0K" : "";
			public static string LineLeft => IsEnabled ? $"\u001b[1K" : "";
			public static string Line => IsEnabled ? $"\u001b[2K" : "";
		}
	}
}
