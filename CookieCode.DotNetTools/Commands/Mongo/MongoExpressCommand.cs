using MongoDB.Driver;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace CookieCode.DotNetTools.Commands.Mongo
{
    /// <summary>
    /// https://hub.docker.com/_/mongo-express
    /// https://github.com/mongo-express/mongo-express
    /// </summary>
    [Description("Starts an instance of mongo-express")]
    internal class MongoExpressCommand : AsyncCommand<MongoUiCommandSettings>, IMongoUiCommand
    {
        private readonly ConnectionStringService _connectionStringService;

        public string Name => "mongo-express";

        public MongoExpressCommand(ConnectionStringService connectionStringService)
        {
            _connectionStringService = connectionStringService;
        }

        public override async Task<int> ExecuteAsync(CommandContext context, MongoUiCommandSettings settings, CancellationToken cancellationToken)
        {
            await _connectionStringService.EnsureConnectionStringAsync("mongo", settings);
            await StartMongoExpressAsync(settings);
            return 0;
        }

        private static async Task StartMongoExpressAsync(MongoUiCommandSettings settings)
        {
            ArgumentNullException.ThrowIfNull(settings.ConnectionString);

            string imageName = "mongo-express:1.0.2-18";
            int imagePort = 8081;
            string? imageUser = null;
            string? imagePass = null;

            var builder = new MongoUrlBuilder(settings.ConnectionString);

            var create = await DockerUtil.CreateContainerAsync(
                imageName: imageName,
                imagePort: imagePort,
                environment: new Dictionary<string, string?>
                {
                    ["ME_CONFIG_MONGODB_URL"] = settings.ConnectionString.Replace("localhost", "host.docker.internal"),
                    ["ME_CONFIG_BASICAUTH"] = false.ToString(),
                    ["ME_CONFIG_BASICAUTH_USERNAME"] = imageUser,
                    ["ME_CONFIG_BASICAUTH_PASSWORD"] = imagePass,
                    ["ME_CONFIG_MONGODB_SSLVALIDATE"] = "false",
                });

            var inspect = await DockerUtil.StartContainerAsync(create.ID);
            var hostPort = await DockerUtil.GetMappedPortAsync(imagePort, create, inspect);
            await DockerUtil.WaitForPortAsync("localhost", hostPort, timeoutSeconds: 30);

            BrowserUtil.OpenUrl(
                url: $"http://localhost:{hostPort}",
                username: imageUser,
                password: imagePass);
        }
    }
}
