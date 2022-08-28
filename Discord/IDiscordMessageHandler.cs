using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace DiscordSandbot.Discord
{
    public interface IDiscordMessageHandler
    {
        /// <summary>
        /// Registers the custom emojis for statistics when used in a message, replies with funny rhymes when possible, tells people to calm down and randomly insults and asks for forgiveness later.
        /// </summary>
        /// <param name="client">The Discord client to attach to the object.</param>
        /// <param name="args">The message event arguments, containin information like who sent the message, the contents of the message itself, etc.</param>
        Task HandleMessageAsync(DiscordClient client, MessageCreateEventArgs args);
        /// <summary>
        /// Registers the custom emojis for statistics when added as a reaction to a message.
        /// </summary>
        /// <param name="client">The Discord client to attach to the object.</param>
        /// <param name="args">The message event arguments, containin information like who sent the message, the contents of the message itself, etc.</param>
        Task HandleAddReactionAsync(DiscordClient client, MessageReactionAddEventArgs args);
        /// <summary>
        /// Deregisters the custom emojis for statistics when removed from a reaction to a message.
        /// </summary>
        /// <param name="client">The Discord client to attach to the object.</param>
        /// <param name="args">The message event arguments, containin information like who sent the message, the contents of the message itself, etc.</param>
        Task HandleRemoveReactionAsync(DiscordClient client, MessageReactionRemoveEventArgs args);
    }
}