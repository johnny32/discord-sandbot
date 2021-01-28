using System;

namespace DiscordSandbot.Models
{
    public class LogEmoji
    {
        public string Username { get; set; }
        public string EmojiId { get; set; }

        public DateTime MessageTimestamp { get; set; }
        public bool IsReaction { get; set; }
    }
}