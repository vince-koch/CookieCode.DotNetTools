using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.Metrics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;

namespace CookieCode.DotNetTools
{
    public class SpectreApplicationBuilder : IHostApplicationBuilder
    {
        private readonly IHostApplicationBuilder _internal;
        private readonly SpectreTypeRegistrar _typeRegistrar;
        private readonly CommandApp _commandApp;

        public SpectreTypeRegistrar TypeRegistrar => _typeRegistrar;

        public CommandApp CommandApp => _commandApp;

        #region Construction

        /// <summary>
        /// Performs the creation of a SpectreApplicationBuilder from an existing (non-built) builder.
        /// Essentially permits bring-your-own-builder scenarios, primarily for WebApplicationBuilder, but could be used for any IHostApplicationBuilder.
        /// Note that it should be possible to get the built IHost (WebApplication) via DI....
        /// WebApplication app = (serviceProvider.GetRequiredService<IHost>() as WebApplication).ThrowIfNull();
        /// </summary>
        public static SpectreApplicationBuilder CreateFrom<TBuilder, THost>(TBuilder builder, Func<TBuilder, THost> build)
            where TBuilder : IHostApplicationBuilder
            where THost : IHost
        {
            // wrap the build func they gave us with an attempt to register the resulting application/host as a singleton
            THost TryRegisterAndBuild()
            {
                // attempt to register the application as a singleton so that it can be resolved via DI in Spectre commands, etc.
                // particularly of use in WebApplicationBuilder when you want to use the resulting WebApplication for configuration
                // prior to invoking RunAsync
                THost? application = default;
                var applicationType = typeof(THost);
                builder.Services.TryAddSingleton(applicationType, sp => application.ThrowIfNull());

                // build the application/host and return it
                application = build(builder);
                return application;
            }

            var applicationBuilder = new SpectreApplicationBuilder(builder, b => TryRegisterAndBuild());
            return applicationBuilder;
        }

        /// <summary>
        /// Creates a SpectreApplicationBuilder using <see cref="Host.CreateApplicationBuilder()"/> to create the underlying builder."/>
        /// </summary>
        public SpectreApplicationBuilder()
            : this(Host.CreateApplicationBuilder())
        {
        }

        /// <summary>
        /// Creates a SpectreApplicationBuilder using the specified <see cref="HostApplicationBuilder"/> as the underlying builder."/>
        /// </summary>
        /// <param name="hostApplicationBuilder"></param>
        public SpectreApplicationBuilder(HostApplicationBuilder hostApplicationBuilder)
            : this(hostApplicationBuilder, b => hostApplicationBuilder.Build())
        {
        }

        /// <summary>
        /// This is the main constructor - all the others eventually filter into here
        /// Requires a host builder and a build function
        /// </summary>
        private SpectreApplicationBuilder(IHostApplicationBuilder hostBuilder, Func<IHostApplicationBuilder, IHost> build)
        {
            // set this as early as possible, hopefully before any writes have occurred
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            _internal = hostBuilder;
            _typeRegistrar = new SpectreTypeRegistrar(_internal, build);
            _commandApp = new CommandApp(_typeRegistrar);

            //_commandApp.SetDefaultCommand<MenuCommand>();
            _commandApp.Configure(config =>
            {
                config.PropagateExceptions();
            });
        }

        #endregion

        public IDictionary<object, object> Properties => throw new NotImplementedException();

        public IConfigurationManager Configuration => _internal.Configuration;

        public IHostEnvironment Environment => _internal.Environment;

        public ILoggingBuilder Logging => _internal.Logging;

        public IMetricsBuilder Metrics => _internal.Metrics;

        public IServiceCollection Services => _internal.Services;

        public void ConfigureContainer<TContainerBuilder>(
            IServiceProviderFactory<TContainerBuilder> factory,
            Action<TContainerBuilder>? configure = null)
            where TContainerBuilder : notnull
        {
            throw new NotImplementedException();
        }

        public ICommandApp Build()
        {
            // note: try won't register if an ISpectrePromptService is already registered, which
            // allows for custom implementations to be registered prior to Build and used in MenuCommand, etc.

            //_internal.Services.TryAddScoped<ISpectrePromptService, SpectrePromptService>();

            return _commandApp;
        }
    }
}
