using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using System;
using System.Threading.Tasks;

namespace CookieCode.DotNetTools
{
    internal class SpectreTypeResolver : ITypeResolver, IDisposable, IAsyncDisposable
    {
        private readonly AsyncServiceScope _scope;

        public SpectreTypeResolver(AsyncServiceScope scope)
        {
            _scope = scope;
        }

        public void Dispose()
        {
            _scope.Dispose();
        }

        public ValueTask DisposeAsync()
        {
            return _scope.DisposeAsync();
        }

        public object? Resolve(Type? type)
        {
            if (type == null)
            {
                return null;
            }

            var value = _scope.ServiceProvider.GetService(type);

            return value;
        }
    }
}
