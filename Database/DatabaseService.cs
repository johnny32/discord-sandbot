using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;

namespace DiscordSandbot.Database
{
    public class DatabaseService : IDatabaseService
    {
        private readonly Configuration _configuration;

        public DatabaseService(Configuration configuration)
        {
            _configuration = configuration;
        }

        public async Task SetupAsync()
        {
            using (var connection = new SqliteConnection(_configuration.ConnectionString))
            {
                var sb = new StringBuilder();
                sb.Append(" SELECT name ");
                sb.Append(" FROM sqlite_master ");
                sb.Append(" WHERE type = 'table' AND name = 'LogEmoji' ");

                var table = await connection.QueryAsync<string>(sb.ToString());
                if (!table.Any(name => name == "LogEmoji"))
                {
                    sb.Clear();
                    sb.Append(" CREATE TABLE LogEmoji ( ");
                    sb.Append(" Username VARCHAR(30) NOT NULL, ");
                    sb.Append(" EmojiId VARCHAR(33) NOT NULL, ");
                    sb.Append(" MessageTimestamp TIMESTAMP ");
                    sb.Append(" ) ");

                    await connection.ExecuteAsync(sb.ToString());
                }
            }
        }

        public async Task DestroyAsync()
        {
            using (var connection = new SqliteConnection(_configuration.ConnectionString))
            {
                await connection.ExecuteAsync("DROP TABLE LogEmoji");
            }
        }

        public async Task InsertEmojiAsync(string emojiId, string username, DateTime timestamp)
        {
            using (var connection = new SqliteConnection(_configuration.ConnectionString))
            {
                var sb = new StringBuilder();
                sb.Append(" INSERT INTO LogEmoji ");
                sb.Append(" (Username, EmojiId, MessageTimestamp) ");
                sb.Append(" VALUES (@username, @emojiId, @timestamp) ");

                var parameters = new
                {
                    username,
                    emojiId,
                    timestamp
                };

                await connection.ExecuteAsync(sb.ToString(), parameters);
            }
        }

        public async Task<IEnumerable<dynamic>> GetAllEmojisAsync()
        {
            using (var connection = new SqliteConnection(_configuration.ConnectionString))
            {
                return await connection.QueryAsync("SELECT * FROM LogEmoji");
            }
        }
    }
}