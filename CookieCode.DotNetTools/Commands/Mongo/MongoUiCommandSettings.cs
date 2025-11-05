using Spectre.Console;
using Spectre.Console.Cli;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookieCode.DotNetTools.Commands.Mongo
{
    internal class MongoUiCommandSettings : CommandSettings
    {
        [CommandOption("--connection-name <CONNECTION_NAME>")]
        public string? ConnectionName { get; set; }

        [CommandOption("--connection-string <CONNECTION_STRING>")]
        public string? ConnectionString { get; set; }
    }
}
