using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console.Cli;

namespace CookieCode.DotNetTools.Commands.Mongo
{
    /// <summary>
    /// https://hub.docker.com/r/ugleiton/mongo-gui
    /// https://github.com/arunbandari/mongo-gui
    /// </summary>
    [Description("Starts an instance of mongo-gui")]
    internal class MongoGuiCommand : AsyncCommand<MongoUiCommandSettings>, IMongoUiCommand
    {
        private readonly ConnectionStringService _connectionStringService;

        public string Name => "mongo-gui";

        public MongoGuiCommand(ConnectionStringService connectionStringService)
        {
            _connectionStringService = connectionStringService;
        }

        public override async Task<int> ExecuteAsync(CommandContext context, MongoUiCommandSettings settings, CancellationToken cancellationToken)
        {
            await _connectionStringService.EnsureConnectionStringAsync("mongo", settings);
            await StartMongoGui(settings);
            return 0;
        }

        private static async Task StartMongoGui(MongoUiCommandSettings settings)
        {
            ArgumentNullException.ThrowIfNull(settings.ConnectionString);

            string imageName = "ugleiton/mongo-gui@sha256:b3f3bad95921b55d4ada02bed765e55b09c9fe65882200b6e8da6dc3ec4c5043";
            int imagePort = 4321;

            var create = await DockerUtil.CreateContainerAsync(
                imageName: imageName,
                imagePort: imagePort,
                environment: new Dictionary<string, string?>
                {
                    ["MONGO_URL"] = settings.ConnectionString.Replace("localhost", "host.docker.internal"),
                });

            var inspect = await DockerUtil.StartContainerAsync(create.ID);
            var hostPort = await DockerUtil.GetMappedPortAsync(imagePort, create, inspect);
            await DockerUtil.WaitForPortAsync("localhost", hostPort, timeoutSeconds: 30);

            BrowserUtil.OpenUrl(url: $"http://localhost:{hostPort}");
        }
    }
}
