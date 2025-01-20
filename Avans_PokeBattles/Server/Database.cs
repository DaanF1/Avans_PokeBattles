using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avans_PokeBattles.Server
{
    public static class Database
    {
        private const string ConnectionString = "Data Source=profiles.db;Version=3;";

        public static void Initialize()
        {
            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Profiles (
                    Name TEXT PRIMARY KEY,
                    Wins INTEGER,
                    Losses INTEGER
                )";
            using var command = new SQLiteCommand(createTableQuery, connection);
            command.ExecuteNonQuery();
        }

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(ConnectionString);
        }
    }
}