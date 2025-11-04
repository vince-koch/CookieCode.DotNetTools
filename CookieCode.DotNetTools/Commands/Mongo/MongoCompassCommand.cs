using MongoDB.Driver;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace CookieCode.DotNetTools.Commands.Mongo
{
    /// <summary>
    /// https://hub.docker.com/r/haohanyang/compass-web
    /// https://github.com/haohanyang/compass-web
    /// </summary>
    [Description("Starts an instance of mongo-compass (web)")]
    internal class MongoCompassCommand : AsyncCommand<MongoUiCommandSettings>, IMongoUiCommand
    {
        public string Name => "mongo-compass";

        public override async Task<int> ExecuteAsync(CommandContext context, MongoUiCommandSettings settings)
        {
            await MongoUiCommandSettings.EnsureConnectionStringAsync(settings);
            await StartMongoCompassAsync(settings);
            return 0;
        }

        private static async Task StartMongoCompassAsync(MongoUiCommandSettings settings)
        {
            ArgumentNullException.ThrowIfNull(settings.ConnectionString);

            string mongo_compass_image_name = "haohanyang/compass-web";
            int mongo_compass_container_port = 8080;
            //string? mongo_compass_username = Guid.NewGuid().ToString();
            //string? mongo_compass_password = Guid.NewGuid().ToString();
            string? mongo_compass_username = null;
            string? mongo_compass_password = null;

            var builder = new MongoUrlBuilder(settings.ConnectionString);

            var create = await DockerUtil.CreateContainerAsync(
                imageName: mongo_compass_image_name,
                imagePort: mongo_compass_container_port,
                environment: new Dictionary<string, string?>
                {
                    ["CW_MONGO_URI"] = settings.ConnectionString.Replace("localhost", "host.docker.internal"),
                });

            var inspect = await DockerUtil.StartContainerAsync(create.ID);
            var hostPort = await DockerUtil.GetMappedPortAsync(mongo_compass_container_port, create, inspect);
            await DockerUtil.WaitForPortAsync("localhost", hostPort, timeoutSeconds: 30);
            
            // this is dumb, but we seem to have a timing issue
            await Task.Delay(TimeSpan.FromSeconds(1));

            BrowserUtil.OpenUrl(
                url: $"http://localhost:{hostPort}",
                username: mongo_compass_username,
                password: mongo_compass_password);
        }
    }
}
