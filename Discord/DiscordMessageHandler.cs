using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordSandbot.Database;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace DiscordSandbot.Discord
{
    public class DiscordMessageHandler : IDiscordMessageHandler
    {
        private readonly IDatabaseService _database;
        private readonly string _botAdminUsername = "jooohnny32"; //For private functions
        private readonly string _botUsername = "Sandbot";

        public DiscordMessageHandler(IDatabaseService database)
        {
            _database = database;
        }

        public async Task HandleMessageAsync(DiscordClient client, MessageCreateEventArgs args)
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

            return;

            /*switch (args.Message.Content)
            {
                case "!help":
                    {
                        await args.Message.RespondAsync(GetListOfCommands());
                        break;
                    }
                case "!setup":
                    {
                        if (args.Message.Author.Username == _botAdminUsername)
                        {
                            await _database.SetupAsync();
                        }
                        break;
                    }
                case "!destroy":
                    {
                        if (args.Message.Author.Username == _botAdminUsername)
                        {
                            await _database.DestroyAsync();
                        }
                        break;
                    }
                case "!listEmojis":
                    {
                        var results = await _database.GetAllEmojisAsync();

                        if (!results.Any())
                            break;

                        var sb = new StringBuilder();
                        foreach (dynamic emoji in results)
                        {
                            sb.AppendLine($"{emoji.Username} - {emoji.EmojiId} - {emoji.MessageTimestamp}");
                        }

                        await args.Message.RespondAsync(sb.ToString());
                        break;
                    }
                default:
                    {
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

                        break;
                    }
            }*/
        }
    }
}