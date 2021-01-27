using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordSandbot.Database
{
    public interface IDatabaseService
    {
        Task SetupAsync();
        Task DestroyAsync();
        Task InsertEmojiAsync(string emojiId, string username, DateTime timestamp);
        Task<IEnumerable<dynamic>> GetAllEmojisAsync();
        Task<IEnumerable<dynamic>> GetAllUsersOfEmojiAsync(string emojiId);
        Task<IEnumerable<dynamic>> GetAllEmojisOfUserAsync(string username);
    }
}