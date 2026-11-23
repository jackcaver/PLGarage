using GameServer.Models;
using GameServer.Models.Config;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Utils;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GameServer.Implementation.Common
{
    public class ContentUpdates
    {
        public static string GetLatest(Database database, Platform platform, ContentUpdateType content_update_type, string serverURL)
        {
            var resp = new Response<List<content_update>>
            {
                status = new ResponseStatus { id = -404, message = "Not Found" },
                response = []
            };
            string data;

            if (content_update_type == ContentUpdateType.ROM_STATUES)
            {
                resp.status.id = 0;
                resp.status.message = "Successful completion";
                resp.response.Add(new content_update
                {
                    available_date = "2011-06-28T00:00:00+00:00",
                    content_update_type = content_update_type.ToString(),
                    created_at = "2011-06-28T21:57:57+00:00",
                    data_md5 = "4c1206b2e920e279bcb5cf6e600faeb5",
                    description = "Backburn Mod and Kart",
                    has_been_uploaded = true,
                    id = 10541,
                    name = "Bckburn",
                    platform = platform.ToString(),
                    updated_at = "2011-06-28T21:57:57+00:00",
                    uuid = "aec45604-a1d1-11e0-9406-1231390081e2",
                    content_url = $"{serverURL}/content_updates/10541/data.bin",
                    data = "PGRsY19zdGF0dWVzPiAKICA8ZGxjX3N0YXR1ZSBpZD0iMTIyNiIgdHlwZT0iQ0hBUkFDVEVSIiAvPiAKICA8ZGxjX3N0YXR1ZSBpZD0iMTE3IiB0eXBlPSJLQVJUIiAvPiAKPC9kbGNfc3RhdHVlcz4A"
                });
            }

            if (content_update_type == ContentUpdateType.HOT_SEAT_PLAYLIST)
            {
                resp.status.id = 0;
                resp.status.message = "Successful completion";
                data = GetHotlapData(database);
                resp.response.Add(new content_update
                {
                    available_date = TimeUtils.Now.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    content_update_type = content_update_type.ToString(),
                    created_at = TimeUtils.Now.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    data_md5 = UserGeneratedContentUtils.CalculateMD5(data),
                    description = "",
                    has_been_uploaded = true,
                    id = 10542,
                    name = "",
                    platform = platform.ToString(),
                    updated_at = TimeUtils.Now.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    uuid = Guid.NewGuid().ToString(),
                    content_url = "",
                    data = Convert.ToBase64String(Encoding.UTF8.GetBytes(data))
                });
            }

            if (content_update_type == ContentUpdateType.THEMED_EVENTS)
            {
                resp.status.id = 0;
                resp.status.message = "Successful completion";

                data = GetTopTracksData(database);
                
                resp.response.Add(new content_update
                {
                    available_date = TimeUtils.Now.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    content_update_type = content_update_type.ToString(),
                    created_at = TimeUtils.Now.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    data_md5 = UserGeneratedContentUtils.CalculateMD5(data),
                    description = "",
                    has_been_uploaded = true,
                    id = 10543,
                    name = "",
                    platform = platform.ToString(),
                    updated_at = TimeUtils.Now.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    uuid = Guid.NewGuid().ToString(),
                    content_url = "",
                    data = Convert.ToBase64String(Encoding.UTF8.GetBytes(data))
                });
            }

            return resp.Serialize();
        }
        
        public static HotLapData ReadHotlapData()
        {
            if (!File.Exists("./hotlap.json"))
                return null;
            
            try
            {
                return JsonConvert.DeserializeObject<HotLapData>(File.ReadAllText("./hotlap.json"));
            }
            catch (Exception e)
            {
                Log.Error($"Unable to read hotlap file: {e}");
            }
            return null;
        }

        public static void WriteHotlapData(HotLapData hotlap)
        {
            if (hotlap == null)
                return;
            
            try
            {
                File.WriteAllText("./hotlap.json", JsonConvert.SerializeObject(hotlap, Formatting.Indented));
            }
            catch (Exception e)
            {
                Log.Error($"Unable to write hotlap file: {e}");
            }
        }
        
        public static void GetNewHotLap(Database database)
        {
            database.Scores.RemoveRange(database.Scores.Where(match => match.SubGroupId == 700 && match.IsMNR).ToList());
            database.SaveChanges();

            HotLapData hotlap = ReadHotlapData() ?? new HotLapData { TrackId = -1, Queue = [] };
            hotlap.Queue ??= [];

            var query = database.PlayerCreations
                .AsSplitQuery()
                .Include(p => p.DownloadsData)
                .Include(p => p.RatingsData)
                .OrderByDescending(p => p.DownloadsData.Count)
                .Where(match => (match.Type == PlayerCreationType.TRACK || match.Type == PlayerCreationType.STORY)
                    && match.IsMNR
                    && match.AutoReset
                    && match.Platform == Platform.PS3
                    && match.ModerationStatus != ModerationStatus.BANNED
                    && match.ModerationStatus != ModerationStatus.ILLEGAL);

            var candidates = query
                .Where(match => match.Downloads >= 10 && match.Rating >= 3.5)
                .Select(p => p.PlayerCreationId);

            if (!candidates.Any())
                candidates = query.Select(p => p.PlayerCreationId);

            int? picked = null;

            while (picked == null && hotlap.Queue.Count != 0)
            {
                int queued = hotlap.Queue[0];
                hotlap.Queue.RemoveAt(0);

                if (query.Any(match => match.PlayerCreationId == queued))
                {
                    picked = queued;
                    Log.Debug($"New hotlap track picked from queue {queued}");
                }
                else
                    Log.Error($"Queued hotlap track {queued} isn't a valid hotlap track, skipping it");
            }

            if (picked == null)
            {
                int currentId = hotlap.TrackId;
                var pool = candidates.Where(id => id != currentId);
                int count = pool.Count();

                if (count == 0)
                {
                    pool = candidates;
                    count = pool.Count();
                }

                if (count > 0)
                {
                    picked = pool.Skip(Random.Shared.Next(0, count)).FirstOrDefault();
                    Log.Debug($"New hotlap track picked {picked}");
                }
                else
                    Log.Debug("There were no candidates to choose hotlap from");
            }

            if (picked != null)
                hotlap.TrackId = picked.Value;

            WriteHotlapData(hotlap);
            ServerCommunication.NotifyHotSeatPlaylistReset();
        }
        
        private static string GetHotlapData(Database database)
        {
            PlayerCreationData creation = null;

            HotLapData hotLap = ReadHotlapData();

            if (hotLap == null)
            {
                GetNewHotLap(database);
                hotLap = ReadHotlapData();
            }

            if (hotLap != null)
                creation = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == hotLap.TrackId);

            var result = new XElement("events",
                new XElement("event",
                    new XAttribute("name", creation != null ? creation.Name : "T1_ModCircuit"),
                    new XAttribute("id", creation != null ? creation.PlayerCreationId : 288),
                    new XAttribute("laps", "1"),
                    new XAttribute("description", (creation != null && creation.Description != null) ? creation.Description : "")
                )
            ).ToString();

            return result;
        }

        private static string GetTopTracksData(Database database)
        {
            var creations = database.PlayerCreations
                .AsNoTracking()
                .Where(match => match.Type == PlayerCreationType.TRACK
                    && match.IsMNR
                    && match.Platform == Platform.PS3
                    && match.ModerationStatus != ModerationStatus.BANNED
                    && match.ModerationStatus != ModerationStatus.ILLEGAL)
                .OrderByDescending(match => match.PointsYesterday)
                .ThenBy(match => match.PlayerCreationId)
                .Take(5)
                .ToList();

            var result = "";

            var xmlResult = new XElement("events");
            foreach (var creation in creations)
            {
                xmlResult.Add(new XElement("event",
                    new XAttribute("name", creation.Name),
                    new XAttribute("id", creation.PlayerCreationId),
                    new XAttribute("laps", "3"),
                    new XAttribute("description", creation.Description ?? "")
                ));
            }

            if (creations.Count == 0)
            {
                xmlResult.Add(new XElement("event",
                    new XAttribute("name", "T1_ModCircuit"),
                    new XAttribute("id", 288),
                    new XAttribute("laps", "1"),
                    new XAttribute("description", "")
                ));
            }

            result = xmlResult.ToString();

            return result;
        }
    }
}
