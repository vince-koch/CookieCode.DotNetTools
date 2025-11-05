using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using MongoDB.Driver;
using Spectre.Console.Cli;

namespace CookieCode.DotNetTools.Commands.Mongo
{
    /// <summary>
    /// https://hub.docker.com/r/huggingface/mongoku
    /// https://github.com/huggingface/Mongoku
    /// </summary>
    [Description("Starts an instance of mongoku")]
    internal class MongokuCommand : AsyncCommand<MongoUiCommandSettings>, IMongoUiCommand
    {
        private readonly ConnectionStringService _connectionStringService;

        public string Name => "mongoku";

        public MongokuCommand(ConnectionStringService connectionStringService)
        {
            _connectionStringService = connectionStringService;
        }

        public override async Task<int> ExecuteAsync(CommandContext context, MongoUiCommandSettings settings, CancellationToken cancellationToken)
        {
            await _connectionStringService.EnsureConnectionStringAsync("mongo", settings);
            await StartMongokuAsync(settings);
            return 0;
        }

        private static async Task StartMongokuAsync(MongoUiCommandSettings settings)
        {
            ArgumentNullException.ThrowIfNull(settings.ConnectionString);

            string mongoku_image_name = "huggingface/mongoku";
            int mongoku_container_port = 3100;
            string? mongoku_username = Guid.NewGuid().ToString();
            string? mongoku_password = Guid.NewGuid().ToString();

            var builder = new MongoUrlBuilder(settings.ConnectionString);

            var hostPort = DockerUtil.GetNextPort();

            var create = await DockerUtil.CreateContainerAsync(
                imageName: mongoku_image_name,
                imagePort: mongoku_container_port,
                environment: new Dictionary<string, string?>
                {
                    ["MONGOKU_DEFAULT_HOST"] = settings.ConnectionString.Replace("localhost", "host.docker.internal"),
                    ["MONGOKU_SERVER_ORIGIN"] = $"http://localhost:{hostPort}",
                    //["MONGOKU_AUTH_BASIC"] = $"{mongoku_username}:{mongoku_password}",
                },
                parameters =>
                {
                    var key = $"{mongoku_container_port}/tcp";
                    parameters.HostConfig.PortBindings[key].First().HostPort = hostPort.ToString();
                });

            var inspect = await DockerUtil.StartContainerAsync(create.ID);
            //var hostPortCheck = await DockerUtil.GetMappedPortAsync(mongoku_container_port, create, inspect);
            await DockerUtil.WaitForPortAsync("localhost", hostPort, timeoutSeconds: 30);

            BrowserUtil.OpenUrl(
                url: $"http://localhost:{hostPort}",
                username: mongoku_username,
                password: mongoku_password);
        }
    }
}
