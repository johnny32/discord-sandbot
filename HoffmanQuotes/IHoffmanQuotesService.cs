using System.Threading.Tasks;
using DSharpPlus.CommandsNext;

namespace DiscordSandbot.HoffmanQuotes
{
    public interface IHoffmanQuotesService
    {
        Task GetRandomQuoteAsync(CommandContext context);
    }
}