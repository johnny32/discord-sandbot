using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace DiscordSandbot.Discord
{
    public interface IDiscordMessageHandler
    {
        Task HandleMessageAsync(DiscordClient client, MessageCreateEventArgs args);
        Task HandleAddReactionAsync(DiscordClient client, MessageReactionAddEventArgs args);
    }
}