using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DiscordSandbot.Models;
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
                    sb.Append(" MessageTimestamp TIMESTAMP, ");
                    sb.Append(" IsReaction INTEGER ");
                    sb.Append(" ) ");

                    await connection.ExecuteAsync(sb.ToString());
                }

                sb.Clear();
                sb.Append(" SELECT name ");
                sb.Append(" FROM sqlite_master ");
                sb.Append(" WHERE type = 'index' AND name = 'LogEmoji_EmojiId_Idx' ");
                var index = await connection.QueryAsync<string>(sb.ToString());
                if (!index.Any(name => name == "LogEmoji_EmojiId_Idx"))
                {
                    sb.Clear();
                    sb.Append(" CREATE INDEX LogEmoji_EmojiId_Idx ");
                    sb.Append(" ON LogEmoji(EmojiId) ");

                    await connection.ExecuteAsync(sb.ToString());
                }

                sb.Clear();
                sb.Append(" SELECT name ");
                sb.Append(" FROM sqlite_master ");
                sb.Append(" WHERE type = 'index' AND name = 'LogEmoji_MessageTimestamp_Idx' ");
                index = await connection.QueryAsync<string>(sb.ToString());
                if (!index.Any(name => name == "LogEmoji_MessageTimestamp_Idx"))
                {
                    sb.Clear();
                    sb.Append(" CREATE INDEX LogEmoji_MessageTimestamp_Idx ");
                    sb.Append(" ON LogEmoji(MessageTimestamp) ");

                    await connection.ExecuteAsync(sb.ToString());
                }

                sb.Clear();
                sb.Append(" SELECT name ");
                sb.Append(" FROM sqlite_master ");
                sb.Append(" WHERE type = 'index' AND name = 'LogEmoji_Username_Idx' ");
                index = await connection.QueryAsync<string>(sb.ToString());
                if (!index.Any(name => name == "LogEmoji_Username_Idx"))
                {
                    sb.Clear();
                    sb.Append(" CREATE INDEX LogEmoji_Username_Idx ");
                    sb.Append(" ON LogEmoji(Username) ");

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

        public async Task InsertEmojiAsync(LogEmoji emoji)
        {
            using (var connection = new SqliteConnection(_configuration.ConnectionString))
            {
                var sb = new StringBuilder();
                sb.Append(" INSERT INTO LogEmoji ");
                sb.Append(" (Username, EmojiId, MessageTimestamp, IsReaction) ");
                sb.Append(" VALUES (@Username, @EmojiId, @MessageTimestamp, @IsReaction) ");

                await connection.ExecuteAsync(sb.ToString(), emoji);
            }
        }

        public async Task RemoveEmojiAsync(LogEmoji emoji)
        {
            using (var connection = new SqliteConnection(_configuration.ConnectionString))
            {
                var sb = new StringBuilder();
                sb.Append(" DELETE FROM LogEmoji ");
                sb.Append(" WHERE EmojiId = @EmojiId ");
                sb.Append(" AND Username = @Username ");
                sb.Append(" AND MessageTimestamp = @MessageTimestamp ");
                sb.Append(" AND IsReaction = @IsReaction ");

                await connection.ExecuteAsync(sb.ToString(), emoji);
            }
        }

        public async Task<IEnumerable<LogEmoji>> GetAllEmojisAsync()
        {
            using (var connection = new SqliteConnection(_configuration.ConnectionString))
            {
                return await connection.QueryAsync<LogEmoji>("SELECT * FROM LogEmoji");
            }
        }

        public async Task<IEnumerable<LogEmoji>> GetAllUsersOfEmojiAsync(string emojiId)
        {
            using (var connection = new SqliteConnection(_configuration.ConnectionString))
            {
                var sb = new StringBuilder();
                sb.Append(" SELECT * ");
                sb.Append(" FROM LogEmoji ");
                sb.Append(" WHERE EmojiId = @emojiId ");

                return await connection.QueryAsync<LogEmoji>(sb.ToString(), new { emojiId });
            }
        }

        public async Task<IEnumerable<LogEmoji>> GetAllEmojisOfUserAsync(string username)
        {
            using (var connection = new SqliteConnection(_configuration.ConnectionString))
            {
                var sb = new StringBuilder();
                sb.Append(" SELECT * ");
                sb.Append(" FROM LogEmoji ");
                sb.Append(" WHERE Username = @username ");

                return await connection.QueryAsync<LogEmoji>(sb.ToString(), new { username });
            }
        }

        public async Task DeleteEmojiAsync(string emojiId)
        {
            using (var connection = new SqliteConnection(_configuration.ConnectionString))
            {
                var sb = new StringBuilder();
                sb.Append(" DELETE ");
                sb.Append(" FROM LogEmoji ");
                sb.Append(" WHERE EmojiId = @emojiId ");

                await connection.ExecuteAsync(sb.ToString(), new { emojiId });
            }
        }

        public async Task<IEnumerable<LogEmoji>> LogEmojisAsync(int numResults)
        {
            using (var connection = new SqliteConnection(_configuration.ConnectionString))
            {
                var sb = new StringBuilder();
                sb.Append(" SELECT * ");
                sb.Append(" FROM LogEmoji ");
                sb.Append(" ORDER BY MessageTimestamp DESC ");
                if (numResults > -1)
                    sb.Append($" LIMIT {numResults} ");

                return await connection.QueryAsync<LogEmoji>(sb.ToString());
            }
        }
    }
}