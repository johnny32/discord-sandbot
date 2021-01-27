using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordSandbot.Database;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace DiscordSandbot.Discord
{
    public class DiscordMessageHandler : IDiscordMessageHandler
    {
        private readonly IDatabaseService _database;
        private readonly string _botUsername = "Sandbot";

        public DiscordMessageHandler(IDatabaseService database)
        {
            _database = database;
        }

        public async Task HandleMessageAsync(DiscordClient client, MessageCreateEventArgs args)
        {
            try
            {
                if (args.Message.Author.Username == _botUsername || args.Message.Content.StartsWith("!"))
                    return;

                //Analize all the possible emojis in the message
                var possibleEmojis = new List<string>();

                string[] parts = args.Message.Content.Split(':');
                if (parts.Length > 2)
                {
                    for (var i = 1; i < parts.Length - 1; i++)
                    {
                        string part = parts[i];
                        if (!part.Contains(' '))
                        {
                            possibleEmojis.Add($":{part}:");
                        }
                    }
                }

                //Add all the possible emojis to the database
                foreach (string emoji in possibleEmojis)
                {
                    await _database.InsertEmojiAsync(emoji, args.Message.Author.Username, args.Message.Timestamp.UtcDateTime);
                }
            }
            catch (Exception e)
            {
                await args.Message.RespondAsync(e.Message);
            }
        }
    }
}