using Docker.DotNet;
using Docker.DotNet.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace CookieCode.DotNetTools.Commands.Mongo
{
    internal class DockerUtil
    {
        private static readonly DockerClient _dockerClient = new DockerClientConfiguration(
            new Uri(Environment.OSVersion.Platform == PlatformID.Win32NT
                ? "npipe://./pipe/docker_engine"
                : "unix:///var/run/docker.sock"))
             .CreateClient();

        public static async Task<CreateContainerResponse> CreateContainerAsync(string imageName, int imagePort, Dictionary<string, string?> environment)
        {
            var environmentVariables = environment
                .Where(pair => !string.IsNullOrWhiteSpace(pair.Value))
                .Select(pair => $"{pair.Key}={pair.Value}")
                .ToArray();

            var create = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
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
            });

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
    }
}
