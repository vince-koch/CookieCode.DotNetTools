using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using MongoDB.Driver;
using Spectre.Console;

namespace CookieCode.DotNetTools.Commands.Mongo
{
    internal class DockerUtil
    {
        private static readonly DockerClient _dockerClient = new DockerClientConfiguration(
            new Uri(Environment.OSVersion.Platform == PlatformID.Win32NT
                ? "npipe://./pipe/docker_engine"
                : "unix:///var/run/docker.sock"))
             .CreateClient();

        public static async Task<CreateContainerResponse> CreateContainerAsync(
            string imageName,
            int imagePort,
            Dictionary<string, string?> environment,
            Action<CreateContainerParameters>? configure = null)
        {
            var progress = new InlineProgress();

            await _dockerClient.Images.CreateImageAsync(
                new ImagesCreateParameters { FromImage = imageName },
                null,
                //new Progress<JSONMessage>()
                progress);

            progress.Clear();

            var environmentVariables = environment
                .Where(pair => !string.IsNullOrWhiteSpace(pair.Value))
                .Select(pair => $"{pair.Key}={pair.Value}")
                .ToArray();

            var parameters = new CreateContainerParameters
            {
                Image = imageName,
                Env = environmentVariables,
                HostConfig = new HostConfig
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        [$"{imagePort}/tcp"] = new List<PortBinding>
                        {
                            new PortBinding
                            {
                                HostIP = "0.0.0.0",
                                HostPort = null
                            }
                        }
                    },
                    AutoRemove = false,
                    //RestartPolicy = new RestartPolicy { Name = "unless-stopped" }
                },
                ExposedPorts = new Dictionary<string, EmptyStruct>
                {
                    [$"{imagePort}/tcp"] = default
                }
            };

            configure?.Invoke(parameters);

            var create = await _dockerClient.Containers.CreateContainerAsync(parameters);

            return create;
        }

        public static async Task<ContainerInspectResponse> StartContainerAsync(string containerId)
        {
            var inspect = await _dockerClient.Containers.InspectContainerAsync(containerId);
            if (!inspect.State.Running)
            {
                await _dockerClient.Containers.StartContainerAsync(containerId, new ContainerStartParameters());
                inspect = await _dockerClient.Containers.InspectContainerAsync(containerId);
            }

            return inspect;
        }

        public static async Task<int> GetMappedPortAsync(
            int containerPort,
            CreateContainerResponse create,
            ContainerInspectResponse inspect)
        {
            // Try to read mapping from NetworkSettings.Ports first
            int hostPort = 0;
            if (inspect.NetworkSettings?.Ports != null &&
                inspect.NetworkSettings.Ports.TryGetValue($"{containerPort}/tcp", out var bindings) &&
                bindings != null && bindings.Count > 0 &&
                int.TryParse(bindings[0].HostPort, out var parsed))
            {
                hostPort = parsed;
            }
            else
            {
                // Fallback: read from the summary’s Ports list
                var list = await _dockerClient.Containers.ListContainersAsync(new ContainersListParameters { All = true });
                var container = list.FirstOrDefault(c => c.ID.StartsWith(create.ID, StringComparison.OrdinalIgnoreCase));
                var port = container?.Ports?.FirstOrDefault(p => p.PrivatePort == containerPort && p.Type == "tcp");
                if (port?.PublicPort != null)
                {
                    hostPort = (int)port.PublicPort;
                }
            }

            if (hostPort == 0)
            {
                throw new InvalidOperationException("Could not determine mapped host port.");
            }

            return hostPort;
        }

        public static async Task WaitForPortAsync(string host, int port, int timeoutSeconds)
        {
            var start = DateTime.UtcNow;

            while ((DateTime.UtcNow - start).TotalSeconds < timeoutSeconds)
            {
                try
                {
                    using var client = new TcpClient();
                    var connectTask = client.ConnectAsync(host, port);
                    var timeoutTask = Task.Delay(1000);
                    var completed = await Task.WhenAny(connectTask, timeoutTask);

                    if (completed == connectTask && client.Connected)
                        return;
                }
                catch
                {
                    // ignore until timeout
                }

                await Task.Delay(1000);
            }

            throw new TimeoutException($"MongoMan container did not become ready on port {port} within {timeoutSeconds}s.");
        }

        public static int GetNextPort()
        {
            using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));

            int? port = (socket.LocalEndPoint as IPEndPoint)?.Port ?? throw new Exception("Unable to identify an available port");

            return port.Value;
        }

        public class InlineProgress : IProgress<JSONMessage>
        {
            private readonly object _lock = new();
            private bool _hasWritten = false;

            public void Report(JSONMessage value)
            {
                lock (_lock)
                {
                    if (!string.IsNullOrEmpty(value.Status))
                    {
                        string line = value.ProgressMessage != null
                            ? $"{value.Status,-15} {value.ProgressMessage,-30}"
                            : value.Status;

                        string markup =  Markup.Escape(line.PadRight(Console.WindowWidth - 1));

                        AnsiConsole.Markup($"\r[gray]{markup}[/]");

                        _hasWritten = true;
                    }
                }
            }

            public void Clear()
            {
                lock (_lock)
                {
                    if (_hasWritten)
                    {
                        int width = Console.WindowWidth;
                        AnsiConsole.Markup($"\r{string.Empty.PadRight(width - 1)}\r");
                    }
                }
            }
        }
    }
}
