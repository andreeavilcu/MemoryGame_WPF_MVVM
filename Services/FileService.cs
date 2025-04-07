using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using MemoryGame.Models;

namespace MemoryGame.Services
{
    public static class FileService
    {
        private static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string UsersFilePath = Path.Combine(BaseDirectory, "users.json");
        private static readonly string GamesDirectory = Path.Combine(BaseDirectory, "SavedGames");
        private static readonly string StatsFilePath = Path.Combine(BaseDirectory, "statistics.json");

        public static void SaveUsers(List<UserModel> users)
        {
            EnsureDirectoriesExist();
            string json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(UsersFilePath, json);
        }

        public static List<UserModel> LoadUsers()
        {
            EnsureDirectoriesExist();
            if (!File.Exists(UsersFilePath))
            {
                return new List<UserModel>();
            }
            string json = File.ReadAllText(UsersFilePath);
            try
            {
                return JsonSerializer.Deserialize<List<UserModel>>(json) ?? new List<UserModel>();
            }
            catch (JsonException)
            {
               return new List<UserModel>();
            }
        }

        public static void SaveGame(string username, GameModel game)
        {
            EnsureDirectoriesExist();
            string filePath = Path.Combine(GamesDirectory, $"{username}.json");
            string json = JsonSerializer.Serialize(game, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        public static GameModel LoadGame(string username)
        {
            string filePath = Path.Combine(GamesDirectory, $"{username}.json");
            if (!File.Exists(filePath))
            {
                return null;
            }

            string json = File.ReadAllText(filePath);
            try
            {
                return JsonSerializer.Deserialize<GameModel>(json);
            }
            catch
            {
                return null;
            }
        }

        public static void DeleteGame(string username)
        {
            string filePath = Path.Combine(GamesDirectory, $"{username}.json");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public static void SaveStatistics(List<StatisticsModel> statistics )
        {
            EnsureDirectoriesExist();
            string json = JsonSerializer.Serialize(statistics, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(StatsFilePath, json);
        }

        public static List<StatisticsModel> LoadStatistics()
        {
            EnsureDirectoriesExist();
            if ( !File.Exists(StatsFilePath))
            {
                return new List<StatisticsModel>();
            }

            string json = File.ReadAllText(StatsFilePath);
            try
            {
                return JsonSerializer.Deserialize<List<StatisticsModel>>(json) ?? new List<StatisticsModel>();
            }
            catch
            {
                return new List<StatisticsModel>();
            }
        }

        public static void UpdateStatistics(string username, bool won)
        {
            var statistics = LoadStatistics();
            var userStats = statistics.Find(s => s.Username == username);

            if (userStats == null)
            {
                userStats = new StatisticsModel { Username = username, GamesPlayed = 0, GamesWon = 0 };
                statistics.Add(userStats);
            }

            userStats.GamesPlayed++;
            if (won)
            {
                userStats.GamesWon++;
            }

            SaveStatistics(statistics);
        }


        public static void DeleteUserData(string username)
        {
            DeleteGame(username);

            var statistics = LoadStatistics();
            statistics.RemoveAll(s => s.Username == username);
            SaveStatistics(statistics);

            var users = LoadUsers();
            users.RemoveAll(u => u.Username == username);
            SaveUsers(users);
        }

        private static void EnsureDirectoriesExist()
        {
            if (!Directory.Exists(GamesDirectory))
            {
                Directory.CreateDirectory(GamesDirectory);
            }
        }

    }
}
