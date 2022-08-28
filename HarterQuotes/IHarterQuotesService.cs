using System.Threading.Tasks;
using DSharpPlus.CommandsNext;

namespace DiscordSandbot.HarterQuotes
{
    public interface IHarterQuotesService
    {
        /// <summary>
        /// Writes a random Harter quote when asked for with the command "!!Harter".
        /// </summary>
        /// <param name="context">The Discord command context to which the service will reply.</param>
        Task GetRandomQuoteAsync(CommandContext context);
    }
}