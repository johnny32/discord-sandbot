using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DiscordSandbot.Database;
using DiscordSandbot.Models;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace DiscordSandbot.Discord
{
    public class DiscordMessageHandler : IDiscordMessageHandler
    {
        private readonly IDatabaseService _database;
        private readonly Configuration _configuration;
        private readonly string _botUsername = "Sandbot";
        private readonly string _emojiPattern = @"<:.+?:\d+>";
        private readonly Regex _regex;

        public DiscordMessageHandler(IDatabaseService database, Configuration configuration)
        {
            _database = database;
            _configuration = configuration;
            _regex = new Regex(_emojiPattern);
        }

        public async Task HandleMessageAsync(DiscordClient client, MessageCreateEventArgs args)
        {
            try
            {
                if (args.Message.Author.Username == _botUsername || args.Message.Content.StartsWith(_configuration.CommandPrefix))
                    return;

                //Analize all the possible emojis in the message
                foreach (Match match in _regex.Matches(args.Message.Content))
                {
                    //The emojis have the format <:emoji_name:emoji_id>, so I strip the emoji_name
                    string[] parts = match.Value.Split(':');
                    string emoji = $":{parts[1]}:";
                    //Add all the possible emojis to the database
                    await _database.InsertEmojiAsync(new LogEmoji
                    {
                        EmojiId = emoji,
                        Username = args.Message.Author.Username,
                        MessageTimestamp = TimeZoneInfo.ConvertTimeFromUtc(args.Message.Timestamp.UtcDateTime, TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time")),
                        IsReaction = false
                    });
                }
            }
            catch (Exception e)
            {
                await args.Message.RespondAsync(e.Message);
            }
        }

        public async Task HandleAddReactionAsync(DiscordClient client, MessageReactionAddEventArgs args)
        {
            if (args.Emoji.Id > 0L)
            {
                //It's a custom emoji
                await _database.InsertEmojiAsync(new LogEmoji
                {
                    EmojiId = $":{args.Emoji.Name}:",
                    Username = args.User.Username,
                    MessageTimestamp = TimeZoneInfo.ConvertTimeFromUtc(args.Message.Timestamp.UtcDateTime, TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time")),
                    IsReaction = true
                });
            }
        }

        public async Task HandleRemoveReactionAsync(DiscordClient client, MessageReactionRemoveEventArgs args)
        {
            if (args.Emoji.Id > 0L)
            {
                //It's a custom emoji
                await _database.RemoveEmojiAsync(new LogEmoji
                {
                    EmojiId = $":{args.Emoji.Name}:",
                    Username = args.User.Username,
                    MessageTimestamp = TimeZoneInfo.ConvertTimeFromUtc(args.Message.Timestamp.UtcDateTime, TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time")),
                    IsReaction = true
                });
            }
        }
    }
}