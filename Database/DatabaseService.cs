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
            using (SqliteConnection connection = new(_configuration.ConnectionString))
            {
                StringBuilder builder = new();
                builder.Append(" SELECT name ");
                builder.Append(" FROM sqlite_master ");
                builder.Append(" WHERE type = 'table' AND name = 'LogEmoji' ");

                IEnumerable<string> table = await connection.QueryAsync<string>(builder.ToString());
                if (!table.Any(name => name == "LogEmoji"))
                {
                    builder.Clear();
                    builder.Append(" CREATE TABLE LogEmoji ( ");
                    builder.Append(" Username VARCHAR(30) NOT NULL, ");
                    builder.Append(" EmojiId VARCHAR(33) NOT NULL, ");
                    builder.Append(" MessageTimestamp TIMESTAMP, ");
                    builder.Append(" IsReaction INTEGER ");
                    builder.Append(" ) ");

                    await connection.ExecuteAsync(builder.ToString());
                }

                builder.Clear();
                builder.Append(" SELECT name ");
                builder.Append(" FROM sqlite_master ");
                builder.Append(" WHERE type = 'index' AND name = 'LogEmoji_EmojiId_Idx' ");
                IEnumerable<string> index = await connection.QueryAsync<string>(builder.ToString());
                if (!index.Any(name => name == "LogEmoji_EmojiId_Idx"))
                {
                    builder.Clear();
                    builder.Append(" CREATE INDEX LogEmoji_EmojiId_Idx ");
                    builder.Append(" ON LogEmoji(EmojiId) ");

                    await connection.ExecuteAsync(builder.ToString());
                }

                builder.Clear();
                builder.Append(" SELECT name ");
                builder.Append(" FROM sqlite_master ");
                builder.Append(" WHERE type = 'index' AND name = 'LogEmoji_MessageTimestamp_Idx' ");
                index = await connection.QueryAsync<string>(builder.ToString());
                if (!index.Any(name => name == "LogEmoji_MessageTimestamp_Idx"))
                {
                    builder.Clear();
                    builder.Append(" CREATE INDEX LogEmoji_MessageTimestamp_Idx ");
                    builder.Append(" ON LogEmoji(MessageTimestamp) ");

                    await connection.ExecuteAsync(builder.ToString());
                }

                builder.Clear();
                builder.Append(" SELECT name ");
                builder.Append(" FROM sqlite_master ");
                builder.Append(" WHERE type = 'index' AND name = 'LogEmoji_Username_Idx' ");
                index = await connection.QueryAsync<string>(builder.ToString());
                if (!index.Any(name => name == "LogEmoji_Username_Idx"))
                {
                    builder.Clear();
                    builder.Append(" CREATE INDEX LogEmoji_Username_Idx ");
                    builder.Append(" ON LogEmoji(Username) ");

                    await connection.ExecuteAsync(builder.ToString());
                }
            }
        }

        public async Task DestroyAsync()
        {
            using (SqliteConnection connection = new(_configuration.ConnectionString))
            {
                await connection.ExecuteAsync("DROP TABLE LogEmoji");
            }
        }

        public async Task InsertEmojiAsync(LogEmoji emoji)
        {
            using (SqliteConnection connection = new(_configuration.ConnectionString))
            {
                StringBuilder builder = new();
                builder.Append(" INSERT INTO LogEmoji ");
                builder.Append(" (Username, EmojiId, MessageTimestamp, IsReaction) ");
                builder.Append(" VALUES (@Username, @EmojiId, @MessageTimestamp, @IsReaction) ");

                await connection.ExecuteAsync(builder.ToString(), emoji);
            }
        }

        public async Task RemoveEmojiAsync(LogEmoji emoji)
        {
            using (SqliteConnection connection = new(_configuration.ConnectionString))
            {
                StringBuilder builder = new();
                builder.Append(" DELETE FROM LogEmoji ");
                builder.Append(" WHERE EmojiId = @EmojiId ");
                builder.Append(" AND Username = @Username ");
                builder.Append(" AND MessageTimestamp = @MessageTimestamp ");
                builder.Append(" AND IsReaction = @IsReaction ");

                await connection.ExecuteAsync(builder.ToString(), emoji);
            }
        }

        public async Task<IEnumerable<LogEmoji>> GetAllEmojisAsync()
        {
            using (SqliteConnection connection = new(_configuration.ConnectionString))
            {
                return await connection.QueryAsync<LogEmoji>("SELECT * FROM LogEmoji");
            }
        }

        public async Task<IEnumerable<LogEmoji>> GetAllUsersOfEmojiAsync(string emojiId)
        {
            using (SqliteConnection connection = new(_configuration.ConnectionString))
            {
                StringBuilder builder = new();
                builder.Append(" SELECT * ");
                builder.Append(" FROM LogEmoji ");
                builder.Append(" WHERE EmojiId = @emojiId ");

                return await connection.QueryAsync<LogEmoji>(builder.ToString(), new { emojiId });
            }
        }

        public async Task<IEnumerable<LogEmoji>> GetAllEmojisOfUserAsync(string username)
        {
            using (SqliteConnection connection = new(_configuration.ConnectionString))
            {
                StringBuilder builder = new();
                builder.Append(" SELECT * ");
                builder.Append(" FROM LogEmoji ");
                builder.Append(" WHERE Username = @username ");

                return await connection.QueryAsync<LogEmoji>(builder.ToString(), new { username });
            }
        }

        public async Task DeleteEmojiAsync(string emojiId)
        {
            using (SqliteConnection connection = new(_configuration.ConnectionString))
            {
                StringBuilder builder = new();
                builder.Append(" DELETE ");
                builder.Append(" FROM LogEmoji ");
                builder.Append(" WHERE EmojiId = @emojiId ");

                await connection.ExecuteAsync(builder.ToString(), new { emojiId });
            }
        }

        public async Task<IEnumerable<LogEmoji>> LogEmojisAsync(int numResults)
        {
            using (SqliteConnection connection = new(_configuration.ConnectionString))
            {
                StringBuilder builder = new();
                builder.Append(" SELECT * ");
                builder.Append(" FROM LogEmoji ");
                builder.Append(" ORDER BY MessageTimestamp DESC ");
                if (numResults > -1)
                    builder.Append($" LIMIT {numResults} ");

                return await connection.QueryAsync<LogEmoji>(builder.ToString());
            }
        }
    }
}