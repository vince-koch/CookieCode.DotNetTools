using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console.Cli;

namespace CookieCode.DotNetTools.Commands.Mongo
{
    /// <summary>
    /// docker run -p 3000:3000 -e MONGODB_URI=mongodb://mongo:27017 ghcr.io/aientech/mongoman:main
    /// https://github.com/aientech/mongoman/pkgs/container/mongoman
    /// https://aientech.github.io/mongoman/
    /// https://github.com/AienTech/mongoman
    /// </summary>
    [Description("Starts an instance of mongoman")]
    internal class MongoManCommand : AsyncCommand<MongoUiCommandSettings>, IMongoUiCommand
    {
        private readonly ConnectionStringService _connectionStringService;

        public string Name => "mongoman";

        public MongoManCommand(ConnectionStringService connectionStringService)
        {
            _connectionStringService = connectionStringService;
        }

        public override async Task<int> ExecuteAsync(CommandContext context, MongoUiCommandSettings settings, CancellationToken cancellationToken)
        {
            await _connectionStringService.EnsureConnectionStringAsync("mongo", settings);
            await StartMongoMan(settings);
            return 0;
        }

        private static async Task StartMongoMan(MongoUiCommandSettings settings)
        {
            ArgumentNullException.ThrowIfNull(settings.ConnectionString);

            string imageName = "ghcr.io/aientech/mongoman:main";
            int imagePort = 3000;

            var create = await DockerUtil.CreateContainerAsync(
                imageName: imageName,
                imagePort: imagePort,
                environment: new Dictionary<string, string?>
                {
                    ["MONGODB_URI"] = settings.ConnectionString.Replace("localhost", "host.docker.internal"),
                });

            var inspect = await DockerUtil.StartContainerAsync(create.ID);
            var hostPort = await DockerUtil.GetMappedPortAsync(imagePort, create, inspect);
            await DockerUtil.WaitForPortAsync("localhost", hostPort, timeoutSeconds: 30);

            BrowserUtil.OpenUrl(url: $"http://localhost:{hostPort}");
        }
    }
}
