using System;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Logging;

namespace DiscordSandbot.HoffmanQuotes
{
    public class HoffmanQuotesService : IHoffmanQuotesService
    {
        private readonly ILogger _logger;
        private readonly Configuration _configuration;
        private readonly Random _random;

        public HoffmanQuotesService(ILogger<HoffmanQuotesService> logger, Configuration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _random = new Random();
        }

        public async Task GetRandomQuoteAsync(CommandContext context)
        {
            string[] quotes = File.ReadAllLines(_configuration.HoffmanQuotesPath);
            await context.Message.RespondAsync(quotes[_random.Next(quotes.Length)], false, null, null);
        }
    }
}