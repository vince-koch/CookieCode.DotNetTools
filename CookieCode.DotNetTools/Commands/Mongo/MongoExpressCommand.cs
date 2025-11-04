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
        public string Name => "mongo-express";

        public override async Task<int> ExecuteAsync(CommandContext context, MongoUiCommandSettings settings, CancellationToken cancellationToken)
        {
            await MongoUiCommandSettings.EnsureConnectionStringAsync(settings);
            await StartMongoExpress(settings);
            return 0;
        }

        private static async Task StartMongoExpress(MongoUiCommandSettings settings)
        {
            ArgumentNullException.ThrowIfNull(settings.ConnectionString);

            string mongo_express_image_name = "mongo-express";
            int mongo_express_container_port = 8081;
            string? mongo_express_username = Guid.NewGuid().ToString();
            string? mongo_express_password = Guid.NewGuid().ToString();

            var builder = new MongoUrlBuilder(settings.ConnectionString);

            var create = await DockerUtil.CreateContainerAsync(
                imageName: mongo_express_image_name,
                imagePort: mongo_express_container_port,
                environment: new Dictionary<string, string?>
                {
                    ["ME_CONFIG_MONGODB_URL"] = settings.ConnectionString.Replace("localhost", "host.docker.internal"),
                    ["ME_CONFIG_BASICAUTH_USERNAME"] = mongo_express_username,
                    ["ME_CONFIG_BASICAUTH_PASSWORD"] = mongo_express_password,
                });

            var inspect = await DockerUtil.StartContainerAsync(create.ID);
            var hostPort = await DockerUtil.GetMappedPortAsync(mongo_express_container_port, create, inspect);
            await DockerUtil.WaitForPortAsync("localhost", hostPort, timeoutSeconds: 30);

            BrowserUtil.OpenUrl(
                url: $"http://localhost:{hostPort}",
                username: mongo_express_username,
                password: mongo_express_password);
        }
    }
}
