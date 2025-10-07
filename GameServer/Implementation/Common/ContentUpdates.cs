using GameServer.Models;
using GameServer.Models.Config;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Utils;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
                    platform = "PS3",
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
                resp.response.Add(new content_update
                {
                    available_date = DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    content_update_type = content_update_type.ToString(),
                    created_at = DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    data_md5 = BitConverter.ToString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(GetHotlapData(database)))).Replace("-", "").ToLower(),
                    description = "",
                    has_been_uploaded = true,
                    id = 10542,
                    name = "",
                    platform = "PS3",
                    updated_at = DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    uuid = Guid.NewGuid().ToString(),
                    content_url = "",
                    data = Convert.ToBase64String(Encoding.UTF8.GetBytes(GetHotlapData(database)))
                });
            }

            if (content_update_type == ContentUpdateType.THEMED_EVENTS)
            {
                resp.status.id = 0;
                resp.status.message = "Successful completion";
                resp.response.Add(new content_update
                {
                    available_date = DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    content_update_type = content_update_type.ToString(),
                    created_at = DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    data_md5 = BitConverter.ToString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(GetTopTracksData(database)))).Replace("-", "").ToLower(),
                    description = "",
                    has_been_uploaded = true,
                    id = 10543,
                    name = "",
                    platform = "PS3",
                    updated_at = DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    uuid = Guid.NewGuid().ToString(),
                    content_url = "",
                    data = Convert.ToBase64String(Encoding.UTF8.GetBytes(GetTopTracksData(database)))
                });
            }

            return resp.Serialize();
        }

        private static HotLapData GetNewHotLap(Database database)
        {
            Random random = new();

            HotLapData result = null;

            var candidates = database.PlayerCreations
                .Include(p => p.Downloads)
                .OrderByDescending(p => p.Downloads.Count)
                .Where(match => match.Type == PlayerCreationType.TRACK
                    && match.IsMNR && match.Platform == Platform.PS3)
                .Select(p => p.PlayerCreationId);

            int count = candidates.Count();

            if (count > 0)
            {
                int trackid;

                if (count > 1)
                    trackid = candidates.Skip(random.Next(0, count)).FirstOrDefault();
                else
                    trackid = candidates.FirstOrDefault();

                result = new()
                {
                    SelectedAt = DateTime.UtcNow,
                    TrackId = trackid
                };
            }

            ServerCommunication.NotifyHotSeatPlaylistReset();

            return result;
        }

        private static string GetHotlapData(Database database)
        {
            PlayerCreationData creation = null;

            HotLapData hotLap = null;
            if (File.Exists("./hotlap.json"))
                hotLap = JsonConvert.DeserializeObject<HotLapData>(File.ReadAllText("./hotlap.json"));
            else
            {
                hotLap = GetNewHotLap(database);
                if (hotLap != null)
                    File.WriteAllText("./hotlap.json", JsonConvert.SerializeObject(hotLap));
            }

            if (hotLap != null && hotLap.SelectedAt < DateTime.UtcNow.AddDays(-1))
            {
                database.Scores.RemoveRange(database.Scores.Where(match => match.SubGroupId == 700 && match.IsMNR).ToList());

                database.SaveChanges();

                hotLap = GetNewHotLap(database);
                if (hotLap != null)
                    File.WriteAllText("./hotlap.json", JsonConvert.SerializeObject(hotLap));
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
                .Include(p => p.Downloads)
                .OrderByDescending(match => match.Downloads.Count)
                .Where(match => match.Type == PlayerCreationType.TRACK && match.IsMNR && match.Platform == Platform.PS3)
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
                    new XAttribute("description", creation.Description != null ? creation.Description : "")
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
