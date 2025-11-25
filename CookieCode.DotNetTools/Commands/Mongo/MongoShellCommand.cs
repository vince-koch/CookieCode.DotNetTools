using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CookieCode.DotNetTools.Commands.Mongo
{
    [Description("Starts an instance of mongosh")]
    internal class MongoShellCommand : AsyncCommand<MongoUiCommandSettings>, IMongoUiCommand
    {
        public string Name => "mongosh";

        private readonly ConnectionStringService _connectionStringService;

        public MongoShellCommand(ConnectionStringService connectionStringService)
        {
            _connectionStringService = connectionStringService;
        }

        public override async Task<int> ExecuteAsync(CommandContext context, MongoUiCommandSettings settings, CancellationToken cancellationToken)
        {
            await _connectionStringService.EnsureConnectionStringAsync("mongo", settings);
            await StartMongosh(settings);
            return 0;
        }

        private async Task StartMongosh(MongoUiCommandSettings settings)
        {
            string imageName = "mongo:8.0";

            // create the container
            var create = await DockerUtil.Client.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = imageName,
                Cmd = new[] { "sleep", "infinity" }, // idle container to exec into
                Tty = true,
                OpenStdin = true,
                AttachStdout = true,
                AttachStderr = true,
                AttachStdin = true,
                HostConfig = new HostConfig
                {
                    AutoRemove = true,
                },
            });

            await InvokeMongoshAsync(create, settings.ConnectionString.ThrowIfNull().Replace("localhost", "host.docker.internal"));

            await DockerUtil.Client.Containers.RemoveContainerAsync(
                create.ID,
                new ContainerRemoveParameters { Force = true });
        }

        private async Task InvokeMongoshAsync(CreateContainerResponse create, string connectionString)
        {
            await DockerUtil.Client.Containers.StartContainerAsync(create.ID, null);

            var psi = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = $"exec -it {create.ID} mongosh {connectionString}",
                UseShellExecute = true,
                RedirectStandardInput = false,
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                CreateNoWindow = true
            };

            AnsiConsole.MarkupLine($"Launching [green]mongosh[/] attached to container [yellow]{create.ID}[/]");
            var process = Process.Start(psi);
            ArgumentNullException.ThrowIfNull(process, nameof(process));

            await process.WaitForExitAsync();
            var exitColor = process.ExitCode == 0 ? "green" : "red";
            AnsiConsole.MarkupLine($"[green]mongosh[/] exited with code [{exitColor}]{process.ExitCode}[/]");
        }
    }
}
