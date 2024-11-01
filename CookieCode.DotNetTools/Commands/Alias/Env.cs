using System;
using System.IO;

namespace CookieCode.DotNetTools.Commands.Alias
{
    internal class Env
    {
        public static Env Instance = new Env();

        public string? ALIAS_HOME
        {
            get
            {
                var value = Environment.GetEnvironmentVariable(nameof(ALIAS_HOME), EnvironmentVariableTarget.User);
                return value;
            }

            set
            {
                if (!Directory.Exists(value))
                {
                    throw new DirectoryNotFoundException($"Can not set {nameof(ALIAS_HOME)} because folder [{value}] does not exist");
                }

                Environment.SetEnvironmentVariable(nameof(ALIAS_HOME), value, EnvironmentVariableTarget.User);
            }
        }
    }
}
