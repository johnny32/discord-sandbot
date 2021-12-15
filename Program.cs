using System;
using System.IO;
using System.Threading.Tasks;
using DiscordSandbot.Database;
using DiscordSandbot.Discord;
using DiscordSandbot.HarterQuotes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;

namespace DiscordSandbot
{
    class Program
    {
        public static IServiceProvider Services { get; private set; }
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            Services = serviceCollection.BuildServiceProvider();

            await Services.GetService<App>().RunAsync();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile("appsettings.Local.json", true)
                .Build();

            var configuration = new Configuration();

            ConfigurationBinder.Bind(builder, configuration);

            services.AddSingleton<Configuration>(configuration);

            services.AddSingleton<IDatabaseService, DatabaseService>();
            services.AddSingleton<IDiscordMessageHandler, DiscordMessageHandler>();
            services.AddSingleton<IHarterQuotesService, HarterQuotesService>();
            services.AddSingleton<IHoffmanQuotesService, HoffmanQuotesService>();

            services.AddTransient<App>();

            Logger serilogLogger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder)
                .CreateLogger();

            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddSerilog(serilogLogger, true);
            });
        }
    }
}
