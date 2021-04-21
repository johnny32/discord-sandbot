using System.Threading.Tasks;
using DSharpPlus.CommandsNext;

namespace DiscordSandbot.HarterQuotes
{
    public interface IHarterQuotesService
    {
        Task GetRandomQuoteAsync(CommandContext context);
    }
}