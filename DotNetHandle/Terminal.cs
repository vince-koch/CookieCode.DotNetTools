namespace DotNetHandle
{
    public static class Terminal
    {
        // foreground colors
        public const string Black = "\u001b[30m";
        public const string Blue = "\u001b[34;1m";
        public const string Cyan = "\u001b[36;1m";
        public const string DarkBlue = "\u001b[34m";
        public const string DarkCyan = "\u001b[36m";
        public const string DarkGray = "\u001b[30;1m";
        public const string DarkGreen = "\u001b[32m";
        public const string DarkMagenta = "\u001b[35m";
        public const string DarkRed = "\u001b[31m";
        public const string DarkYellow = "\u001b[33m";
        public const string Gray = "\u001b[37m";
        public const string Green = "\u001b[32;1m";
        public const string Magenta = "\u001b[35;1m";
        public const string Red = "\u001b[31;1m";
        public const string White = "\u001b[37;1m";
        public const string Yellow = "\u001b[33;1m";

        // decorations
        public const string Bold = "\u001b[1m";
        public const string Underline = "\u001b[4m";
        public const string Reversed = "\u001b[7m";

        public static class Cursor
        {
            // cursor navigation
            public const string Up = "\u001b[{n}A";
            public const string Down = "\u001b[{n}B";
            public const string Right = "\u001b[{n}C";
            public const string Left = "\u001b[{n}D";
            public const string SavePosition = "\u001b[{s}";
            public const string RestorePosition = "\u001b[{u}";
            // Up: \u001b[{n}A moves cursor up by n
            // Down: \u001b[{n}B moves cursor down by n
            // Right: \u001b[{n}C moves cursor right by n
            // Left: \u001b[{n}D moves cursor left by n
            // Next Line: \u001b[{n}E moves cursor to beginning of line n lines down
            // Prev Line: \u001b[{n}F moves cursor to beginning of line n lines down
            // Set Column: \u001b[{n}G moves cursor to column n
            // Set Position: \u001b[{n};{m}H moves cursor to row n column m
        }

        // clear screen/line
        public const string ClearScreenAfterCursor = "\u001b[{0}J";
        public const string ClearScreenBeforeCursor = "\u001b[{1}J";
        public const string ClearScreen = "\u001b[{2}J";
        public const string ClearLineAfterCursor = "\u001b[{0}K";
        public const string ClearLineBeforeCursor = "\u001b[{1}K";
        public const string ClearLine = "\u001b[{2}K";

        // reset
        public const string Reset = "\u001b[0m";
    }
}