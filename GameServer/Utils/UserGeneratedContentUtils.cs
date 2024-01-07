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

        public static void AddStoryLevel(Database database, int raceType, int id, SortColumn sortColumn) 
        {
            //genius story level check
            List<int> storyLevelIds = new List<int> { 
                //LBPK
                845, 576, 753, 734, 510, 612, 861, 755, 758, 657, 930, 951, 610, 501, 777, 689, 814, 869, 729, 596, 760, 699,
                705, 939, 903, 609, 647, 529, 915, 684, 849, 582, 790, 857, 715, 738, 625, 959, 998, 520, 881, 828, 712, 840,
                624, 811, 823, 918, 614, 1049, 821, 702, 913, 766, 606, 550, 708, 648, 772, 688, 579, 539, 698, 759, 763, 941,
                630, 697, 808, 1023, 703 
                //MNR
                //TODO: add MNR story levels
            };
            if (!storyLevelIds.Contains(id)) return;

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
