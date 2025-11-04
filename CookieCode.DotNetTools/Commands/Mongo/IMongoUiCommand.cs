using Spectre.Console.Cli;
using System.Threading;
using System.Threading.Tasks;

namespace CookieCode.DotNetTools.Commands.Mongo
{
    internal interface IMongoUiCommand
    {
        string Name { get; }

        Task<int> ExecuteAsync(CommandContext context, MongoUiCommandSettings settings, CancellationToken cancellationToken);
    }
}
