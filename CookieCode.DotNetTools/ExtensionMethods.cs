using System;

namespace CookieCode.DotNetTools
{
    internal static class ExtensionMethods
    {
        public static TValue ThrowIfNull<TValue>(this TValue? value, string? name = null)
		{
            if (value != null)
            {
                return value;
            }

            var valueName = name ?? nameof(value);
            throw new ArgumentNullException(valueName);
        }
	}
}
