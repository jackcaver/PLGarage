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
            { 845, new StoryLevelData { Name = "TUTORIALLOOP", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 576, new StoryLevelData { Name = "GARDENGRIP", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 753, new StoryLevelData { Name = "TUTORIALDRIFT", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 734, new StoryLevelData { Name = "THEWEDDING", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 510, new StoryLevelData { Name = "THEWEDDING_VERSUS", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 612, new StoryLevelData { Name = "GARDENGRIP_VERSUS", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 861, new StoryLevelData { Name = "TUTORIALWEAPONS", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 755, new StoryLevelData { Name = "SERPENTSHRINE", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 758, new StoryLevelData { Name = "TUTORIALSHIELD", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 657, new StoryLevelData { Name = "SERPENTSHRINE_VERSUS", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 930, new StoryLevelData { Name = "THEMINES", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 951, new StoryLevelData { Name = "SACKRALLYKENYA", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 610, new StoryLevelData { Name = "THEMINES_VERSUS", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 501, new StoryLevelData { Name = "KINGSCASTLE_VERSUS", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 777, new StoryLevelData { Name = "KINGSCASTLE", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 689, new StoryLevelData { Name = "OFFROADTEST", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 814, new StoryLevelData { Name = "SACKBOYGP", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 869, new StoryLevelData { Name = "TURTLEISLAND", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 729, new StoryLevelData { Name = "TURTLEISLAND_VERSUS", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 596, new StoryLevelData { Name = "ISLANDHOPPING", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 760, new StoryLevelData { Name = "ISLANDHOPPING_VERSUS", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 699, new StoryLevelData { Name = "STUNTOFZEN", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 705, new StoryLevelData { Name = "HUGEMONSTERRALLY_VERSUS", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 939, new StoryLevelData { Name = "DINOPARK", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 903, new StoryLevelData { Name = "DINOPARK_VERSUS", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 609, new StoryLevelData { Name = "EGGARENA", RaceType = RaceType.BATTLE, ScoreboardMode = 0 } },
            { 647, new StoryLevelData { Name = "DINOFIGHT", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 529, new StoryLevelData { Name = "STARFISHIN", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 915, new StoryLevelData { Name = "DINOFIGHT_VERSUS", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 684, new StoryLevelData { Name = "SUGARRUSH", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 849, new StoryLevelData { Name = "DONTBAKEMYKART", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 582, new StoryLevelData { Name = "SUGARRUSH_VERSUS", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 790, new StoryLevelData { Name = "CURRENTEVENTS", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 857, new StoryLevelData { Name = "CURRENTEVENTS_VERSUS", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 715, new StoryLevelData { Name = "CHEWCHOUXTRAIN", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 738, new StoryLevelData { Name = "CHEWCHOUXTRAIN_VERSUS", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 625, new StoryLevelData { Name = "SKYLOOPS", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 959, new StoryLevelData { Name = "SKYLOOPS_VERSUS", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 998, new StoryLevelData { Name = "AIRSHIPS", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 520, new StoryLevelData { Name = "AIRSHIPS_VERSUS", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 881, new StoryLevelData { Name = "TREASUREHUNT", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 828, new StoryLevelData { Name = "TREASUREHUNT_VERSUS", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 712, new StoryLevelData { Name = "CLIFFJAM", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 840, new StoryLevelData { Name = "CARNIVALAFFAIRS", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 624, new StoryLevelData { Name = "CARNIVALAFFAIRS_VERSUS", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 811, new StoryLevelData { Name = "BESTBEFOREDATE", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 823, new StoryLevelData { Name = "CSRSCALE", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 918, new StoryLevelData { Name = "TANKBATTLE", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 614, new StoryLevelData { Name = "BESTBEFOREDATE_VERSUS", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 1049, new StoryLevelData { Name = "TREEHOUSE", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 821, new StoryLevelData { Name = "TREEHOUSE_VERSUS", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 702, new StoryLevelData { Name = "FIREBUGCIRCUIT", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 913, new StoryLevelData { Name = "FIREBUGCIRCUIT_VERSUS", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 766, new StoryLevelData { Name = "EVESOFFROAD", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 606, new StoryLevelData { Name = "FIREBUGBOSS", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 550, new StoryLevelData { Name = "WIPEOUT", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 708, new StoryLevelData { Name = "WIPEOUT_VERSUS", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 648, new StoryLevelData { Name = "JUGGERTEST", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 772, new StoryLevelData { Name = "JUGGERTEST_VERSUS", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 688, new StoryLevelData { Name = "HUGESPACESHIP", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 579, new StoryLevelData { Name = "HUGESPACESHIP_VERSUS", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 539, new StoryLevelData { Name = "CTFGOLF", RaceType = RaceType.BATTLE, ScoreboardMode = 0 } },
            { 698, new StoryLevelData { Name = "CIRCUITCIRCUIT", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 759, new StoryLevelData { Name = "CIRCUITCIRCUIT_VERSUS", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 763, new StoryLevelData { Name = "RIDESCROLLER", RaceType = RaceType.BATTLE, ScoreboardMode = 0 } },
            { 941, new StoryLevelData { Name = "DRUMSMASH", RaceType = RaceType.RACE, ScoreboardMode = 0 } },
            { 630, new StoryLevelData { Name = "DRUMSMASH_VERSUS", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 697, new StoryLevelData { Name = "FUNKHOLE", RaceType = RaceType.BATTLE, ScoreboardMode = 0 } },
            { 808, new StoryLevelData { Name = "FUNKHOLE_VERSUS", RaceType = RaceType.BATTLE, ScoreboardMode = 1 } },
            { 1023, new StoryLevelData { Name = "PINBALL", RaceType = RaceType.BATTLE, ScoreboardMode = 0 } },
            { 703, new StoryLevelData { Name = "ENDGAMEBOSS", RaceType = RaceType.BATTLE, ScoreboardMode = 0 } },

            //MNR
            { 288, new StoryLevelData { Name = "T1_MODCIRCUIT", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 289, new StoryLevelData { Name = "T1_MOUNTAINJUMPS", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 291, new StoryLevelData { Name = "T1_RURALFARM", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 292, new StoryLevelData { Name = "T4_TEMPLEOFTIKI", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 293, new StoryLevelData { Name = "T4_SPEEDJUNGLE", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 294, new StoryLevelData { Name = "T4_MOTOISLANd", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 296, new StoryLevelData { Name = "T3_OCEANCLIFF", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 297, new StoryLevelData { Name = "T4_RUMBLEISLAND", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 298, new StoryLevelData { Name = "T2_DIRECLIFF", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 299, new StoryLevelData { Name = "T1_FLAMINGJUMPS", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 302, new StoryLevelData { Name = "T1_MODFINALE", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 303, new StoryLevelData { Name = "T4_OVERVOLCANO", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 304, new StoryLevelData { Name = "T2_SINKHOLE", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 305, new StoryLevelData { Name = "T3_MARINA", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 306, new StoryLevelData { Name = "T4_RICKETYBRIDGE", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 307, new StoryLevelData { Name = "T3_OLDDISTRICT", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 309, new StoryLevelData { Name = "T1_DRIFTPARADISE", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 310, new StoryLevelData { Name = "T2_UPHEAVAL", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 311, new StoryLevelData { Name = "T1_MODOBAHN", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 312, new StoryLevelData { Name = "T1_SPEEDSPRINGS", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 314, new StoryLevelData { Name = "T3_CRAGGYHILLS", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 315, new StoryLevelData { Name = "T3_VISTAPOINT", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 316, new StoryLevelData { Name = "T3_BOARDWALK", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 331, new StoryLevelData { Name = "T2_MINERSRIFT", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 335, new StoryLevelData { Name = "T1_PARKCIRCUIT", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 336, new StoryLevelData { Name = "T1_VILLAGEJUMPS", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 339, new StoryLevelData { Name = "T2_MARKETRUN", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 341, new StoryLevelData { Name = "T2_SANDSTORM", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 595, new StoryLevelData { Name = "T6_IceBreaker", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 597, new StoryLevelData { Name = "T6_DownhillPeak", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 602, new StoryLevelData { Name = "T6_SnowStormCity", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 626, new StoryLevelData { Name = "T5_TerracedDrop", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 627, new StoryLevelData { Name = "T5_Chinatown", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 628, new StoryLevelData { Name = "T5_GreatWall", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 629, new StoryLevelData { Name = "T5_HiddenForest", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 631, new StoryLevelData { Name = "T5_Fortress", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 633, new StoryLevelData { Name = "T7_GridlockRiver", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 634, new StoryLevelData { Name = "T7_SubwayMayhem", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 635, new StoryLevelData { Name = "T7_TouristTrap", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 636, new StoryLevelData { Name = "T7_SkyscraperHop", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 637, new StoryLevelData { Name = "T7_HomerunCircuit", RaceType = RaceType.RACE, ScoreboardMode = 1 } },

            //MNR: Road Trip
            //TODO: Add road trip levels
        };

        public static void CheckStoryLevelName(Database database, int id)
        {
            if (!StoryLevels.ContainsKey(id)) return;
            var creation = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == id);
            if (creation != null && creation.Name != StoryLevels[id].Name)
            {
                creation.Name = StoryLevels[id].Name;
                database.SaveChanges();
            }
        }

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
            else CheckStoryLevelName(database, id);
        }
    }
}
