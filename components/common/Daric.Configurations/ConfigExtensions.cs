using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Daric.Configurations
{
    public static class ConfigExtensions
    {
        /// <summary>
        /// Registers custom config as <see cref="IConfig"/> in DI container
        /// </summary>
        /// <typeparam name="TConfig">The type of config that should be registered</typeparam>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> which the custom config should add to</param>
        /// <param name="options"><see cref="IConfigurationBuilder"/> to add other config types</param>
        /// <returns>The <see cref="IServiceCollection"/> which passed in the first place </returns>
        /// <exception cref="InvalidOperationException">When the "Config" section is not present in the provided configs, like appsettings.json</exception>
        public static IServiceCollection AddConfig<TConfig>(this IServiceCollection serviceCollection,
            Func<IConfigurationBuilder, IConfigurationBuilder>? options) where TConfig : class, IConfig
        {
            return serviceCollection.AddConfig<TConfig>(options, false);
        }

        /// <summary>
        /// Registers custom config as <see cref="IConfig"/> in DI container
        /// </summary>
        /// <typeparam name="TConfig">The type of config that should be registered</typeparam>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> which the custom config should add to</param>
        /// <param name="options"><see cref="IConfigurationBuilder"/> to add other config types</param>
        /// <param name="overrideOptionsPriority">If passed <see cref="bool.TrueString"/>, then add <<paramref name="options"/> at last, otherwise the default configurations will be prioritized</param>
        /// <returns>The <see cref="IServiceCollection"/> which passed in the first place </returns>
        /// <exception cref="InvalidOperationException">When the "Config" section is not present in the provided configs, like appsettings.json</exception>
        public static IServiceCollection AddConfig<TConfig>(this IServiceCollection serviceCollection,
            Func<IConfigurationBuilder, IConfigurationBuilder>? options, bool overrideOptionsPriority) where TConfig : class, IConfig
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            if (!overrideOptionsPriority)
                options?.Invoke(configurationBuilder);

            AddConfigFile(configurationBuilder, "");
            AddConfigFile(configurationBuilder, $".{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");
            AddConfigFile(configurationBuilder, $".{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}");
            configurationBuilder.AddEnvironmentVariables();

            if (overrideOptionsPriority)
                options?.Invoke(configurationBuilder);

            var configuration = configurationBuilder.Build();
            serviceCollection.AddSingleton<IConfiguration>(configuration);
            serviceCollection.Configure<BaseConfig<TConfig>>(configuration);
            serviceCollection.AddScoped<IConfig>(serviceProvider =>
                serviceProvider.GetRequiredService<IOptionsSnapshot<BaseConfig<TConfig>>>().Value?.Config ??
                throw new InvalidOperationException("Can not find Config in appSettings"));
            return serviceCollection;
        }

        /// <summary>
        /// Registers custom config as <see cref="IConfig"/> in DI container
        /// </summary>
        /// <typeparam name="TConfig">The type of config that should be registered</typeparam>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> which the custom config should add to</param>
        /// <returns>The <see cref="IServiceCollection"/> which passed in the first place </returns>
        /// <exception cref="InvalidOperationException">When the "Config" section is not present in the provided configs, like appsettings.json</exception>
        public static IServiceCollection AddConfig<TConfig>(this IServiceCollection serviceCollection)
            where TConfig : class, IConfig
        {
            return serviceCollection.AddConfig<TConfig>(opt => opt);
        }

        /// <summary>
        /// Registers <see cref="IConfig"/> in DI container
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> which the config should add to</param>
        /// <param name="options"><see cref="IConfigurationBuilder"/> to add other config types</param>
        /// <param name="overrideOptionsPriority">If passed <see cref="bool.TrueString"/>, then add <<paramref name="options"/> at last, otherwise the default configurations will be prioritized</param>
        /// <returns>The <see cref="IServiceCollection"/> which passed in the first place </returns>
        /// <exception cref="InvalidOperationException">When the "Config" section is not present in the provided configs, like appsettings.json</exception>
        public static IServiceCollection AddConfig(this IServiceCollection serviceCollection,
                                                   Func<IConfigurationBuilder, IConfigurationBuilder>? options,
                                                   bool overrideOptionsPriority)
        {
            return serviceCollection.AddConfig<Config>(options, overrideOptionsPriority);
        }

        /// <summary>
        /// Registers <see cref="IConfig"/> in DI container
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> which the config should add to</param>
        /// <param name="options"><see cref="IConfigurationBuilder"/> to add other config types</param>
        /// <returns>The <see cref="IServiceCollection"/> which passed in the first place </returns>
        /// <exception cref="InvalidOperationException">When the "Config" section is not present in the provided configs, like appsettings.json</exception>
        public static IServiceCollection AddConfig(this IServiceCollection serviceCollection,
                                                   Func<IConfigurationBuilder, IConfigurationBuilder>? options)
        {
            return serviceCollection.AddConfig(options, false);
        }

        /// <summary>
        /// Registers <see cref="IConfig"/> in DI container
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> which the custom config should add to</param>
        /// <returns>The <see cref="IServiceCollection"/> which passed in the first place </returns>
        /// <exception cref="InvalidOperationException">When the "Config" section is not present in the provided configs, like appsettings.json</exception>
        public static IServiceCollection AddConfig(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddConfig<Config>();
        }

        private static void AddConfigFile(IConfigurationBuilder configurationBuilder, string configFile)
        {
            var appsettingsBaseName = "appsettings{0}.json";
            var appsettingsName = string.Format(appsettingsBaseName, configFile);
            configurationBuilder.AddJsonFile(appsettingsName, true, true);
        }
    }
}