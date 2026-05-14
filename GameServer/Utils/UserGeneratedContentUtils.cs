using GameServer.Implementation.Storage;
using GameServer.Models;
using GameServer.Models.Config;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace GameServer.Utils
{
    public class UserGeneratedContentUtils
    {
        public static IUGCStorage GetStorage(StorageType type)
        {
            switch (type)
            {
                default:
                    throw new NotImplementedException($"Storage type {type} is not implemented");
                    
                case StorageType.Local:
                    return new LocalStorage();
                
                case StorageType.S3:
                    return new S3Storage();
            }
        }

        public static string GetCDNURL(HttpRequest request)
        {
            string protocol = request.IsHttps ? "https://" : "http://";
            return ServerConfig.Instance.ExternalURL.Replace("auto", $"{protocol}{request.Host.Host}", StringComparison.CurrentCultureIgnoreCase).TrimEnd('/');
        }
        
        public static Stream Resize(byte[] image, int width, int height)
        {
            Image Image = Image.Load(image);

            if (Image.Width == width && Image.Height == height)
                return new MemoryStream(image);

            Image.Mutate(x => x.Resize(width, height));

            var stream = new MemoryStream();
            Image.Save(stream, new PngEncoder());
            stream.Position = 0;
            return stream;
        }

        public static Stream Resize(Stream image, int width, int height)
        {
            image.Position = 0;
            var bytes = new byte[image.Length];
            image.Read(bytes);
            return Resize(bytes, width, height);
        }

        public static string CalculateMD5(Stream stream)
        {
            string hash = BitConverter.ToString(MD5.HashData(stream)).Replace("-", "").ToLower();
            stream.Position = 0;
            return hash;
        }

        public static string CalculateMD5(string data)
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
            return CalculateMD5(stream);
        }

        private static readonly Dictionary<int, StoryLevelData> StoryLevels = new()
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
            { 512, new StoryLevelData { Name = "Tour1Race1", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 513, new StoryLevelData { Name = "Tour1Race2", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 514, new StoryLevelData { Name = "Tour1Race3", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 515, new StoryLevelData { Name = "Tour1Race4", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 516, new StoryLevelData { Name = "Tour1Race5", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 517, new StoryLevelData { Name = "Tour2Race1", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 518, new StoryLevelData { Name = "Tour2Race2", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 519, new StoryLevelData { Name = "Tour2Race3", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            { 521, new StoryLevelData { Name = "Tour2Race5", RaceType = RaceType.RACE, ScoreboardMode = 1 } },
            //TODO: Check if there is more story/dlc tracks that could be reported by the game
        };

        public static void CheckStoryLevelName(Database database, int id)
        {
            if (!StoryLevels.TryGetValue(id, out StoryLevelData storyLevel)) return;
            var creation = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == id);
            if (creation != null && creation.Name != storyLevel.Name)
            {
                creation.Name = storyLevel.Name;
                database.SaveChanges();
            }
        }

        public static void AddStoryLevel(Database database, int id) 
        {
            //genius story level check
            if (!StoryLevels.TryGetValue(id, out StoryLevelData storyLevel)) return;

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
                    ScoreboardMode = storyLevel.ScoreboardMode,
                    Name = storyLevel.Name,
                    Description = "story level",
                    IsRemixable = false,
                    Type = PlayerCreationType.STORY,
                    FirstPublished = DateTime.Parse("2012-11-06"),
                    LastPublished = DateTime.Parse("2012-11-06"),
                    CreatedAt = DateTime.Parse("2012-11-06"),
                    UpdatedAt = DateTime.Parse("2012-11-06"),
                    Platform = Platform.PS3,
                    RaceType = storyLevel.RaceType
                });
                database.SaveChanges();
            }
            else CheckStoryLevelName(database, id);
        }

        public static bool CheckImage(Stream image, int MaxWidth = 0, int MaxHeight = 0)
        {
            ImageInfo info;

            try
            {
                info = Image.Identify(image);
            }
            catch
            {
                image.Position = 0;
                return false;
            }

            image.Position = 0;

            if (info == null 
                || (MaxWidth != 0 && info.Width > MaxWidth) 
                || (MaxHeight != 0 && info.Height > MaxHeight))
                return false;

            return true;
        }
    }
}
