using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Spectre.Console.Cli;
using System;

namespace CookieCode.DotNetTools
{
    public class SpectreTypeRegistrar : ITypeRegistrar
    {
        private readonly IHostApplicationBuilder _builder;
        private readonly Func<IHostApplicationBuilder, IHost> _build;

        public IHost? Host { get; private set; }

        internal SpectreTypeRegistrar(
            IHostApplicationBuilder builder,
            Func<IHostApplicationBuilder, IHost> build)
        {
            _builder = builder;
            _build = build;
        }

        public ITypeResolver Build()
        {
            if (Host == null)
            {
                _builder.ConfigureContainer(new DefaultServiceProviderFactory(new ServiceProviderOptions
                {
                    ValidateOnBuild = false,
                    ValidateScopes = true,  // keep scope validation at least
                }));

                Host = _build(_builder);
            }

            var scope = Host.Services.CreateAsyncScope();
            var typeResolver = new SpectreTypeResolver(scope);
            return typeResolver;
        }

        public void Register(Type service, Type implementation)
        {
            _builder.Services.TryAddScoped(service, implementation);
        }

        public void RegisterInstance(Type service, object implementation)
        {
            _builder.Services.TryAddScoped(service, sp => implementation);
        }

        public void RegisterLazy(Type service, Func<object> factory)
        {
            _builder.Services.TryAddScoped(service, sp => factory());
        }
    }
}
