using System.Threading.Tasks;
using DSharpPlus.EventArgs;

namespace DiscordSandbot.Discord
{
    public interface IDiscordMessageHandler
    {
        Task HandleMessageAsync(MessageCreateEventArgs args);
    }
}