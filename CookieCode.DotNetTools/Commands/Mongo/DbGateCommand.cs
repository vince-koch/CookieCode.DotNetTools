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
    /// https://hub.docker.com/r/dbgate/dbgate
    /// https://github.com/dbgate/dbgate/
    /// https://docs.dbgate.io/
    /// </summary>
    [Description("Starts an instance of DbGate")]
    internal class DbGateCommand : AsyncCommand<MongoUiCommandSettings>, IMongoUiCommand
    {
        private readonly ConnectionStringService _connectionStringService;

        public string Name => "dbgate";

        public DbGateCommand(ConnectionStringService connectionStringService)
        {
            _connectionStringService = connectionStringService;
        }

        public override async Task<int> ExecuteAsync(CommandContext context, MongoUiCommandSettings settings, CancellationToken cancellationToken)
        {
            await _connectionStringService.EnsureConnectionStringAsync("mongo", settings);
            await StartDbGate(settings);
            return 0;
        }

        private static async Task StartDbGate(MongoUiCommandSettings settings)
        {
            ArgumentNullException.ThrowIfNull(settings.ConnectionString);

            string imageName = "dbgate/dbgate:6.7.3-alpine";
            int imagePort = 3000;

            var create = await DockerUtil.CreateContainerAsync(
                imageName: imageName,
                imagePort: imagePort,
                environment: new Dictionary<string, string?>
                {
                    ["CONNECTIONS"] = "con1",
                    ["LABEL_con1"] = "MongoDB",
                    ["ENGINE_con1"] = "mongo@dbgate-plugin-mongo",
                    ["URL_con1"] = settings.ConnectionString.Replace("localhost", "host.docker.internal"),
                });

            var inspect = await DockerUtil.StartContainerAsync(create.ID);
            var hostPort = await DockerUtil.GetMappedPortAsync(imagePort, create, inspect);
            await DockerUtil.WaitForPortAsync("localhost", hostPort, timeoutSeconds: 30);

            BrowserUtil.OpenUrl(url: $"http://localhost:{hostPort}");
        }
    }
}
