using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DiscordSandbot.Database;
using DiscordSandbot.Models;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;

namespace DiscordSandbot.Discord
{
    public class DiscordMessageHandler : IDiscordMessageHandler
    {
        private readonly ILogger _logger;
        private readonly IDatabaseService _database;
        private readonly Configuration _configuration;
        private readonly string _emojiPattern = @"<:.+?:\d+>";
        private readonly Regex _regex;
        private readonly Random _random;

        public DiscordMessageHandler(ILogger<DiscordMessageHandler> logger, IDatabaseService database, Configuration configuration)
        {
            _logger = logger;
            _database = database;
            _configuration = configuration;
            _regex = new Regex(_emojiPattern);
            _random = new Random();
        }

        public async Task HandleMessageAsync(DiscordClient client, MessageCreateEventArgs args)
        {
            try
            {
                if (args.Message.Author.Username == _configuration.BotUsername || args.Message.Content.StartsWith(_configuration.CommandPrefix))
                    return;

                //Analize all the possible emojis in the message
                foreach (Match match in _regex.Matches(args.Message.Content))
                {
                    //The emojis have the format <:emoji_name:emoji_id>, so I strip the emoji_name
                    string[] parts = match.Value.Split(':');
                    string emoji = $":{parts[1]}:";

                    _logger.LogInformation($"{args.Message.Author.Username} wrote {emoji}");

                    //Add all the possible emojis to the database
                    await _database.InsertEmojiAsync(new LogEmoji
                    {
                        EmojiId = emoji,
                        Username = args.Message.Author.Username,
                        MessageTimestamp = TimeZoneInfo.ConvertTimeFromUtc(args.Message.Timestamp.UtcDateTime, TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time")),
                        IsReaction = false
                    });
                }

                string message = args.Message.Content.Replace("?", "").Replace("!", "").Replace(".", "").Replace("¡", "").Replace("¿", "").Replace("\"", "").Replace("'", "").Replace(")", "").TrimEnd().ToLowerInvariant();
                if (message.EndsWith("ado"))
                    await args.Message.RespondAsync($"{args.Message.Author.Mention} el que tengo aquí colgado");
                else if (message.EndsWith("ada"))
                    await args.Message.RespondAsync($"{args.Message.Author.Mention} la que tengo aquí colgada");
                else if (message.EndsWith("ados"))
                    await args.Message.RespondAsync($"{args.Message.Author.Mention} los que tengo aquí colgados");
                else if (message.EndsWith("adas"))
                    await args.Message.RespondAsync($"{args.Message.Author.Mention} las que tengo aquí colgadas");

                double randomValue = _random.NextDouble();
                if (randomValue < 0.00001)
                {
                    var messageObj = await args.Message.RespondAsync($"{args.Message.Author.Mention} hijo de puta");
                    await Task.Delay(1000);
                    await messageObj.ModifyAsync($"{args.Message.Author.Mention} ~~hijo de puta~~ perdón");
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
                var emoji = $":{args.Emoji.Name}:";
                _logger.LogInformation($"{args.User.Username} reacted {emoji}");

                await _database.InsertEmojiAsync(new LogEmoji
                {
                    EmojiId = emoji,
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
                var emoji = $":{args.Emoji.Name}:";
                _logger.LogInformation($"{args.User.Username} removed reaction {emoji}");

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