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
    /// https://hub.docker.com/r/huggingface/mongoku
    /// https://github.com/huggingface/Mongoku
    /// </summary>
    [Description("Starts an instance of mongoku")]
    internal class MongokuCommand : AsyncCommand<MongoUiCommandSettings>, IMongoUiCommand
    {
        public string Name => "mongoku";

        public override async Task<int> ExecuteAsync(CommandContext context, MongoUiCommandSettings settings, CancellationToken cancellationToken)
        {
            await MongoUiCommandSettings.EnsureConnectionStringAsync(settings);
            await StartMongoExpress(settings);
            return 0;
        }

        private static async Task StartMongoExpress(MongoUiCommandSettings settings)
        {
            ArgumentNullException.ThrowIfNull(settings.ConnectionString);

            string mongoku_image_name = "huggingface/mongoku";
            int mongoku_container_port = 3100;
            string? mongoku_username = Guid.NewGuid().ToString();
            string? mongoku_password = Guid.NewGuid().ToString();

            var builder = new MongoUrlBuilder(settings.ConnectionString);

            var create = await DockerUtil.CreateContainerAsync(
                imageName: mongoku_image_name,
                imagePort: mongoku_container_port,
                environment: new Dictionary<string, string?>
                {
                    ["MONGOKU_DEFAULT_HOST"] = settings.ConnectionString.Replace("localhost", "host.docker.internal"),
                    //["MONGOKU_AUTH_BASIC"] = $"{mongoku_username}:{mongoku_password}",
                });

            var inspect = await DockerUtil.StartContainerAsync(create.ID);
            var hostPort = await DockerUtil.GetMappedPortAsync(mongoku_container_port, create, inspect);
            await DockerUtil.WaitForPortAsync("localhost", hostPort, timeoutSeconds: 30);

            BrowserUtil.OpenUrl(
                url: $"http://localhost:{hostPort}",
                username: mongoku_username,
                password: mongoku_password);
        }
    }
}
