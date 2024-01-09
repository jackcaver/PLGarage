using GameServer.Models;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.Games;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Request;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace GameServer.Utils
{
    public class UserGeneratedContentUtils
    {
        public static void SaveAvatar(int UserId, PlayerAvatar avatar, Stream stream, bool IsMNR) 
        {
            if (!Directory.Exists($"UGC/PlayerAvatars/{UserId}/"))
            {
                Directory.CreateDirectory($"UGC/PlayerAvatars/{UserId}/");
            }
            if (!Directory.Exists($"UGC/PlayerAvatars/{UserId}/MNR/") && IsMNR)
            {
                Directory.CreateDirectory($"UGC/PlayerAvatars/{UserId}/MNR/");
            }
            FileStream file;
            if (!IsMNR)
                file = File.Create($"UGC/PlayerAvatars/{UserId}/{avatar.player_avatar_type.ToString().ToLower()}.png");
            else
                file = File.Create($"UGC/PlayerAvatars/{UserId}/MNR/{avatar.player_avatar_type.ToString().ToLower()}.png");
            stream.CopyTo(file);
            file.Close();
            stream.Close();
        }

        public static void SaveGriefReportData(int id, Stream data, Stream preview)
        {
            if (!Directory.Exists($"UGC/GriefReports/{id}"))
            {
                Directory.CreateDirectory($"UGC/GriefReports/{id}/");
            }
            FileStream previewFile = File.Create($"UGC/GriefReports/{id}/Preview.png");
            preview.CopyTo(previewFile);
            previewFile.Close();
            FileStream dataFile = File.Create($"UGC/GriefReports/{id}/data.xml");
            data.CopyTo(dataFile);
            dataFile.Close();
            data.Close();
            preview.Close();
        }

        public static void SavePlayerCreationComplaintPreview(int id, Stream preview)
        {
            if (!Directory.Exists($"UGC/PlayerCreationComplaints/{id}"))
            {
                Directory.CreateDirectory($"UGC/PlayerCreationComplaints/{id}/");
            }
            FileStream previewFile = File.Create($"UGC/PlayerCreationComplaints/{id}/Preview.png");
            preview.CopyTo(previewFile);
            previewFile.Close();
            preview.Close();
        }

        public static void SavePlayerCreation(int id, Stream data, Stream preview)
        {
            if (!Directory.Exists($"UGC/PlayerCreations/{id}/"))
            {
                Directory.CreateDirectory($"UGC/PlayerCreations/{id}/");
            }
            FileStream dataFile = File.Create($"UGC/PlayerCreations/{id}/data.bin");
            data.CopyTo(dataFile);
            dataFile.Close();
            data.Close();

            FileStream previewFile = File.Create($"UGC/PlayerCreations/{id}/preview_image.png");
            preview.CopyTo(previewFile);
            previewFile.Close();
            preview.Close();
        }

        public static void SavePlayerCreation(int id, Stream data)
        {
            if (!Directory.Exists($"UGC/PlayerCreations/{id}/"))
            {
                Directory.CreateDirectory($"UGC/PlayerCreations/{id}/");
            }
            FileStream dataFile = File.Create($"UGC/PlayerCreations/{id}/data.bin");
            data.CopyTo(dataFile);
            dataFile.Close();
            data.Close();
        }

        public static void SavePlayerPhoto(int id, Stream data)
        {
            if (!Directory.Exists($"UGC/PlayerCreations/{id}/"))
            {
                Directory.CreateDirectory($"UGC/PlayerCreations/{id}/");
            }
            FileStream dataFile = File.Create($"UGC/PlayerCreations/{id}/data.jpg");
            data.CopyTo(dataFile);
            dataFile.Close();
            data.Close();
        }

        public static void SaveGhostCarData(GameType gameType, Platform platform, int track_id, int player_id, Stream data)
        {
            if (!Directory.Exists($"UGC/GhostCarData/{gameType}/{platform}/{track_id}/{player_id}/"))
            {
                Directory.CreateDirectory($"UGC/GhostCarData/{gameType}/{platform}/{track_id}/{player_id}/");
            }
            FileStream dataFile = File.Create($"UGC/GhostCarData/{gameType}/{platform}/{track_id}/{player_id}/data.bin");
            data.CopyTo(dataFile);
            dataFile.Close();
            data.Close();
        }

        public static FileStream LoadGhostCarData(GameType gameType, Platform platform, int track_id, int player_id)
        {
            if (File.Exists($"UGC/GhostCarData/{gameType}/{platform}/{track_id}/{player_id}/data.bin"))
                return File.OpenRead($"UGC/GhostCarData/{gameType}/{platform}/{track_id}/{player_id}/data.bin");
            else
                return null;
        }

        public static string CalculateGhostCarDataMD5(Stream data)
        {
            string hash = BitConverter.ToString(MD5.Create().ComputeHash(data)).Replace("-", "").ToLower();
            return hash;
        }

        public static string CalculateGhostCarDataMD5(GameType gameType, Platform platform, int track_id, int player_id)
        {
            FileStream fileStream;
            if (File.Exists($"UGC/GhostCarData/{gameType}/{platform}/{track_id}/{player_id}/data.bin"))
                fileStream = File.OpenRead($"UGC/GhostCarData/{gameType}/{platform}/{track_id}/{player_id}/data.bin");
            else
                return "";

            string hash = BitConverter.ToString(MD5.Create().ComputeHash(fileStream)).Replace("-", "").ToLower();
            fileStream.Close();
            return hash;
        }

        public static FileStream LoadPlayerCreation(int id, string file)
        {
            if (File.Exists($"UGC/PlayerCreations/{id}/{file}"))
                return File.OpenRead($"UGC/PlayerCreations/{id}/{file}");
            else if (file.Contains("_128x128") && File.Exists($"UGC/PlayerCreations/{id}/{file.Replace("_128x128", "")}"))
                return File.OpenRead($"UGC/PlayerCreations/{id}/{file.Replace("_128x128", "")}");
            else
                return null;
        }

        public static FileStream LoadPlayerAvatar(int id, string file, bool IsMNR = false)
        {
            if (File.Exists($"UGC/PlayerAvatars/{id}/{file}") && !IsMNR)
                return File.OpenRead($"UGC/PlayerAvatars/{id}/{file}");
            else if (file.Contains("_128x128") && File.Exists($"UGC/PlayerAvatars/{id}/{file.Replace("_128x128", "")}") && !IsMNR)
                return File.OpenRead($"UGC/PlayerAvatars/{id}/{file.Replace("_128x128", "")}");
            else if (File.Exists($"UGC/PlayerAvatars/{id}/MNR/{file}") && IsMNR)
                return File.OpenRead($"UGC/PlayerAvatars/{id}/MNR/{file}");
            else if (file.Contains("_128x128") && File.Exists($"UGC/PlayerAvatars/{id}/MNR/{file.Replace("_128x128", "")}") && IsMNR)
                return File.OpenRead($"UGC/PlayerAvatars/{id}/MNR/{file.Replace("_128x128", "")}");
            else
                return null;
        }

        public static FileStream LoadAnnouncementImage(string file)
        {
            if (File.Exists($"UGC/Announcements/{file}"))
                return File.OpenRead($"UGC/Announcements/{file}");
            return null;
        }

        public static string CalculateMD5(int id, string file)
        {
            FileStream fileStream;
            if (File.Exists($"UGC/PlayerCreations/{id}/{file}"))
                fileStream = File.OpenRead($"UGC/PlayerCreations/{id}/{file}");
            else if (file.Contains("_128x128") && File.Exists($"UGC/PlayerCreations/{id}/{file.Replace("_128x128", "")}"))
                fileStream = File.OpenRead($"UGC/PlayerCreations/{id}/{file.Replace("_128x128", "")}");
            else
                return "";

            string hash = BitConverter.ToString(MD5.Create().ComputeHash(fileStream)).Replace("-", "").ToLower();
            fileStream.Close();
            return hash;
        }

        public static string CalculateAvatarMD5(int id, string file, bool IsMNR = false)
        {
            FileStream fileStream;
            if (File.Exists($"UGC/PlayerAvatars/{id}/{file}") && !IsMNR)
                fileStream = File.OpenRead($"UGC/PlayerAvatars/{id}/{file}");
            else if (file.Contains("_128x128") && File.Exists($"UGC/PlayerAvatars/{id}/{file.Replace("_128x128", "")}") && !IsMNR)
                fileStream = File.OpenRead($"UGC/PlayerAvatars/{id}/{file.Replace("_128x128", "")}");
            else if (File.Exists($"UGC/PlayerAvatars/{id}/MNR/{file}") && IsMNR)
                fileStream = File.OpenRead($"UGC/PlayerAvatars/{id}/MNR/{file}");
            else if (file.Contains("_128x128") && File.Exists($"UGC/PlayerAvatars/{id}/MNR/{file.Replace("_128x128", "")}") && IsMNR)
                fileStream = File.OpenRead($"UGC/PlayerAvatars/{id}/MNR/{file.Replace("_128x128", "")}");
            else
                return "";

            string hash = BitConverter.ToString(MD5.Create().ComputeHash(fileStream)).Replace("-", "").ToLower();
            fileStream.Close();
            return hash;
        }

        public static long CalculateSize(int id, string file)
        {
            FileStream fileStream = File.OpenRead($"UGC/PlayerCreations/{id}/{file}");
            long size = fileStream.Length;
            fileStream.Close();
            return size;
        }

        public static void RemovePlayerCreation(int id)
        {
            if (!Directory.Exists($"UGC/PlayerCreations/{id}/"))
                return;
            Directory.Delete($"UGC/PlayerCreations/{id}/", true);
        }

        private static Dictionary<int, StoryLevelData> StoryLevels = new Dictionary<int, StoryLevelData>
        {
            //LBPK
            { 845, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 576, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 753, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 734, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 510, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 612, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 861, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 755, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 758, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 657, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 930, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 951, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 610, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 501, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 777, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 689, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 814, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 869, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 729, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 596, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 760, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 699, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 705, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 939, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 903, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 609, new StoryLevelData { Name = "story level", RaceType = RaceType.BATTLE, ScoreboardMode = 0 } },
            { 647, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 529, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 915, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 684, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 849, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 582, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 790, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 857, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 715, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 738, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 625, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 959, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 998, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 520, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 881, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 828, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 712, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 840, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 624, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 811, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 823, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 918, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 614, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 1049, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 821, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 702, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 913, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 766, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 606, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 550, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 708, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 648, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 772, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 688, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 579, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 539, new StoryLevelData { Name = "story level", RaceType = RaceType.BATTLE, ScoreboardMode = 0 } },
            { 698, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 759, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 763, new StoryLevelData { Name = "story level", RaceType = RaceType.BATTLE, ScoreboardMode = 0 } },
            { 941, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 630, new StoryLevelData { Name = "story level", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 697, new StoryLevelData { Name = "story level", RaceType = RaceType.BATTLE, ScoreboardMode = 0 } },
            { 808, new StoryLevelData { Name = "story level", RaceType = RaceType.BATTLE, ScoreboardMode = 1 } },
            { 1023, new StoryLevelData { Name = "story level", RaceType = RaceType.BATTLE, ScoreboardMode = 0 } },
            { 703, new StoryLevelData { Name = "story level", RaceType = RaceType.BATTLE, ScoreboardMode = 0 } },

            //MNR
            //TODO: add MNR story levels
        };

        public static void AddStoryLevel(Database database, int id) 
        {
            //genius story level check
            if (!StoryLevels.ContainsKey(id)) return;

            var UFGUser = database.Users.FirstOrDefault(match => match.Username == "ufg");

            if (UFGUser == null)
            {
                database.Users.Add(new User
                {
                    UserId = 1,
                    Username = "ufg",
                    CreatedAt = DateTime.Parse("2012-11-06"),
                    UpdatedAt = DateTime.Parse("2012-11-06"),
                    PolicyAccepted = false,
                    Quote = "ufg"
                });
                database.SaveChanges();
                UFGUser = database.Users.FirstOrDefault(match => match.Username == "ufg");
            }

            if (database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == id) == null)
            {
                database.PlayerCreations.Add(new PlayerCreationData
                {
                    AI = true,
                    PlayerId = UFGUser.UserId,
                    TrackId = id,
                    PlayerCreationId = id,
                    ScoreboardMode = StoryLevels[id].ScoreboardMode,
                    Name = StoryLevels[id].Name,
                    Description = "story level",
                    IsRemixable = false,
                    Type = PlayerCreationType.STORY,
                    FirstPublished = DateTime.Parse("2012-11-06"),
                    LastPublished = DateTime.Parse("2012-11-06"),
                    CreatedAt = DateTime.Parse("2012-11-06"),
                    UpdatedAt = DateTime.Parse("2012-11-06"),
                    Platform = Platform.PS3,
                    RaceType = StoryLevels[id].RaceType
                });
                database.SaveChanges();
            }
        }
    }
}
