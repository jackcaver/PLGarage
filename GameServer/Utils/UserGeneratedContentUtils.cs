using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Request;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace GameServer.Utils
{
    public class UserGeneratedContentUtils
    {
        public static void SaveAvatar(int UserId, PlayerAvatar avatar, Stream stream) 
        {
            if (!Directory.Exists($"UGC/PlayerAvatars/{UserId}/"))
            {
                Directory.CreateDirectory($"UGC/PlayerAvatars/{UserId}/");
            }
            FileStream file = File.Create($"UGC/PlayerAvatars/{UserId}/{avatar.player_avatar_type.ToString().ToLower()}.png");
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

        public static FileStream LoadPlayerCreation(int id, string file)
        {
            if (File.Exists($"UGC/PlayerCreations/{id}/{file}"))
                return File.OpenRead($"UGC/PlayerCreations/{id}/{file}");
            else if (file.Contains("_128x128") && File.Exists($"UGC/PlayerCreations/{id}/{file.Replace("_128x128", "")}"))
                return File.OpenRead($"UGC/PlayerCreations/{id}/{file.Replace("_128x128", "")}");
            else
                return null;
        }

        public static FileStream LoadPlayerAvatar(int id, string file)
        {
            if (File.Exists($"UGC/PlayerAvatars/{id}/{file}"))
                return File.OpenRead($"UGC/PlayerAvatars/{id}/{file}");
            else if (file.Contains("_128x128") && File.Exists($"UGC/PlayerAvatars/{id}/{file.Replace("_128x128", "")}"))
                return File.OpenRead($"UGC/PlayerAvatars/{id}/{file.Replace("_128x128", "")}");
            else
                return null;
        }

        public static FileStream LoadAnnouncementImage(string file)
        {
            if (File.Exists($"UGC/Announcements/{file}"))
                return File.OpenRead($"UGC/Announcements/{file}");
            return null;
        }

        public static string CalculateMD5(string file)
        {
            FileStream fileStream = File.OpenRead(file);
            string hash = BitConverter.ToString(MD5.Create().ComputeHash(fileStream)).Replace("-", "").ToLower();
            fileStream.Close();
            return hash;
        }

        public static void RemovePlayerCreation(int id)
        {
            if (!Directory.Exists($"UGC/PlayerCreations/{id}/"))
                return;
            Directory.Delete($"UGC/PlayerCreations/{id}/", true);
        }

        public static void AddStoryLevel(Database database, int raceType, int id, SortColumn sortColumn) 
        {
            var UFGUser = database.Users.FirstOrDefault(match => match.Username == "ufg");

            if (UFGUser == null)
            {
                database.Users.Add(new User
                {
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
                    ScoreboardMode = (sortColumn == SortColumn.finish_time) ? 1 : 0,
                    Name = "story level",
                    Description = "story level",
                    IsRemixable = false,
                    Type = PlayerCreationType.STORY,
                    FirstPublished = DateTime.Parse("2012-11-06"),
                    LastPublished = DateTime.Parse("2012-11-06"),
                    CreatedAt = DateTime.Parse("2012-11-06"),
                    UpdatedAt = DateTime.Parse("2012-11-06"),
                    Platform = Platform.PS3,
                    RaceType = (raceType == 702) ? RaceType.BATTLE : RaceType.RACE
                });
                database.SaveChanges();
            }
        }
    }
}
