using System;
using System.Collections.Generic;
using System.Linq;

namespace CookieCode.DotNetTools
{
    public static class ExceptionExtensions
    {
        public static T ThrowIfNull<T>(this T? argument)
        {
            ArgumentNullException.ThrowIfNull(argument);
            T result = argument;
            return result;
        }

        public static T ThrowIfNull<T>(this T? argument, string paramName)
        {
            ArgumentNullException.ThrowIfNull(argument, paramName);
            T result = argument;
            return result;
        }

        public static string ThrowIfNullOrEmpty(string? argument)
        {
            ArgumentException.ThrowIfNullOrEmpty(argument);
            string result = argument;
            return result;
        }

        public static string ThrowIfNullOrEmpty(string? argument, string paramName)
        {
            ArgumentException.ThrowIfNullOrEmpty(argument, paramName);
            string result = argument;
            return result;
        }

        public static string ThrowIfNullOrWhiteSpace(string? argument)
        {

            ArgumentException.ThrowIfNullOrWhiteSpace(argument);
            string result = argument;
            return result;
        }

        public static string ThrowIfNullOrWhiteSpace(string? argument, string paramName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(argument, paramName);
            string result = argument;
            return result;
        }

        public static IEnumerable<T> SingleOrThrow<T>(IEnumerable<T> argument)
        {
            var count = argument.Count();
            if (count != 1)
            {
                throw new ArgumentOutOfRangeException(null, count, $"Expected exactly 1 element, but found {count}");
            }

            return argument;
        }

        public static IEnumerable<T> SingleOrThrow<T>(IEnumerable<T> argument, string paramName)
        {
            var count = argument.Count();
            if (count != 1)
            {
                throw new ArgumentOutOfRangeException(paramName, count, $"Expected exactly 1 element for ${paramName}, but found {count}");
            }

            return argument;
        }
    }
}
