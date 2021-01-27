using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordSandbot
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            await serviceProvider.GetService<App>().RunAsync();
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

            services.AddTransient<App>();
        }
    }
}
