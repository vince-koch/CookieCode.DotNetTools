namespace DotNetHandle
{
    public static partial class ExtensionMethods
    {
        public static string DarkGray(this string text)
        {
            return $"{Terminal.DarkGray}{text}{Terminal.Reset}";
        }

        public static string DarkYellow(this string text)
        {
            return $"{Terminal.DarkYellow}{text}{Terminal.Reset}";
        }

        public static string White(this string text)
        {
            return $"{Terminal.White}{text}{Terminal.Reset}";
        }

        public static string Yellow(this string text)
        {
            return $"{Terminal.Yellow}{text}{Terminal.Reset}";
        }
    }
}
