using System;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Logging;

namespace DiscordSandbot.HarterQuotes
{
    public class HarterQuotesService : IHarterQuotesService
    {
        private readonly ILogger _logger;
        private readonly Configuration _configuration;
        private readonly Random _random;

        public HarterQuotesService(ILogger<HarterQuotesService> logger, Configuration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _random = new Random();
        }

        public async Task GetRandomQuoteAsync(CommandContext context)
        {
            string[] files = Directory.GetFiles(_configuration.HarterQuotesPath);
            string selectedFile = files[_random.Next(files.Length)];
            await context.Message.RespondWithFileAsync(selectedFile);
        }
    }
}