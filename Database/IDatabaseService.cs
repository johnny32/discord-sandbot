using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordSandbot.Models;

namespace DiscordSandbot.Database
{
    public interface IDatabaseService
    {
        Task SetupAsync();
        Task DestroyAsync();
        Task InsertEmojiAsync(LogEmoji emoji);
        Task RemoveEmojiAsync(LogEmoji emoji);
        Task<IEnumerable<LogEmoji>> GetAllEmojisAsync();
        Task<IEnumerable<LogEmoji>> GetAllUsersOfEmojiAsync(string emojiId);
        Task<IEnumerable<LogEmoji>> GetAllEmojisOfUserAsync(string username);
        Task DeleteEmojiAsync(string emojiId);
    }
}