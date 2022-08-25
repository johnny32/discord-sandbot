using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DiscordSandbot.Database;
using DiscordSandbot.Helpers;
using DiscordSandbot.Models;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using TimeZoneConverter;

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
        private readonly LimitedQueue<DiscordMessage> _lastMessagesQueue;
        private const int LAST_MESSAGES_QUEUE_CAPACITY = 3;

        public DiscordMessageHandler(ILogger<DiscordMessageHandler> logger, IDatabaseService database, Configuration configuration)
        {
            _logger = logger;
            _database = database;
            _configuration = configuration;
            _regex = new Regex(_emojiPattern);
            _random = new Random();
            _lastMessagesQueue = new LimitedQueue<DiscordMessage>(LAST_MESSAGES_QUEUE_CAPACITY);
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

                    try
                    {
                        DiscordEmoji emojiObj = DiscordEmoji.FromName(client, emoji);
                    }
                    catch (ArgumentException)
                    {
                        _logger.LogInformation($"{args.Message.Author.Username} wrote an emoji not present on this server: {emoji}. Ignoring...");
                        continue;
                    }

                    _logger.LogInformation($"{args.Message.Author.Username} wrote {emoji}");

                    //Add all the possible emojis to the database
                    await _database.InsertEmojiAsync(new LogEmoji
                    {
                        EmojiId = emoji,
                        Username = args.Message.Author.Username,
                        MessageTimestamp = TimeZoneInfo.ConvertTimeFromUtc(args.Message.Timestamp.UtcDateTime, TZConvert.GetTimeZoneInfo("Romance Standard Time")),
                        IsReaction = false
                    });
                }

                string message = args.Message.Content.Replace("?", "").Replace("!", "").Replace(".", "").Replace("¡", "").Replace("¿", "").Replace("\"", "").Replace("'", "").Replace(")", "").TrimEnd().ToLowerInvariant();
                if (message.EndsWith("gado") && !message.EndsWith("colgado"))
                    await args.Message.RespondAsync($"{args.Message.Author.Mention} el que tengo aquí colgado");
                else if (message.EndsWith("gada") && !message.EndsWith("colgada"))
                    await args.Message.RespondAsync($"{args.Message.Author.Mention} la que tengo aquí colgada");
                else if (message.EndsWith("gados") && !message.EndsWith("colgados"))
                    await args.Message.RespondAsync($"{args.Message.Author.Mention} los que tengo aquí colgados");
                else if (message.EndsWith("gadas") && !message.EndsWith("colgadas"))
                    await args.Message.RespondAsync($"{args.Message.Author.Mention} las que tengo aquí colgadas");
                else if (message.EndsWith(" concurso"))
                    await args.Message.RespondAsync($"{args.Message.Author.Mention} el de levantar mi polla a pulso");
                else if (message.EndsWith("ano") && !message.EndsWith(" mano"))
                    await args.Message.RespondAsync($"{args.Message.Author.Mention} me la agarras con la mano");

                _lastMessagesQueue.Enqueue(args.Message);
                if (_lastMessagesQueue.Count == LAST_MESSAGES_QUEUE_CAPACITY
                    && _lastMessagesQueue.All(message => !message.Content.Any(c => Char.IsLower(c))
                        && message.Content.Count(c => Char.IsUpper(c)) > message.Content.Count(c => !Char.IsUpper(c))
                        && message.Author.Equals(_lastMessagesQueue.First().Author)))
                {
                    await args.Message.RespondAsync($"{args.Message.Author.Mention} cálmate, estás montando una escenita.");
                    _lastMessagesQueue.Clear();
                }

                double randomValue = _random.NextDouble();
                if (randomValue < 0.00001)
                {
                    DiscordMessage messageObj = await args.Message.RespondAsync($"{args.Message.Author.Mention} hijo de puta");
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
                string emoji = $":{args.Emoji.Name}:";

                try
                {
                    DiscordEmoji emojiObj = DiscordEmoji.FromName(client, emoji);
                }
                catch (ArgumentException)
                {
                    _logger.LogInformation($"{args.Message.Author.Username} reacted with an emoji not present on this server: {emoji}. Ignoring...");
                    return;
                }

                _logger.LogInformation($"{args.User.Username} reacted {emoji}");

                await _database.InsertEmojiAsync(new LogEmoji
                {
                    EmojiId = emoji,
                    Username = args.User.Username,
                    MessageTimestamp = TimeZoneInfo.ConvertTimeFromUtc(args.Message.Timestamp.UtcDateTime, TZConvert.GetTimeZoneInfo("Romance Standard Time")),
                    IsReaction = true
                });
            }
        }

        public async Task HandleRemoveReactionAsync(DiscordClient client, MessageReactionRemoveEventArgs args)
        {
            if (args.Emoji.Id > 0L)
            {
                //It's a custom emoji
                string emoji = $":{args.Emoji.Name}:";

                try
                {
                    DiscordEmoji emojiObj = DiscordEmoji.FromName(client, emoji);
                }
                catch (ArgumentException)
                {
                    _logger.LogInformation($"{args.Message.Author.Username} removed a reaction of an emoji not present on this server: {emoji}. Ignoring...");
                    return;
                }

                _logger.LogInformation($"{args.User.Username} removed reaction {emoji}");

                await _database.RemoveEmojiAsync(new LogEmoji
                {
                    EmojiId = $":{args.Emoji.Name}:",
                    Username = args.User.Username,
                    MessageTimestamp = TimeZoneInfo.ConvertTimeFromUtc(args.Message.Timestamp.UtcDateTime, TZConvert.GetTimeZoneInfo("Romance Standard Time")),
                    IsReaction = true
                });
            }
        }
    }
}