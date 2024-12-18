using GameServer.Models;
using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Utils;
using System.Collections.Generic;
using System;
using System.Linq;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Implementation.Common;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace GameServer.Implementation.Player_Creation
{
    public class PlayerCreations
    {
        public static string UpdatePlayerCreation(Database database, Guid SessionID, PlayerCreation PlayerCreation, int id = 0)
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var Creation = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == id && match.PlayerId == user.UserId);

            if (PlayerCreation.player_creation_type == PlayerCreationType.PLANET)
                Creation = database.PlayerCreations.FirstOrDefault(match => match.PlayerId == user.UserId && match.Type == PlayerCreationType.PLANET);

            if (Creation == null && PlayerCreation.player_creation_type == PlayerCreationType.PLANET)
            {
                return CreatePlayerCreation(database, SessionID, PlayerCreation);
            }

            if (Creation == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            Creation.AI = PlayerCreation.ai;
            Creation.AssociatedCoordinates = PlayerCreation.associated_coordinates;
            Creation.AssociatedItemIds = PlayerCreation.associated_item_ids;
            Creation.AssociatedUsernames = PlayerCreation.associated_usernames;
            Creation.AutoReset = PlayerCreation.auto_reset;
            Creation.AutoTags = PlayerCreation.auto_tags;
            Creation.BattleFriendlyFire = PlayerCreation.battle_friendly_fire;
            Creation.BattleKillCount = PlayerCreation.battle_kill_count;
            Creation.BattleTimeLimit = PlayerCreation.battle_time_limit;
            Creation.Description = PlayerCreation.description;
            Creation.Difficulty = PlayerCreation.difficulty;
            Creation.DLCKeys = PlayerCreation.dlc_keys;
            Creation.IsRemixable = PlayerCreation.is_remixable;
            Creation.LastPublished = DateTime.UtcNow;
            Creation.LevelMode = PlayerCreation.level_mode;
            Creation.LongestDrift = PlayerCreation.longest_drift;
            Creation.LongestHangTime = PlayerCreation.longest_hang_time;
            Creation.MaxHumans = PlayerCreation.max_humans;
            Creation.Name = PlayerCreation.name;
            Creation.NumLaps = PlayerCreation.num_laps;
            Creation.NumRacers = PlayerCreation.num_racers;
            Creation.Platform = PlayerCreation.platform;
            Creation.RaceType = PlayerCreation.race_type;
            Creation.RequiresDLC = PlayerCreation.requires_dlc;
            Creation.ScoreboardMode = PlayerCreation.scoreboard_mode;
            Creation.Speed = PlayerCreation.speed;
            Creation.Tags = PlayerCreation.tags;
            Creation.TrackTheme = PlayerCreation.track_theme;
            Creation.Type = PlayerCreation.player_creation_type;
            Creation.UpdatedAt = DateTime.UtcNow;
            Creation.UserTags = PlayerCreation.user_tags;
            Creation.WeaponSet = PlayerCreation.weapon_set;
            Creation.Version++;

            if (Creation.Type == PlayerCreationType.TRACK && !session.IsMNR)
            {
                database.ActivityLog.Add(new ActivityEvent
                {
                    AuthorId = user.UserId,
                    Type = ActivityType.player_creation_event,
                    List = ActivityList.both,
                    Topic = "player_creation_updated",
                    Description = "",
                    PlayerId = 0,
                    PlayerCreationId = Creation.PlayerCreationId,
                    CreatedAt = DateTime.UtcNow,
                    AllusionId = Creation.PlayerCreationId,
                    AllusionType = "PlayerCreation::Track"
                });
            }

            database.SaveChanges();

            if (PlayerCreation.player_creation_type != PlayerCreationType.PLANET)
                UserGeneratedContentUtils.SavePlayerCreation(Creation.PlayerCreationId,
                   PlayerCreation.data.OpenReadStream(),
                   PlayerCreation.preview.OpenReadStream());
            else
                UserGeneratedContentUtils.SavePlayerCreation(Creation.PlayerCreationId, PlayerCreation.data.OpenReadStream());

            if (PlayerCreation.player_creation_type == PlayerCreationType.PLANET)
            {
                var planetUpdateResp = new Response<List<Planet>>
                {
                    status = new ResponseStatus { id = 0, message = "Successful completion" },
                    response = [new Planet { id = id }]
                };
                return planetUpdateResp.Serialize();
            }

            var resp = new Response<List<player_creation>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [new player_creation { id = id }]
            };
            return resp.Serialize();
        }

        public static string CreatePlayerCreation(Database database, Guid SessionID, PlayerCreation Creation)
        {
            var session = Session.GetSession(SessionID);
            int id = database.PlayerCreations.Count(match => match.Type != PlayerCreationType.STORY) + 10000;
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);
            var deletedCreation = database.PlayerCreations.FirstOrDefault(match => match.Type == PlayerCreationType.DELETED 
                && ((match.Name == Creation.player_creation_type.ToString() && match.IsMNR == session.IsMNR
                && match.Platform == session.Platform) || (match.Name == null && !session.IsMNR)));

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            int quota = database.PlayerCreations.Count(match => match.PlayerId == user.UserId 
                && match.Type != PlayerCreationType.PHOTO && match.Type != PlayerCreationType.DELETED 
                && match.IsMNR == session.IsMNR && match.Platform == session.Platform);
            if (quota >= user.Quota && Creation.player_creation_type != PlayerCreationType.PHOTO)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            if (deletedCreation != null)
            {
                id = deletedCreation.PlayerCreationId;
                database.Remove(deletedCreation);
            }
            else
            {
                //Check if id is not used by something...
                bool IsAvailable = false;
                while (!IsAvailable)
                {
                    var check = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == id);
                    IsAvailable = check == null || (check != null && check.Type == PlayerCreationType.DELETED);
                    if (!IsAvailable) id++;
                }
            }

            database.PlayerCreations.Add(new PlayerCreationData
            {
                PlayerCreationId = id,
                AI = Creation.ai,
                AssociatedCoordinates = Creation.associated_coordinates,
                AssociatedItemIds = Creation.associated_item_ids,
                AssociatedUsernames = Creation.associated_usernames,
                AutoReset = Creation.auto_reset,
                AutoTags = Creation.auto_tags,
                BattleFriendlyFire = Creation.battle_friendly_fire,
                BattleKillCount = Creation.battle_kill_count,
                BattleTimeLimit = Creation.battle_time_limit,
                CreatedAt = DateTime.UtcNow,
                Description = Creation.description,
                Difficulty = Creation.difficulty,
                DLCKeys = Creation.dlc_keys,
                FirstPublished = DateTime.UtcNow,
                IsRemixable = Creation.is_remixable,
                IsTeamPick = Creation.is_team_pick,
                LastPublished = DateTime.UtcNow,
                LevelMode = Creation.level_mode,
                LongestDrift = Creation.longest_drift,
                LongestHangTime = Creation.longest_hang_time,
                MaxHumans = Creation.max_humans,
                Name = Creation.name,
                NumLaps = Creation.num_laps,
                NumRacers = Creation.num_racers,
                Platform = Creation.platform,
                PlayerId = user.UserId,
                RaceType = Creation.race_type,
                RequiresDLC = Creation.requires_dlc,
                ScoreboardMode = Creation.scoreboard_mode,
                Speed = Creation.speed,
                Tags = Creation.tags,
                TrackTheme = Creation.track_theme,
                Type = Creation.player_creation_type,
                UpdatedAt = DateTime.UtcNow,
                UserTags = Creation.user_tags,
                WeaponSet = Creation.weapon_set,
                TrackId = Creation.track_id == 0 ? id : Creation.track_id,
                Version = 1,
                //MNR
                IsMNR = session.IsMNR,
                ParentCreationId = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == Creation.parent_creation_id) != null || Creation.parent_creation_id < 10000 ? Creation.parent_creation_id : 0,
                ParentPlayerId = database.Users.FirstOrDefault(match => match.UserId == Creation.parent_player_id) != null ? Creation.parent_player_id == 0 ? Creation.parent_player_id : Creation.parent_player_id : 0,
                OriginalPlayerId = database.Users.FirstOrDefault(match => match.UserId == Creation.original_player_id) != null ? Creation.original_player_id == 0 ? Creation.original_player_id : user.UserId : user.UserId,
                BestLapTime = Creation.best_lap_time
            });

            if (Creation.player_creation_type == PlayerCreationType.TRACK && !session.IsMNR)
            {
                database.ActivityLog.Add(new ActivityEvent
                {
                    AuthorId = user.UserId,
                    Type = ActivityType.player_creation_event,
                    List = ActivityList.both,
                    Topic = "player_creation_created",
                    Description = "",
                    PlayerId = 0,
                    PlayerCreationId = id,
                    CreatedAt = DateTime.UtcNow,
                    AllusionId = id,
                    AllusionType = "PlayerCreation::Track"
                });
            }

            database.SaveChanges();


            if (Creation.player_creation_type == PlayerCreationType.PHOTO)
                UserGeneratedContentUtils.SavePlayerPhoto(id,
                   Creation.data.OpenReadStream());
            else if (Creation.player_creation_type == PlayerCreationType.PLANET)
                UserGeneratedContentUtils.SavePlayerCreation(id, Creation.data.OpenReadStream());
            else
                UserGeneratedContentUtils.SavePlayerCreation(id,
                   Creation.data.OpenReadStream(),
                   Creation.preview.OpenReadStream());

            if (Creation.player_creation_type == PlayerCreationType.PLANET)
            {
                var planetUpdateResp = new Response<List<Planet>>
                {
                    status = new ResponseStatus { id = 0, message = "Successful completion" },
                    response = [new Planet { id = id }]
                };
                return planetUpdateResp.Serialize();
            }

            var resp = new Response<List<player_creation>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [new player_creation { id = id }]
            };
            return resp.Serialize();
        }

        public static string RemovePlayerCreation(Database database, Guid SessionID, int id)
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);

            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var Creation = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == id && match.PlayerId == user.UserId);

            if (Creation == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            database.PlayerCreations.Remove(Creation);

            foreach (var item in database.PlayerCreations.Where(match => match.TrackId == id).ToList())
            {
                var Photo = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == item.PlayerCreationId);
                Photo.TrackId = 4912;
            }

            foreach (var item in database.ActivityLog.Where(match => match.PlayerCreationId == id).ToList())
            {
                var Activity = database.ActivityLog.FirstOrDefault(match => match.Id == item.Id);
                database.ActivityLog.Remove(Activity);
            }

            database.SaveChanges();

            UserGeneratedContentUtils.RemovePlayerCreation(id);

            database.PlayerCreations.Add(new PlayerCreationData
            {
                PlayerCreationId = id,
                Name = Creation.Type.ToString(),
                PlayerId = user.UserId,
                Platform = Creation.Platform,
                Type = PlayerCreationType.DELETED,
                IsMNR = Creation.IsMNR
            });
            database.SaveChanges();

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }

        public static string GetPlayerCreation(Database database, Guid SessionID, int id, bool IsCounted, bool download = false)
        {
            var session = Session.GetSession(SessionID);
            var Creation = database.PlayerCreations
                .Include(x => x.Downloads)
                .Include(x => x.RacesStarted)
                .Include(x => x.UniqueRacers)
                .Include(x => x.Hearts)
                .Include(x => x.Points)
                .Include(x => x.Ratings)
                .Include(x => x.Views)
                .Include(x => x.Author)
                .FirstOrDefault(match => match.PlayerCreationId == id);
            var User = database.Users.FirstOrDefault(match => match.Username == session.Username);

            if (Creation == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            if (User != null)
            {
                if (IsCounted && !download)
                {
                    database.PlayerCreationViews.Add(new PlayerCreationView { PlayerCreationId = Creation.PlayerCreationId, ViewedAt = DateTime.UtcNow });
                    database.SaveChanges();
                }

                if (IsCounted && download)
                {
                    var uniqueRacer = database.PlayerCreationUniqueRacers.FirstOrDefault(match => match.PlayerId == User.UserId);
                    database.PlayerCreationDownloads.Add(new PlayerCreationDownload { PlayerCreationId = Creation.PlayerCreationId, DownloadedAt = DateTime.UtcNow });
                    database.PlayerCreationPoints.Add(new PlayerCreationPoint { PlayerCreationId = Creation.PlayerCreationId, PlayerId = Creation.PlayerId, Platform = Creation.Platform, Type = Creation.Type, CreatedAt = DateTime.UtcNow, Amount = 100 });

                    if (uniqueRacer == null)
                    {
                        database.PlayerCreationUniqueRacers.Add(new PlayerCreationUniqueRacer
                        {
                            PlayerId = User.UserId,
                            PlayerCreationId = Creation.PlayerCreationId,
                            Version = Creation.Version
                        });
                        if (!session.IsMNR)
                        {
                            database.ActivityLog.Add(new ActivityEvent
                            {
                                AuthorId = User.UserId,
                                Type = ActivityType.player_creation_event,
                                List = ActivityList.activity_log,
                                Topic = "player_creation_downloaded",
                                Description = "",
                                PlayerId = 0,
                                PlayerCreationId = Creation.PlayerCreationId,
                                CreatedAt = DateTime.UtcNow,
                                AllusionId = Creation.PlayerCreationId,
                                AllusionType = "PlayerCreation::Track"
                            });
                            database.ActivityLog.Add(new ActivityEvent
                            {
                                AuthorId = User.UserId,
                                Type = ActivityType.player_creation_event,
                                List = ActivityList.activity_log,
                                Topic = "player_creation_played",
                                Description = "",
                                PlayerId = 0,
                                PlayerCreationId = Creation.PlayerCreationId,
                                CreatedAt = DateTime.UtcNow,
                                AllusionId = Creation.PlayerCreationId,
                                AllusionType = "PlayerCreation::Track"
                            });
                        }
                    }

                    if (uniqueRacer != null && uniqueRacer.Version != Creation.Version)
                    {
                        database.PlayerCreationDownloads.Add(new PlayerCreationDownload { PlayerCreationId = Creation.PlayerCreationId, DownloadedAt = DateTime.UtcNow });
                        if (!session.IsMNR)
                        {
                            database.ActivityLog.Add(new ActivityEvent
                            {
                                AuthorId = User.UserId,
                                Type = ActivityType.player_creation_event,
                                List = ActivityList.activity_log,
                                Topic = "player_creation_downloaded",
                                Description = "",
                                PlayerId = 0,
                                PlayerCreationId = Creation.PlayerCreationId,
                                CreatedAt = DateTime.UtcNow,
                                AllusionId = Creation.PlayerCreationId,
                                AllusionType = "PlayerCreation::Track"
                            });
                            database.ActivityLog.Add(new ActivityEvent
                            {
                                AuthorId = User.UserId,
                                Type = ActivityType.player_creation_event,
                                List = ActivityList.activity_log,
                                Topic = "player_creation_played",
                                Description = "",
                                PlayerId = 0,
                                PlayerCreationId = Creation.PlayerCreationId,
                                CreatedAt = DateTime.UtcNow,
                                AllusionId = Creation.PlayerCreationId,
                                AllusionType = "PlayerCreation::Track"
                            });
                        }
                        uniqueRacer.Version = Creation.Version;
                    }

                    database.SaveChanges();
                }
            }

            var allPlayerCreations = database.PlayerCreations           // I dont really know how to optimise this any more without breaking the rank system?
                            .Include(x => x.Points)
                            .Where(match => match.Type == Creation.Type)
                            .OrderBy(match => match.Points.Count())     // TODO: Should this be OrderByDescending?
                            .Select(match => match.PlayerCreationId)    // To optimise the amount of data we get back, this is a particularly tricky situation
                            .AsEnumerable();                            // Evaluate our query, find row index after

            var resp = new Response<List<player_creation>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [
                    new player_creation
                    {
                        id = Creation.PlayerCreationId,
                        ai = Creation.AI,
                        associated_item_ids = Creation.AssociatedItemIds,
                        auto_reset = Creation.AutoReset,
                        battle_friendly_fire = Creation.BattleFriendlyFire,
                        battle_kill_count = Creation.BattleKillCount,
                        battle_time_limit = Creation.BattleTimeLimit,
                        coolness = (Creation.Ratings.Count(match => match.Type == RatingType.YAY) - Creation.Ratings.Count(match => match.Type == RatingType.BOO)) + 
                            ((Creation.RacesStarted.Count() + Creation.RacesFinished) / 2) + Creation.Hearts.Count(),
                        created_at = Creation.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        description = Creation.Description,
                        difficulty = Creation.Difficulty.ToString(),
                        dlc_keys = Creation.DLCKeys != null ? Creation.DLCKeys : "",
                        downloads = Creation.Downloads.Count(),
                        downloads_last_week = Creation.Downloads.Count(match => match.DownloadedAt >= DateTime.UtcNow.AddDays(-14) && match.DownloadedAt <= DateTime.UtcNow.AddDays(-7)),
                        downloads_this_week = Creation.Downloads.Count(match => match.DownloadedAt >= DateTime.UtcNow.AddDays(-7) && match.DownloadedAt <= DateTime.UtcNow),
                        first_published = Creation.FirstPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        last_published = Creation.LastPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        hearts = Creation.Hearts.Count(),
                        is_remixable = Creation.IsRemixable,
                        is_team_pick = Creation.IsTeamPick,
                        level_mode = Creation.LevelMode,
                        longest_drift = Creation.LongestDrift,
                        longest_hang_time = Creation.LongestHangTime,
                        max_humans = Creation.MaxHumans,
                        name = Creation.Name,
                        num_laps = Creation.NumLaps,
                        num_racers = Creation.NumRacers,
                        platform = Creation.Platform.ToString(),
                        player_creation_type = Creation.Type.ToString(),
                        player_id = Creation.PlayerId,
                        races_finished = Creation.RacesFinished,
                        races_started = Creation.RacesStarted.Count(),
                        races_started_this_month = Creation.RacesStarted.Count(match => match.StartedAt >= DateTime.UtcNow.AddMonths(-1) && match.StartedAt <= DateTime.UtcNow),
                        races_started_this_week = Creation.RacesStarted.Count(match => match.StartedAt >= DateTime.UtcNow.AddDays(-7) && match.StartedAt <= DateTime.UtcNow),
                        races_won = Creation.RacesWon,
                        race_type = Creation.RaceType.ToString(),
                        rank = allPlayerCreations
                            .Select((id, idx) => new { id, idx })
                            .Where(row => row.id == Creation.PlayerCreationId)
                            .Select(row => row.idx)
                            .FirstOrDefault(),
                        rating_down = Creation.Ratings.Count(match => match.Type == RatingType.BOO),
                        rating_up = Creation.Ratings.Count(match => match.Type == RatingType.YAY),
                        scoreboard_mode = Creation.ScoreboardMode,
                        speed = Creation.Speed.ToString(),
                        tags = Creation.Tags,
                        track_theme = Creation.TrackTheme,
                        unique_racer_count = Creation.UniqueRacers.Count(),
                        updated_at = Creation.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        username = Creation.Author.Username,
                        user_tags = Creation.UserTags,
                        version = Creation.Version,
                        views = Creation.Views.Count(),
                        views_last_week = Creation.Views.Count(match => match.ViewedAt >= DateTime.UtcNow.AddDays(-14) && match.ViewedAt <= DateTime.UtcNow.AddDays(-7)),
                        views_this_week = Creation.Views.Count(match => match.ViewedAt >= DateTime.UtcNow.AddDays(-7) && match.ViewedAt <= DateTime.UtcNow),
                        votes = Creation.Ratings.Count(match => !Creation.IsMNR || match.Rating != 0),
                        weapon_set = Creation.WeaponSet,
                        data_md5 = download ? UserGeneratedContentUtils.CalculateMD5(id, "data.bin") : null,
                        data_size = download ? UserGeneratedContentUtils.CalculateSize(id, "data.bin").ToString() : null,
                        preview_md5 = download ? UserGeneratedContentUtils.CalculateMD5(id, "preview_image.png") : null,
                        preview_size = download ? UserGeneratedContentUtils.CalculateSize(id, "preview_image.png").ToString() : null,
                        //MNR
                        points = Creation.Points.Sum(p => p.Amount),
                        points_last_week = Creation.Points.Where(match => match.CreatedAt >= DateTime.UtcNow.AddDays(-14) && match.CreatedAt <= DateTime.UtcNow.AddDays(-7)).Sum(p => p.Amount),
                        points_this_week = Creation.Points.Where(match => match.CreatedAt >= DateTime.UtcNow.AddDays(-7) && match.CreatedAt <= DateTime.UtcNow).Sum(p => p.Amount),
                        points_today = Creation.Points.Where(match => match.CreatedAt >= DateTime.UtcNow.Date && match.CreatedAt <= DateTime.UtcNow).Sum(p => p.Amount),
                        points_yesterday = Creation.Points.Where(match => match.CreatedAt >= DateTime.UtcNow.AddDays(-1).Date && match.CreatedAt <= DateTime.UtcNow.Date).Sum(p => p.Amount),
                        rating = (Creation.Ratings.Count() != 0 ? (float)Creation.Ratings.Average(r => r.Rating) : 0).ToString("0.0", CultureInfo.InvariantCulture),
                        star_rating = (Creation.Ratings.Count() != 0 ? (float)Creation.Ratings.Average(r => r.Rating) : 0).ToString("0.0", CultureInfo.InvariantCulture),
                        original_player_id = Creation.OriginalPlayerId,
                        original_player_username = database.Users.FirstOrDefault(match => match.UserId == Creation.OriginalPlayerId) != null ? database.Users.FirstOrDefault(match => match.UserId == Creation.OriginalPlayerId).Username : "",
                        parent_creation_id = Creation.ParentCreationId != 0 ? Creation.ParentCreationId.ToString() : "",
                        parent_creation_name = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == Creation.ParentCreationId) != null ? database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == Creation.ParentCreationId).Name : "",
                        parent_player_id = Creation.ParentPlayerId != 0 ? Creation.ParentPlayerId.ToString() : "",
                        parent_player_username = database.Users.FirstOrDefault(match => match.UserId == Creation.ParentPlayerId) != null ? database.Users.FirstOrDefault(match => match.UserId == Creation.ParentPlayerId).Username : "",
                        best_lap_time = Creation.BestLapTime,
                        moderation_status = Creation.ModerationStatus.ToString(),
                        moderation_status_id = (int)Creation.ModerationStatus,
                    }
                ]
            };
            return resp.Serialize();
        }

        public static string PlayerCreationsFriendsPublished(Database database, string usernameFilter, PlayerCreationType type)
        {
            var Creations = new List<PlayerCreationData> { };

            if (usernameFilter != null)
            {
                foreach (string username in usernameFilter.Split(','))
                {
                    var user = database.Users.FirstOrDefault(match => match.Username == username);
                    if (user != null)
                    {
                        var userTracks = database.PlayerCreations.Where(match => match.PlayerId == user.UserId
                            && match.Type == type).ToList();
                        if (userTracks != null)
                            Creations.AddRange(userTracks);
                    }
                }
            }

            var resp = new Response<List<player_creations>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [new player_creations { friends_published = Creations.Count != 0 }]
            };
            return resp.Serialize();
        }

        public static string SearchPlayerCreations(Database database, Guid SessionID, int page, int per_page, SortColumn sort_column, SortOrder sort_order,
            int limit, Platform platform, Filters filters, string keyword = null, bool TeamPicks = false, 
            bool LuckyDip = false, bool IsMNR = false)
        {
            IQueryable<PlayerCreationData> creationQuery = database.PlayerCreations     // TODO: Is it an issue someone might be able to fudge the entire database out like this?
                .Include(x => x.Downloads)
                .Include(x => x.RacesStarted)
                .Include(x => x.UniqueRacers)
                .Include(x => x.Hearts)
                .Include(x => x.Points)
                .Include(x => x.Ratings)
                .Include(x => x.Views)
                .Include(x => x.Author);
            var session = Session.GetSession(SessionID);
            var User = database.Users.FirstOrDefault(match => match.Username == session.Username);

            if (filters.username == null && filters.id == null && filters.player_id == null)
                creationQuery = creationQuery.Where(match => match.Type == filters.player_creation_type && match.Platform == platform 
                    && match.IsMNR == IsMNR);

            //filters
            if (filters.username != null)
            {
                foreach (string username in filters.username)
                    creationQuery = creationQuery.Where(match => match.Author.Username == username);   // TODO: Optimise
            }

            if (filters.id != null)
            {
                foreach (string id in filters.id)
                    creationQuery = creationQuery.Where(match => match.PlayerCreationId.ToString() == id);   // TODO: Optimise and figure out PlayerCreationType.STORY
            }

            if (filters.player_id != null)
            {
                foreach (string player_id in filters.player_id)
                    creationQuery = creationQuery.Where(match => match.PlayerId.ToString() == player_id);   // TODO: Optimise
            }

            creationQuery = creationQuery.Where(match => match.ModerationStatus != ModerationStatus.BANNED  // Change to == APPROVED?
                && match.ModerationStatus != ModerationStatus.ILLEGAL);

            if (keyword != null)
                creationQuery = creationQuery.Where(match => match.Name.Contains(keyword));

            if (filters.race_type != null)
                creationQuery = creationQuery.Where(match => filters.race_type.Equals(match.RaceType.ToString()));

            if (filters.tags != null && filters.tags.Length != 0)   // !!! THE BELOW WONT COMPILE !!!! TODO: Investigate
                creationQuery = creationQuery.Where(match => match.Tags != null && filters.tags.All(x => match.Tags.Contains(x)));  // We have removed a split here but should do the same job without

            if (filters.auto_reset != null)
                creationQuery = creationQuery.Where(match => match.AutoReset == filters.auto_reset);

            if (filters.ai != null)
                creationQuery = creationQuery.Where(match => match.AI == filters.ai);

            if (filters.is_remixable != null)
                creationQuery = creationQuery.Where(match => match.IsRemixable == filters.is_remixable);

            if (TeamPicks)
                creationQuery = creationQuery.Where(match => match.IsTeamPick);

            // Only filter I havent figured out a way to translate into SQL yet, this might also cause performance issues?
            //if (User != null && !User.ShowCreationsWithoutPreviews)
            //    creations.RemoveAll(match => !match.HasPreview);

            switch (sort_column)
            {
                //cool levels
                case SortColumn.coolness:
                    creationQuery = creationQuery.OrderBy(match => (match.Ratings.Count(match => match.Type == RatingType.YAY) - match.Ratings.Count(match => match.Type == RatingType.BOO)) +
                            ((match.RacesStarted.Count() + match.RacesFinished) / 2) + match.Hearts.Count());
                    break;

                //newest levels
                case SortColumn.created_at:
                    creationQuery = creationQuery.OrderBy(match => match.CreatedAt);
                    break;

                //most played
                case SortColumn.races_started:
                    creationQuery = creationQuery.OrderBy(match => match.RacesStarted.Count());
                    break;
                case SortColumn.races_started_this_week:
                    creationQuery = creationQuery.OrderBy(match => match.RacesStarted.Count(match => match.StartedAt >= DateTime.UtcNow.AddDays(-7) && match.StartedAt <= DateTime.UtcNow));
                    break;
                case SortColumn.races_started_this_month:
                    creationQuery = creationQuery.OrderBy(match => match.RacesStarted.Count(match => match.StartedAt >= DateTime.UtcNow.AddMonths(-1) && match.StartedAt <= DateTime.UtcNow));
                    break;

                //highest rated
                case SortColumn.rating_up:
                    creationQuery = creationQuery.OrderBy(match => match.Ratings.Count(match => match.Type == RatingType.YAY));
                    break;
                case SortColumn.rating_up_this_week:
                    creationQuery = creationQuery.OrderBy(match => match.Ratings.Count(match => match.Type == RatingType.YAY && match.RatedAt >= DateTime.UtcNow.AddDays(-7) && match.RatedAt <= DateTime.UtcNow));
                    break;
                case SortColumn.rating_up_this_month:
                    creationQuery = creationQuery.OrderBy(match => match.Ratings.Count(match => match.Type == RatingType.YAY && match.RatedAt >= DateTime.UtcNow.AddMonths(-1) && match.RatedAt <= DateTime.UtcNow));
                    break;

                //most hearted
                case SortColumn.hearts:
                    creationQuery = creationQuery.OrderBy(match => match.Hearts.Count());
                    break;
                case SortColumn.hearts_this_week:
                    creationQuery = creationQuery.OrderBy(match => match.Hearts.Count(match => match.HeartedAt >= DateTime.UtcNow.AddDays(-7) && match.HeartedAt <= DateTime.UtcNow));
                    break;
                case SortColumn.hearts_this_month:
                    creationQuery = creationQuery.OrderBy(match => match.Hearts.Count(match => match.HeartedAt >= DateTime.UtcNow.AddMonths(-1) && match.HeartedAt <= DateTime.UtcNow));
                    break;

                //MNR
                case SortColumn.rating:
                    creationQuery = creationQuery.OrderBy(match => match.Ratings.Count() != 0 ? (float)match.Ratings.Average(r => r.Rating) : 0);   // Can we simplify this and remove the count check?
                    break;

                //points
                case SortColumn.points:
                    creationQuery = creationQuery.OrderBy(match => match.Points.Sum(p => p.Amount));
                    break;
                case SortColumn.points_today:
                    creationQuery = creationQuery.OrderBy(match => match.Points.Where(match => match.CreatedAt >= DateTime.UtcNow.Date && match.CreatedAt <= DateTime.UtcNow).Sum(p => p.Amount));
                    break;
                case SortColumn.points_yesterday:
                    creationQuery = creationQuery.OrderBy(match => match.Points.Where(match => match.CreatedAt >= DateTime.UtcNow.AddDays(-1).Date && match.CreatedAt <= DateTime.UtcNow.Date).Sum(p => p.Amount));
                    break;
                case SortColumn.points_this_week:
                    creationQuery = creationQuery.OrderBy(match => match.Points.Where(match => match.CreatedAt >= DateTime.UtcNow.AddDays(-7) && match.CreatedAt <= DateTime.UtcNow).Sum(p => p.Amount));
                    break;
                case SortColumn.points_last_week:
                    creationQuery = creationQuery.OrderBy(match => match.Points.Where(match => match.CreatedAt >= DateTime.UtcNow.AddDays(-14) && match.CreatedAt <= DateTime.UtcNow.AddDays(-7)).Sum(p => p.Amount));
                    break;

                //download
                case SortColumn.downloads:
                    creationQuery = creationQuery.OrderBy(match => match.Downloads.Count());
                    break;
                case SortColumn.downloads_this_week:
                    creationQuery = creationQuery.OrderBy(match => match.Downloads.Count(match => match.DownloadedAt >= DateTime.UtcNow.AddDays(-7) && match.DownloadedAt <= DateTime.UtcNow));
                    break;
                case SortColumn.downloads_last_week:
                    creationQuery = creationQuery.OrderBy(match => match.Downloads.Count(match => match.DownloadedAt >= DateTime.UtcNow.AddDays(-14) && match.DownloadedAt <= DateTime.UtcNow.AddDays(-7)));
                    break;

                //views
                case SortColumn.views:
                    creationQuery = creationQuery.OrderBy(match => match.Views.Count());
                    break;
                case SortColumn.views_this_week:
                    creationQuery = creationQuery.OrderBy(match => match.Views.Count(match => match.ViewedAt >= DateTime.UtcNow.AddDays(-7) && match.ViewedAt <= DateTime.UtcNow));
                    break;
                case SortColumn.views_last_week:
                    creationQuery = creationQuery.OrderBy(match => match.Views.Count(match => match.ViewedAt >= DateTime.UtcNow.AddDays(-14) && match.ViewedAt <= DateTime.UtcNow.AddDays(-7)));
                    break;
            }

            if (LuckyDip)
                creationQuery = creationQuery.OrderBy(match => EF.Functions.Random());  // TODO: is session.RandomSeed required? Will this even add onto the above, needs ThenBy?

            var total = creationQuery.Count();

            //calculating pages
            int pageEnd = PageCalculator.GetPageEnd(page, per_page);
            int pageStart = PageCalculator.GetPageStart(page, per_page);
            int totalPages = PageCalculator.GetTotalPages(per_page, total);

            if (pageEnd > total)
                pageEnd = total;

            var creations = creationQuery
                .Skip(pageStart)
                .Take(pageEnd - pageStart)
                .ToList();

            var allPlayerCreations = database.PlayerCreations           // I dont really know how to optimise this any more without breaking the rank system?
                            .Include(x => x.Points)
                            .Where(match => match.Type == filters.player_creation_type)
                            .OrderBy(match => match.Points.Count())     // TODO: Should this be OrderByDescending?
                            .Select(match => match.PlayerCreationId)    // To optimise the amount of data we get back, this is a particularly tricky situation
                            .AsEnumerable();                            // Evaluate our query, find row index after

            var playerCreationsList = new List<player_creation>(
                creations.Select(creation => new player_creation
                {
                    id = creation.PlayerCreationId,
                    ai = creation.AI,
                    associated_item_ids = creation.AssociatedItemIds,
                    auto_reset = creation.AutoReset,
                    battle_friendly_fire = creation.BattleFriendlyFire,
                    battle_kill_count = creation.BattleKillCount,
                    battle_time_limit = creation.BattleTimeLimit,
                    coolness = (creation.Ratings.Count(match => match.Type == RatingType.YAY) - creation.Ratings.Count(match => match.Type == RatingType.BOO)) +
                            ((creation.RacesStarted.Count() + creation.RacesFinished) / 2) + creation.Hearts.Count(),
                    created_at = creation.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    description = creation.Description,
                    difficulty = creation.Difficulty.ToString(),
                    dlc_keys = creation.DLCKeys,
                    downloads = creation.Downloads.Count(),
                    downloads_last_week = creation.Downloads.Count(match => match.DownloadedAt >= DateTime.UtcNow.AddDays(-14) && match.DownloadedAt <= DateTime.UtcNow.AddDays(-7)),
                    downloads_this_week = creation.Downloads.Count(match => match.DownloadedAt >= DateTime.UtcNow.AddDays(-7) && match.DownloadedAt <= DateTime.UtcNow),
                    first_published = creation.FirstPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    last_published = creation.LastPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    hearts = creation.Hearts.Count(),
                    is_remixable = creation.IsRemixable,
                    is_team_pick = creation.IsTeamPick,
                    level_mode = creation.LevelMode,
                    longest_drift = creation.LongestDrift,
                    longest_hang_time = creation.LongestHangTime,
                    max_humans = creation.MaxHumans,
                    name = creation.Name,
                    num_laps = creation.NumLaps,
                    num_racers = creation.NumRacers,
                    platform = creation.Platform.ToString(),
                    player_creation_type = (creation.Type == PlayerCreationType.STORY) ? PlayerCreationType.TRACK.ToString() : creation.Type.ToString(),
                    player_id = creation.PlayerId,
                    races_finished = creation.RacesFinished,
                    races_started = creation.RacesStarted.Count(),
                    races_started_this_month = creation.RacesStarted.Count(match => match.StartedAt >= DateTime.UtcNow.AddMonths(-1) && match.StartedAt <= DateTime.UtcNow),
                    races_started_this_week = creation.RacesStarted.Count(match => match.StartedAt >= DateTime.UtcNow.AddDays(-7) && match.StartedAt <= DateTime.UtcNow),
                    races_won = creation.RacesWon,
                    race_type = creation.RaceType.ToString(),
                    rank = allPlayerCreations
                            .Select((id, idx) => new { id, idx })
                            .Where(row => row.id == creation.PlayerCreationId)
                            .Select(row => row.idx)
                            .FirstOrDefault(),
                    rating_down = creation.Ratings.Count(match => match.Type == RatingType.BOO),
                    rating_up = creation.Ratings.Count(match => match.Type == RatingType.YAY),
                    scoreboard_mode = creation.ScoreboardMode,
                    speed = creation.Speed.ToString(),
                    tags = creation.Tags,
                    track_theme = creation.TrackTheme,
                    unique_racer_count = creation.UniqueRacers.Count(),
                    updated_at = creation.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    username = creation.Author.Username,
                    user_tags = creation.UserTags,
                    version = creation.Version,
                    views = creation.Views.Count(),
                    views_last_week = creation.Views.Count(match => match.ViewedAt >= DateTime.UtcNow.AddDays(-14) && match.ViewedAt <= DateTime.UtcNow.AddDays(-7)),
                    views_this_week = creation.Views.Count(match => match.ViewedAt >= DateTime.UtcNow.AddDays(-7) && match.ViewedAt <= DateTime.UtcNow),
                    votes = creation.Ratings.Count(match => !IsMNR || match.Rating != 0),
                    weapon_set = creation.WeaponSet,
                    //MNR
                    points = creation.Points.Count(),
                    points_last_week = creation.Points.Where(match => match.CreatedAt >= DateTime.UtcNow.AddDays(-14) && match.CreatedAt <= DateTime.UtcNow.AddDays(-7)).Sum(p => p.Amount),
                    points_this_week = creation.Points.Where(match => match.CreatedAt >= DateTime.UtcNow.AddDays(-7) && match.CreatedAt <= DateTime.UtcNow).Sum(p => p.Amount),
                    points_today = creation.Points.Where(match => match.CreatedAt >= DateTime.UtcNow.Date && match.CreatedAt <= DateTime.UtcNow).Sum(p => p.Amount),
                    points_yesterday = creation.Points.Where(match => match.CreatedAt >= DateTime.UtcNow.AddDays(-1).Date && match.CreatedAt <= DateTime.UtcNow.Date).Sum(p => p.Amount),
                    rating = (creation.Ratings.Count() != 0 ? (float)creation.Ratings.Average(r => r.Rating) : 0).ToString("0.0", CultureInfo.InvariantCulture),
                    star_rating = (creation.Ratings.Count() != 0 ? (float)creation.Ratings.Average(r => r.Rating) : 0).ToString("0.0", CultureInfo.InvariantCulture),
                    moderation_status = creation.ModerationStatus.ToString(),
                    moderation_status_id = (int)creation.ModerationStatus
                }));

            var resp = new Response<List<player_creations>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [
                    new player_creations
                    {
                        page = page,
                        row_end = pageEnd,
                        row_start = pageStart,
                        total = total,
                        total_pages = totalPages,
                        PlayerCreationsList = playerCreationsList
                    }
                ]
            };
            return resp.Serialize();
        }

        public static string Mine(Database database, Guid SessionID, int page, int per_page, SortColumn sort_column, SortOrder sort_order,
            int limit, Filters filters, string keyword = null, Platform? platformOverride = null) 
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);
            if (user == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            Platform platform = session.Platform;
            if (platformOverride != null)
                platform = (Platform)platformOverride;

            filters.player_id = new string[1] { user.UserId.ToString() };
            return SearchPlayerCreations(database, SessionID, page, per_page, sort_column, sort_order, limit, platform, filters, keyword, false, false, true);
        }

        public static string SearchPhotos(Database database, int? track_id, string username, string associated_usernames, int page, int per_page)
        {
            IQueryable<PlayerCreationData> photosQuery = database.PlayerCreations
                                                            .Include(x => x.Author)
                                                            .Where(match => match.Type == PlayerCreationType.PHOTO);
            int player_id = 0;
            var user = database.Users.FirstOrDefault(match => match.Username == username);
            if (user != null)
                player_id = user.UserId;

            if (associated_usernames != null)
                photosQuery = photosQuery.Where(match => match.AssociatedUsernames.Contains(associated_usernames));
            if (username != null)
                photosQuery = photosQuery.Where(match => match.PlayerId == player_id);
            if (track_id != null)
                photosQuery = photosQuery.Where(match => match.TrackId == track_id);

            photosQuery = photosQuery.OrderBy(match => match.CreatedAt);

            var total = photosQuery.Count();

            int pageEnd = PageCalculator.GetPageEnd(page, per_page);
            int pageStart = PageCalculator.GetPageStart(page, per_page);
            int totalPages = PageCalculator.GetTotalPages(per_page, total);

            if (pageEnd > total)
                pageEnd = total;

            var photos = photosQuery
                .Skip(pageStart)
                .Take(pageEnd - pageStart)
                .ToList();

            var PhotoList = new List<Photo>(photos.Select(photo => new Photo
            {
                associated_usernames = photo.AssociatedUsernames,
                id = photo.PlayerCreationId,
                track_id = photo.TrackId,
                username = photo.Author.Username
            }));

            var resp = new Response<List<Photos>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [
                    new Photos {
                        total = total,
                        current_page = page,
                        row_start = pageStart,
                        row_end = pageEnd,
                        total_pages = totalPages,
                        PhotoList = PhotoList
                    }
                ]
            };
            return resp.Serialize();
        }

        public static string GetTrackProfile(Database database, Guid SessionID, int id)
        {
            // TODO: Includes
            var session = Session.GetSession(SessionID);
            var Track = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == id);
            var requestedBy = database.Users.FirstOrDefault(match => match.Username == session.Username);
            var TrackPhotos = database.PlayerCreations.Where(match => match.TrackId == id && match.Type == PlayerCreationType.PHOTO).ToList();
            var TrackScores = database.Scores.Where(match => match.SubKeyId == id).ToList();
            var TrackComments = database.PlayerCreationComments.Where(match => match.PlayerCreationId == id).ToList();
            var TrackReviews = database.PlayerCreationReviews.Where(match => match.PlayerCreationId == id).ToList();
            var TrackActivity = database.ActivityLog.Where(match => match.PlayerCreationId == id).ToList();
            List<Photo> PhotoList = [];
            List<SubLeaderboardPlayer> ScoresList = [];
            List<Comment> CommentsList = [];
            List<Review> ReviewsList = [];
            List<Activity> ActivityList = [];

            TrackPhotos.Sort((curr, prev) => prev.CreatedAt.CompareTo(curr.CreatedAt));
            TrackComments.Sort((curr, prev) => prev.CreatedAt.CompareTo(curr.CreatedAt));
            TrackReviews.Sort((curr, prev) => prev.CreatedAt.CompareTo(curr.CreatedAt));
            TrackActivity.Sort((curr, prev) => prev.CreatedAt.CompareTo(curr.CreatedAt));

            if (Track == null || id < 9000)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            if (Track.ScoreboardMode == 1)
                TrackScores.Sort((curr, prev) => curr.FinishTime.CompareTo(prev.FinishTime));
            else
                TrackScores.Sort((curr, prev) => prev.Points.CompareTo(curr.Points));

            foreach (PlayerCreationData Photo in TrackPhotos.Take(3))
            {
                PhotoList.Add(new Photo
                {
                    id = Photo.PlayerCreationId
                });
            }

            foreach (Score Score in TrackScores.Take(3))
            {
                ScoresList.Add(new SubLeaderboardPlayer
                {
                    player_id = Score.PlayerId,
                    username = Score.Username,
                    rank = Score.GetRank(Track.ScoreboardMode == 1 ? SortColumn.finish_time : SortColumn.score),
                    score = Score.Points,
                    finish_time = Score.FinishTime
                });
            }

            foreach (PlayerCreationCommentData Comment in TrackComments.Take(3))
            {
                if (Comment != null)
                {
                    CommentsList.Add(new Comment
                    {
                        id = Comment.Id,
                        player_id = Comment.PlayerId,
                        username = Comment.Username,
                        body = Comment.Body,
                        rating_up = 0,
                        rated_by_me = false,
                        updated_at = Comment.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz")
                    });
                }
            }

            foreach (PlayerCreationReview Review in TrackReviews.Take(3))
            {
                if (Review != null)
                {
                    ReviewsList.Add(new Review
                    {
                        id = Review.Id,
                        content = Review.Content,
                        mine = "false",
                        player_creation_id = Review.PlayerCreationId,
                        player_creation_name = Review.PlayerCreationName,
                        player_creation_username = Review.PlayerCreationUsername,
                        player_id = Review.PlayerId,
                        rated_by_me = "false",
                        rating_down = Review.RatingDown.ToString(),
                        rating_up = Review.RatingUp.ToString(),
                        username = Review.Username,
                        tags = Review.Tags,
                        updated_at = Review.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz")
                    });
                }
            }

            foreach (var Activity in TrackActivity.Take(3))
            {
                var Author = database.Users.FirstOrDefault(match => match.UserId == Activity.AuthorId);
                ActivityList.Add(new Activity
                {
                    player_creation_id = id,
                    player_creation_hearts = Track.Hearts.Count(),
                    player_creation_rating_up = Track.Ratings.Count(match => match.Type == RatingType.YAY),
                    player_creation_rating_down = Track.Ratings.Count(match => match.Type == RatingType.BOO),
                    player_creation_races_started = Track.RacesStarted.Count(),
                    player_creation_username = Track.Author.Username,
                    player_creation_description = Track.Description,
                    player_creation_name = Track.Name,
                    player_creation_player_id = Track.PlayerId,
                    player_creation_associated_item_ids = Track.AssociatedItemIds,
                    player_creation_level_mode = Track.LevelMode,
                    player_creation_is_team_pick = Track.IsTeamPick,
                    type = "player_creation_activity",
                    events = [
                            new Event
                            {
                                topic = Activity.Type.ToString(),
                                type = Activity.Topic,
                                details = Activity.Description,
                                creator_username = Author != null ? Author.Username : "",
                                creator_id = Activity.AuthorId,
                                timestamp = Activity.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                                seconds_ago = (int)new TimeSpan(DateTime.UtcNow.Ticks - Activity.CreatedAt.Ticks).TotalSeconds,
                                tags = Activity.Tags,
                                allusion_type = Activity.AllusionType,
                                allusion_id = Activity.AllusionId,
                                player_id = Activity.PlayerId
                            }
                        ]
                });
            }

            var allPlayerCreations = database.PlayerCreations           // I dont really know how to optimise this any more without breaking the rank system?
                            .Include(x => x.Points)
                            .Where(match => match.Type == Track.Type)
                            .OrderBy(match => match.Points.Count())     // TODO: Should this be OrderByDescending?
                            .Select(match => match.PlayerCreationId)    // To optimise the amount of data we get back, this is a particularly tricky situation
                            .AsEnumerable();                            // Evaluate our query, find row index after

            var resp = new Response<List<Track>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [
                    new Track
                    {
                        id = Track.PlayerCreationId,
                        ai = Track.AI,
                        associated_item_ids = Track.AssociatedItemIds,
                        auto_reset = Track.AutoReset,
                        battle_friendly_fire = Track.BattleFriendlyFire,
                        battle_kill_count = Track.BattleKillCount,
                        battle_time_limit = Track.BattleTimeLimit,
                        coolness = (Track.Ratings.Count(match => match.Type == RatingType.YAY) - Track.Ratings.Count(match => match.Type == RatingType.BOO)) +
                            ((Track.RacesStarted.Count() + Track.RacesFinished) / 2) + Track.Hearts.Count(),
                        created_at = Track.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        description = Track.Description,
                        difficulty = Track.Difficulty.ToString(),
                        dlc_keys = Track.DLCKeys,
                        downloads = Track.Downloads.Count(),
                        downloads_last_week = Track.Downloads.Count(match => match.DownloadedAt >= DateTime.UtcNow.AddDays(-14) && match.DownloadedAt <= DateTime.UtcNow.AddDays(-7)),
                        downloads_this_week = Track.Downloads.Count(match => match.DownloadedAt >= DateTime.UtcNow.AddDays(-7) && match.DownloadedAt <= DateTime.UtcNow),
                        first_published = Track.FirstPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        last_published = Track.LastPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        hearts = Track.Hearts.Count(),
                        is_remixable = Track.IsRemixable,
                        is_team_pick = Track.IsTeamPick,
                        level_mode = Track.LevelMode,
                        longest_drift = Track.LongestDrift,
                        longest_hang_time = Track.LongestHangTime,
                        max_humans = Track.MaxHumans,
                        name = Track.Name,
                        num_laps = Track.NumLaps,
                        num_racers = Track.NumRacers,
                        platform = Track.Platform.ToString(),
                        player_creation_type = Track.Type == PlayerCreationType.STORY ? PlayerCreationType.TRACK.ToString() : Track.Type.ToString(),
                        player_id = Track.PlayerId,
                        races_finished = Track.RacesFinished,
                        races_started = Track.RacesStarted.Count(),
                        races_started_this_month = Track.RacesStarted.Count(match => match.StartedAt >= DateTime.UtcNow.AddMonths(-1) && match.StartedAt <= DateTime.UtcNow),
                        races_started_this_week = Track.RacesStarted.Count(match => match.StartedAt >= DateTime.UtcNow.AddDays(-7) && match.StartedAt <= DateTime.UtcNow),
                        races_won = Track.RacesWon,
                        race_type = Track.RaceType.ToString(),
                        rank = allPlayerCreations
                            .Select((id, idx) => new { id, idx })
                            .Where(row => row.id == Track.PlayerCreationId)
                            .Select(row => row.idx)
                            .FirstOrDefault(),
                        rating_down = Track.Ratings.Count(match => match.Type == RatingType.BOO),
                        rating_up = Track.Ratings.Count(match => match.Type == RatingType.YAY),
                        scoreboard_mode = Track.ScoreboardMode,
                        speed = Track.Speed.ToString(),
                        tags = Track.Tags,
                        track_theme = Track.TrackTheme,
                        unique_racer_count = Track.UniqueRacers.Count(),
                        updated_at = Track.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        username = Track.Author.Username,
                        user_tags = Track.UserTags,
                        version = Track.Version,
                        views = Track.Views.Count(),
                        views_last_week = Track.Views.Count(match => match.ViewedAt >= DateTime.UtcNow.AddDays(-14) && match.ViewedAt <= DateTime.UtcNow.AddDays(-7)),
                        views_this_week = Track.Views.Count(match => match.ViewedAt >= DateTime.UtcNow.AddDays(-7) && match.ViewedAt <= DateTime.UtcNow),
                        votes = Track.Ratings.Count(match => !Track.IsMNR || match.Rating != 0),
                        weapon_set = Track.WeaponSet,
                        hearted_by_me = (requestedBy == null) ? "false" : Track.Hearts.Any(x => x.UserId == requestedBy.UserId).ToString().ToLower(),
                        queued_by_me = (requestedBy == null) ? "false" : Track.IsBookmarkedByMe(requestedBy.UserId).ToString().ToLower(),   // TODO
                        reviewed_by_me = (requestedBy == null) ? "false" : Track.IsReviewedByMe(requestedBy.UserId).ToString().ToLower(),   // TODO
                        activities = [new Activities { total = TrackActivity.Count, ActivityList = ActivityList }],
                        comments = CommentsList,
                        leaderboard = [new SubLeaderboard { total = TrackScores.Count, LeaderboardPlayersList = ScoresList }],
                        photos = [new Photos { total = PhotoList.Count, PhotoList = PhotoList }],
                        reviews = [new Reviews { total = TrackReviews.Count, ReviewList = ReviewsList }]
                    }
                ]
            };
            return resp.Serialize();
        }

        public static string VerifyPlayerCreations(Database database, List<int> id, List<int> offline_id)
        {
            List<PlayerCreationToVerify> creations = [];
            foreach (int item in id)
            {
                var creation = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == item);
                if (creation != null || (creation != null && (creation.ModerationStatus != ModerationStatus.BANNED
                    || creation.ModerationStatus != ModerationStatus.ILLEGAL)))
                {
                    creations.Add(new PlayerCreationToVerify
                    {
                        id = item,
                        type = creation.Type.ToString(),
                        suggested_action = "allow"
                    });
                }
                else
                {
                    creations.Add(new PlayerCreationToVerify
                    {
                        id = item,
                        type = PlayerCreationType.TRACK.ToString(),
                        suggested_action = "ban"
                    });
                }
            }
            var resp = new Response<List<PlayerCreationVerify>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [
                    new PlayerCreationVerify { total = creations.Count, PlayerCreationsList = creations }
                ]
            };
            return resp.Serialize();
        }

        public static string GetPlanet(Database database, int player_id)
        {
            var Planet = database.PlayerCreations.FirstOrDefault(match => match.PlayerId == player_id && match.Type == PlayerCreationType.PLANET);

            if (Planet == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var resp = new Response<List<Planet>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [new Planet { id = Planet.PlayerCreationId }]
            };
            return resp.Serialize();
        }

        public static string GetPlanetProfile(Database database, int player_id)
        {
            var Planet = database.PlayerCreations
                .FirstOrDefault(match => match.PlayerId == player_id && match.Type == PlayerCreationType.PLANET);

            if (Planet == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }
            var trackList = new List<Track> { };
            var creations = database.PlayerCreations
                .Include(x => x.Downloads)
                .Include(x => x.Hearts)
                .Include(x => x.RacesStarted)
                .Include(x => x.Ratings)
                .Include(x => x.UniqueRacers)
                .Include(x => x.Author)
                .Include(x => x.Views)
                .Where(match => match.PlayerId == player_id && match.Type == PlayerCreationType.TRACK && !match.IsMNR)
                .ToList();

            var allPlayerCreations = database.PlayerCreations           // I dont really know how to optimise this any more without breaking the rank system?
                            .Include(x => x.Points)
                            .Where(match => match.Type == PlayerCreationType.TRACK)
                            .OrderBy(match => match.Points.Count())     // TODO: Should this be OrderByDescending?
                            .Select(match => match.PlayerCreationId)    // To optimise the amount of data we get back, this is a particularly tricky situation
                            .AsEnumerable();                            // Evaluate our query, find row index after

            foreach (PlayerCreationData Track in creations)
            {
                trackList.Add(new Track
                {
                    id = Track.PlayerCreationId,
                    ai = Track.AI,
                    associated_item_ids = Track.AssociatedItemIds,
                    auto_reset = Track.AutoReset,
                    battle_friendly_fire = Track.BattleFriendlyFire,
                    battle_kill_count = Track.BattleKillCount,
                    battle_time_limit = Track.BattleTimeLimit,
                    coolness = (Track.Ratings.Count(match => match.Type == RatingType.YAY) - Track.Ratings.Count(match => match.Type == RatingType.BOO)) +
                            ((Track.RacesStarted.Count() + Track.RacesFinished) / 2) + Track.Hearts.Count(),
                    created_at = Track.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    description = Track.Description,
                    difficulty = Track.Difficulty.ToString(),
                    dlc_keys = Track.DLCKeys,
                    downloads = Track.Downloads.Count(),
                    downloads_last_week = Track.Downloads.Count(match => match.DownloadedAt >= DateTime.UtcNow.AddDays(-14) && match.DownloadedAt <= DateTime.UtcNow.AddDays(-7)),
                    downloads_this_week = Track.Downloads.Count(match => match.DownloadedAt >= DateTime.UtcNow.AddDays(-7) && match.DownloadedAt <= DateTime.UtcNow),
                    first_published = Track.FirstPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    last_published = Track.LastPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    hearts = Track.Hearts.Count(),
                    is_remixable = Track.IsRemixable,
                    is_team_pick = Track.IsTeamPick,
                    level_mode = Track.LevelMode,
                    longest_drift = Track.LongestDrift,
                    longest_hang_time = Track.LongestHangTime,
                    max_humans = Track.MaxHumans,
                    name = Track.Name,
                    num_laps = Track.NumLaps,
                    num_racers = Track.NumRacers,
                    platform = Track.Platform.ToString(),
                    player_creation_type = Track.Type.ToString(),
                    player_id = Track.PlayerId,
                    races_finished = Track.RacesFinished,
                    races_started = Track.RacesStarted.Count(),
                    races_started_this_month = Track.RacesStarted.Count(match => match.StartedAt >= DateTime.UtcNow.AddMonths(-1) && match.StartedAt <= DateTime.UtcNow),
                    races_started_this_week = Track.RacesStarted.Count(match => match.StartedAt >= DateTime.UtcNow.AddDays(-7) && match.StartedAt <= DateTime.UtcNow),
                    races_won = Track.RacesWon,
                    race_type = Track.RaceType.ToString(),
                    rank = allPlayerCreations
                            .Select((id, idx) => new { id, idx })
                            .Where(row => row.id == Track.PlayerCreationId)
                            .Select(row => row.idx)
                            .FirstOrDefault(),
                    rating_down = Track.Ratings.Count(match => match.Type == RatingType.BOO),
                    rating_up = Track.Ratings.Count(match => match.Type == RatingType.YAY),
                    scoreboard_mode = Track.ScoreboardMode,
                    speed = Track.Speed.ToString(),
                    tags = Track.Tags,
                    track_theme = Track.TrackTheme,
                    unique_racer_count = Track.UniqueRacers.Count(),
                    updated_at = Track.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    username = Track.Author.Username,
                    user_tags = Track.UserTags,
                    version = Track.Version,
                    views = Track.Views.Count(),
                    views_last_week = Track.Views.Count(match => match.ViewedAt >= DateTime.UtcNow.AddDays(-14) && match.ViewedAt <= DateTime.UtcNow.AddDays(-7)),
                    views_this_week = Track.Views.Count(match => match.ViewedAt >= DateTime.UtcNow.AddDays(-7) && match.ViewedAt <= DateTime.UtcNow),
                    votes = Track.Ratings.Count(match => !Track.IsMNR || match.Rating != 0),
                    weapon_set = Track.WeaponSet
                });
            }

            var resp = new Response<List<Planet>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [ new Planet {
                    id = Planet.PlayerCreationId,
                    name = Planet.Name,
                    player_id = Planet.PlayerId,
                    username = Planet.Author.Username,
                    tracks = new Tracks {
                        total = Planet.Author.TotalTracks,
                        TrackList = trackList
                    }
                } ]
            };
            return resp.Serialize();
        }
    }
}
