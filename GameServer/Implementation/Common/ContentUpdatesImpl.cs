using GameServer.Models.Config;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Request;
using GameServer.Models.Response;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using System;
using GameServer.Models;
using GameServer.Utils;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using GameServer.Models.Common;

namespace GameServer.Implementation.Common
{
    public class ContentUpdatesImpl
    {
        public static string GetLatest(Database database, Platform platform, ContentUpdateType content_update_type, string serverURL)
        {
            var resp = new Response<List<ContentUpdate>>
            {
                status = new ResponseStatus { id = -404, message = "Not Found" },
                response = []
            };

            if (content_update_type == ContentUpdateType.ROM_STATUES)   // TODO: HasFlag?
            {
                resp.status.id = 0;
                resp.status.message = "Successful completion";
                resp.response.Add(new ContentUpdate
                {
                    AvailableDate = "2011-06-28T00:00:00+00:00",
                    ContentUpdateType = content_update_type.ToString(),
                    CreatedAt = "2011-06-28T21:57:57+00:00",
                    DataMd5 = "4c1206b2e920e279bcb5cf6e600faeb5",
                    Description = "Backburn Mod and Kart",
                    HasBeenUploaded = true,
                    Id = 10541,
                    Name = "Bckburn",
                    Platform = "PS3",
                    UpdatedAt = "2011-06-28T21:57:57+00:00",
                    Uuid = "aec45604-a1d1-11e0-9406-1231390081e2",
                    ContentUrl = $"{serverURL}/content_updates/10541/data.bin",
                    Data = "PGRsY19zdGF0dWVzPiAKICA8ZGxjX3N0YXR1ZSBpZD0iMTIyNiIgdHlwZT0iQ0hBUkFDVEVSIiAvPiAKICA8ZGxjX3N0YXR1ZSBpZD0iMTE3IiB0eXBlPSJLQVJUIiAvPiAKPC9kbGNfc3RhdHVlcz4A"
                });
            }

            if (content_update_type == ContentUpdateType.HOT_SEAT_PLAYLIST)
            {
                resp.status.id = 0;
                resp.status.message = "Successful completion";
                resp.response.Add(new ContentUpdate
                {
                    AvailableDate = DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    ContentUpdateType = content_update_type.ToString(),
                    CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    DataMd5 = BitConverter.ToString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(GetHotlapData(database)))).Replace("-", "").ToLower(),
                    Description = "",
                    HasBeenUploaded = true,
                    Id = 10542,
                    Name = "",
                    Platform = "PS3",
                    UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    Uuid = Guid.NewGuid().ToString(),
                    ContentUrl = "",
                    Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(GetHotlapData(database)))
                });
            }

            if (content_update_type == ContentUpdateType.THEMED_EVENTS)
            {
                resp.status.id = 0;
                resp.status.message = "Successful completion";
                resp.response.Add(new ContentUpdate
                {
                    AvailableDate = DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    ContentUpdateType = content_update_type.ToString(),
                    CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    DataMd5 = BitConverter.ToString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(GetTopTracksData(database)))).Replace("-", "").ToLower(),
                    Description = "",
                    HasBeenUploaded = true,
                    Id = 10543,
                    Name = "",
                    Platform = "PS3",
                    UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    Uuid = Guid.NewGuid().ToString(),
                    ContentUrl = "",
                    Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(GetTopTracksData(database)))
                });
            }

            return resp.Serialize();
        }

        private static string GetHotlapData(Database database)
        {
            Random random = new();
            var creations = database.PlayerCreations.Where(match => match.Type == PlayerCreationType.TRACK && match.IsMNR && match.Platform == Platform.PS3).ToList();
            PlayerCreationData creation = null;

            HotLapData hotLap = null;
            if (File.Exists("./hotlap.json"))
                hotLap = JsonConvert.DeserializeObject<HotLapData>(File.ReadAllText("./hotlap.json"));
            else
            {
                hotLap = creations.Count != 0 ? new HotLapData { TrackId = creations[random.Next(0, creations.Count - 1)].TrackId, SelectedAt = DateTime.UtcNow } : null;
                if (hotLap != null)
                    File.WriteAllText("./hotlap.json", JsonConvert.SerializeObject(hotLap));
            }

            if (hotLap != null && hotLap.SelectedAt < DateTime.UtcNow.AddDays(-1))
            {
                foreach (var score in database.Scores.Where(match => match.SubGroupId == 700 && match.IsMNR).ToList())
                {
                    database.Scores.Remove(score);
                }
                database.SaveChanges();
                hotLap = new HotLapData { TrackId = creations[random.Next(0, creations.Count - 1)].TrackId, SelectedAt = DateTime.UtcNow };
                File.WriteAllText("./hotlap.json", JsonConvert.SerializeObject(hotLap));
            }

            if (hotLap != null)
                creation = creations.FirstOrDefault(match => match.Id == hotLap.TrackId);

            var result = new XElement("events",
                new XElement("event",
                    new XAttribute("name", creation != null ? creation.Name : "T1_ModCircuit"),
                    new XAttribute("id", creation != null ? creation.Id : 288),
                    new XAttribute("laps", "1"),
                    new XAttribute("description", (creation != null && creation.Description != null) ? creation.Description : "")
                )
            ).ToString();

            return result;
        }

        private static string GetTopTracksData(Database database)
        {
            var creations = database.PlayerCreations
                .Where(match => match.Type == PlayerCreationType.TRACK && match.IsMNR && match.Platform == Platform.PS3)
                .OrderBy(match => match.Points.Count())
                .ToList();

            var result = "";

            var xmlResult = new XElement("events");
            foreach (var creation in creations)
            {
                xmlResult.Add(new XElement("event",
                    new XAttribute("name", creation.Name),
                    new XAttribute("id", creation.Id),
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
