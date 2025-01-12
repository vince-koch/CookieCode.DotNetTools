using System;

namespace CookieCode.DotNetTools
{
    public static partial class ExtensionMethods
    {
        public static T ThrowIfNull<T>(this T? value, string? paramName = null) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName ?? nameof(value), "Value cannot be null.");
            }

            return value;
        }
    }
}
