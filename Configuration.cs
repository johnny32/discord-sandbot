namespace DiscordSandbot
{
    public class Configuration
    {
        public string DiscordToken { get; set; }
        public string BotUsername { get; set; }
        public string BotAdminUsername { get; set; }
        public string ConnectionString { get; set; }
        public string CommandPrefix { get; set; }
        public string HarterQuotesPath { get; set; }
    }
}