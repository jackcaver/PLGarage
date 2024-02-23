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
                    response = new List<Planet> { new Planet { id = id } }
                };
                return planetUpdateResp.Serialize();
            }

            var resp = new Response<List<player_creation>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<player_creation> { new player_creation { id = id } }
            };
            return resp.Serialize();
        }

        public static string CreatePlayerCreation(Database database, Guid SessionID, PlayerCreation Creation)
        {
            var session = Session.GetSession(SessionID);
            int id = database.PlayerCreations.Count(match => match.Type != PlayerCreationType.STORY) + 10000;
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);
            var deletedCreation = database.PlayerCreations.FirstOrDefault(match => match.Type == PlayerCreationType.DELETED);

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
                && match.IsMNR == session.IsMNR);
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
                    response = new List<Planet> { new Planet { id = id } }
                };
                return planetUpdateResp.Serialize();
            }

            var resp = new Response<List<player_creation>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<player_creation> { new player_creation { id = id } }
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
                PlayerId = user.UserId,
                Platform = Platform.PS3,
                Type = PlayerCreationType.DELETED
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
            var Creation = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == id);
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

            var resp = new Response<List<player_creation>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<player_creation> {
                    new player_creation
                    {
                        id = Creation.PlayerCreationId,
                        ai = Creation.AI,
                        associated_item_ids = Creation.AssociatedItemIds,
                        auto_reset = Creation.AutoReset,
                        battle_friendly_fire = Creation.BattleFriendlyFire,
                        battle_kill_count = Creation.BattleKillCount,
                        battle_time_limit = Creation.BattleTimeLimit,
                        coolness = Creation.Coolness,
                        created_at = Creation.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        description = Creation.Description,
                        difficulty = Creation.Difficulty.ToString(),
                        dlc_keys = Creation.DLCKeys != null ? Creation.DLCKeys : "",
                        downloads = Creation.Downloads,
                        downloads_last_week = Creation.DownloadsLastWeek,
                        downloads_this_week = Creation.DownloadsThisWeek,
                        first_published = Creation.FirstPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        last_published = Creation.LastPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        hearts = Creation.Hearts,
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
                        races_started = Creation.RacesStarted,
                        races_started_this_month = Creation.RacesStartedThisMonth,
                        races_started_this_week = Creation.RacesStartedThisWeek,
                        races_won = Creation.RacesWon,
                        race_type = Creation.RaceType.ToString(),
                        rank = Creation.Rank,
                        rating_down = Creation.RatingDown,
                        rating_up = Creation.RatingUp,
                        scoreboard_mode = Creation.ScoreboardMode,
                        speed = Creation.Speed.ToString(),
                        tags = Creation.Tags,
                        track_theme = Creation.TrackTheme,
                        unique_racer_count = Creation.UniqueRacerCount,
                        updated_at = Creation.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        username = Creation.Username,
                        user_tags = Creation.UserTags,
                        version = Creation.Version,
                        views = Creation.Views,
                        views_last_week = Creation.ViewsLastWeek,
                        views_this_week = Creation.ViewsThisWeek,
                        votes = Creation.Votes,
                        weapon_set = Creation.WeaponSet,
                        data_md5 = download ? UserGeneratedContentUtils.CalculateMD5(id, "data.bin") : null,
                        data_size = download ? UserGeneratedContentUtils.CalculateSize(id, "data.bin").ToString() : null,
                        preview_md5 = download ? UserGeneratedContentUtils.CalculateMD5(id, "preview_image.png") : null,
                        preview_size = download ? UserGeneratedContentUtils.CalculateSize(id, "preview_image.png").ToString() : null,
                        //MNR
                        points = Creation.Points,
                        points_last_week = Creation.PointsLastWeek,
                        points_this_week = Creation.PointsThisWeek,
                        points_today = Creation.PointsToday,
                        points_yesterday = Creation.PointsYesterday,
                        rating = Creation.Rating.ToString("0.00", CultureInfo.InvariantCulture),
                        star_rating = Creation.StarRating,
                        original_player_id = Creation.OriginalPlayerId,
                        original_player_username = database.Users.FirstOrDefault(match => match.UserId == Creation.OriginalPlayerId) != null ? database.Users.FirstOrDefault(match => match.UserId == Creation.OriginalPlayerId).Username : "",
                        parent_creation_id = Creation.ParentCreationId != 0 ? Creation.ParentCreationId.ToString() : "",
                        parent_creation_name = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == Creation.ParentCreationId) != null ? database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == Creation.ParentCreationId).Name : "",
                        parent_player_id = Creation.ParentPlayerId != 0 ? Creation.ParentPlayerId.ToString() : "",
                        parent_player_username = database.Users.FirstOrDefault(match => match.UserId == Creation.ParentPlayerId) != null ? database.Users.FirstOrDefault(match => match.UserId == Creation.ParentPlayerId).Username : "",
                        best_lap_time = Creation.BestLapTime,
                        moderation_status = "APPROVED",
                        moderation_status_id = 1202,
                    }
                }
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
                response = new List<player_creations> { new player_creations { friends_published = Creations.Count != 0 } }
            };
            return resp.Serialize();
        }

        public static string SearchPlayerCreations(Database database, int page, int per_page, SortColumn sort_column, SortOrder sort_order,
            int limit, Platform platform, Filters filters, string keyword = null, bool TeamPicks = false, 
            bool LuckyDip = false, bool IsMNR = false)
        {
            var Creations = new List<PlayerCreationData> { };

            if (filters.username == null && filters.id == null && filters.player_id == null)
                Creations = database.PlayerCreations.Where(match => match.Type == filters.player_creation_type && match.Platform == platform 
                    && match.IsMNR == IsMNR).ToList();

            //filters
            if (filters.username != null)
            {
                foreach (string username in filters.username)
                {
                    var user = database.Users.FirstOrDefault(match => match.Username == username);
                    if (user != null)
                    {
                        var userTracks = database.PlayerCreations.Where(match => match.PlayerId == user.UserId
                            && match.Type == filters.player_creation_type && match.Platform == platform && match.IsMNR == IsMNR).ToList();
                        if (userTracks != null)
                            Creations.AddRange(userTracks);
                    }
                }
            }

            if (filters.id != null)
            {
                foreach (string id in filters.id)
                {
                    var Creation = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId.ToString() == id &&
                        (match.Type == filters.player_creation_type || match.Type == PlayerCreationType.STORY) && match.IsMNR == IsMNR);
                    if (Creation != null)
                        Creations.Add(Creation);
                }
            }

            if (filters.player_id != null)
            {
                foreach (string player_id in filters.player_id)
                {
                    var user = database.Users.FirstOrDefault(match => match.UserId.ToString() == player_id);
                    if (user != null)
                    {
                        var userTracks = database.PlayerCreations.Where(match => match.PlayerId == user.UserId
                            && match.Type == filters.player_creation_type && match.Platform == platform && match.IsMNR == IsMNR).ToList();
                        if (userTracks != null)
                            Creations.AddRange(userTracks);
                    }
                }
            }

            Creations.RemoveAll(match => match.ModerationStatus == ModerationStatus.BANNED 
                || match.ModerationStatus == ModerationStatus.ILLEGAL);

            if (keyword != null)
                Creations.RemoveAll(match => !match.Name.Contains(keyword));

            if (filters.race_type != null)
                Creations.RemoveAll(match => !filters.race_type.Equals(match.RaceType.ToString()));

            if (filters.tags != null && filters.tags.Length != 0)
            {
                Creations.RemoveAll(match => match.Tags == null);
                foreach (string tag in filters.tags)
                {
                    Creations.RemoveAll(match => !match.Tags.Split(',').Contains(tag));
                }
            }

            if (filters.auto_reset != null)
                Creations.RemoveAll(match => match.AutoReset != filters.auto_reset);

            if (filters.ai != null)
                Creations.RemoveAll(match => match.AI != filters.ai);

            if (filters.is_remixable != null)
                Creations.RemoveAll(match => match.IsRemixable != filters.is_remixable);

            if (TeamPicks)
                Creations.RemoveAll(match => !match.IsTeamPick);

            //cool levels
            if (sort_column == SortColumn.coolness)
                Creations.Sort((curr, prev) => prev.Coolness.CompareTo(curr.Coolness));

            //newest levels
            if (sort_column == SortColumn.created_at)
                Creations.Sort((curr, prev) => prev.CreatedAt.CompareTo(curr.CreatedAt));

            //most played 
            if (sort_column == SortColumn.races_started)
                Creations.Sort((curr, prev) => prev.RacesStarted.CompareTo(curr.RacesStarted));
            if (sort_column == SortColumn.races_started_this_week)
                Creations.Sort((curr, prev) => prev.RacesStartedThisWeek.CompareTo(curr.RacesStartedThisWeek));
            if (sort_column == SortColumn.races_started_this_month)
                Creations.Sort((curr, prev) => prev.RacesStartedThisMonth.CompareTo(curr.RacesStartedThisMonth));

            //highest rated
            if (sort_column == SortColumn.rating_up)
                Creations.Sort((curr, prev) => prev.RatingUp.CompareTo(curr.RatingUp));
            if (sort_column == SortColumn.rating_up_this_week)
                Creations.Sort((curr, prev) => prev.RatingUpThisWeek.CompareTo(curr.RatingUpThisWeek));
            if (sort_column == SortColumn.rating_up_this_month)
                Creations.Sort((curr, prev) => prev.RatingUpThisMonth.CompareTo(curr.RatingUpThisMonth));

            //most hearted
            if (sort_column == SortColumn.hearts)
                Creations.Sort((curr, prev) => prev.Hearts.CompareTo(curr.Hearts));
            if (sort_column == SortColumn.hearts_this_week)
                Creations.Sort((curr, prev) => prev.HeartsThisWeek.CompareTo(curr.HeartsThisWeek));
            if (sort_column == SortColumn.hearts_this_month)
                Creations.Sort((curr, prev) => prev.HeartsThisMonth.CompareTo(curr.HeartsThisMonth));

            //MNR
            if (sort_column == SortColumn.rating)
                Creations.Sort((curr, prev) => prev.Rating.CompareTo(curr.Rating));
            //points
            if (sort_column == SortColumn.points)
                Creations.Sort((curr, prev) => prev.Points.CompareTo(curr.Points));
            if (sort_column == SortColumn.points_today)
                Creations.Sort((curr, prev) => prev.PointsToday.CompareTo(curr.PointsYesterday));
            if (sort_column == SortColumn.points_yesterday)
                Creations.Sort((curr, prev) => prev.PointsYesterday.CompareTo(curr.PointsYesterday));
            if (sort_column == SortColumn.points_this_week)
                Creations.Sort((curr, prev) => prev.PointsThisWeek.CompareTo(curr.PointsThisWeek));
            if (sort_column == SortColumn.points_last_week)
                Creations.Sort((curr, prev) => prev.PointsLastWeek.CompareTo(curr.PointsLastWeek));
            //download
            if (sort_column == SortColumn.downloads)
                Creations.Sort((curr, prev) => prev.Downloads.CompareTo(curr.Downloads));
            if (sort_column == SortColumn.downloads_this_week)
                Creations.Sort((curr, prev) => prev.DownloadsThisWeek.CompareTo(curr.DownloadsThisWeek));
            if (sort_column == SortColumn.downloads_last_week)
                Creations.Sort((curr, prev) => prev.DownloadsLastWeek.CompareTo(curr.DownloadsLastWeek));
            //views
            if (sort_column == SortColumn.views)
                Creations.Sort((curr, prev) => prev.Views.CompareTo(curr.Views));
            if (sort_column == SortColumn.views_this_week)
                Creations.Sort((curr, prev) => prev.ViewsThisWeek.CompareTo(curr.ViewsThisWeek));
            if (sort_column == SortColumn.views_last_week)
                Creations.Sort((curr, prev) => prev.ViewsLastWeek.CompareTo(curr.ViewsLastWeek));

            //calculating pages
            int pageEnd = PageCalculator.GetPageEnd(page, per_page);
            int pageStart = PageCalculator.GetPageStart(page, per_page);
            int totalPages = PageCalculator.GetTotalPages(per_page, Creations.Count);

            if (pageEnd > Creations.Count)
                pageEnd = Creations.Count;

            var playerCreationsList = new List<player_creation> { };

            for (int i = pageStart; i < pageEnd; i++)
            {
                var Creation = Creations[i];
                if (Creation != null)
                {
                    playerCreationsList.Add(new player_creation
                    {
                        id = Creation.PlayerCreationId,
                        ai = Creation.AI,
                        associated_item_ids = Creation.AssociatedItemIds,
                        auto_reset = Creation.AutoReset,
                        battle_friendly_fire = Creation.BattleFriendlyFire,
                        battle_kill_count = Creation.BattleKillCount,
                        battle_time_limit = Creation.BattleTimeLimit,
                        coolness = Creation.Coolness,
                        created_at = Creation.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        description = Creation.Description,
                        difficulty = Creation.Difficulty.ToString(),
                        dlc_keys = Creation.DLCKeys,
                        downloads = Creation.Downloads,
                        downloads_last_week = Creation.DownloadsLastWeek,
                        downloads_this_week = Creation.DownloadsThisWeek,
                        first_published = Creation.FirstPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        last_published = Creation.LastPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        hearts = Creation.Hearts,
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
                        player_creation_type = (Creation.Type == PlayerCreationType.STORY) ? PlayerCreationType.TRACK.ToString() : Creation.Type.ToString(),
                        player_id = Creation.PlayerId,
                        races_finished = Creation.RacesFinished,
                        races_started = Creation.RacesStarted,
                        races_started_this_month = Creation.RacesStartedThisMonth,
                        races_started_this_week = Creation.RacesStartedThisWeek,
                        races_won = Creation.RacesWon,
                        race_type = Creation.RaceType.ToString(),
                        rank = Creation.Rank,
                        rating_down = Creation.RatingDown,
                        rating_up = Creation.RatingUp,
                        scoreboard_mode = Creation.ScoreboardMode,
                        speed = Creation.Speed.ToString(),
                        tags = Creation.Tags,
                        track_theme = Creation.TrackTheme,
                        unique_racer_count = Creation.UniqueRacerCount,
                        updated_at = Creation.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        username = Creation.Username,
                        user_tags = Creation.UserTags,
                        version = Creation.Version,
                        views = Creation.Views,
                        views_last_week = Creation.ViewsLastWeek,
                        views_this_week = Creation.ViewsThisWeek,
                        votes = Creation.Votes,
                        weapon_set = Creation.WeaponSet,
                        //MNR
                        points = Creation.Points,
                        points_last_week = Creation.PointsLastWeek,
                        points_this_week = Creation.PointsThisWeek,
                        points_today = Creation.PointsToday,
                        points_yesterday = Creation.PointsYesterday,
                        rating = Creation.Rating.ToString("0.0", CultureInfo.InvariantCulture),
                        star_rating = Creation.StarRating
                    });
                }
            }

            var resp = new Response<List<player_creations>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<player_creations> {
                    new player_creations
                    {
                        page = page,
                        row_end = pageEnd,
                        row_start = pageStart,
                        total = Creations.Count,
                        total_pages = totalPages,
                        PlayerCreationsList = playerCreationsList
                    }
                }
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
            return SearchPlayerCreations(database, page, per_page, sort_column, sort_order, limit, platform, filters, keyword, false, false, true);
        }

        public static string SearchPhotos(Database database, int? track_id, string username, string associated_usernames, int page, int per_page)
        {
            List<PlayerCreationData> Photos = new List<PlayerCreationData> { };
            int player_id = 0;
            var user = database.Users.FirstOrDefault(match => match.Username == username);
            if (user != null)
                player_id = user.UserId;

            if (associated_usernames != null)
                Photos = database.PlayerCreations.Where(match => match.Type == PlayerCreationType.PHOTO && match.AssociatedUsernames.Contains(associated_usernames)).ToList();
            if (username != null)
                Photos = database.PlayerCreations.Where(match => match.Type == PlayerCreationType.PHOTO && match.PlayerId == player_id).ToList();
            if (track_id != null)
                Photos = database.PlayerCreations.Where(match => match.Type == PlayerCreationType.PHOTO && match.TrackId == track_id).ToList();
            if (track_id != null && username != null)
                Photos = database.PlayerCreations.Where(match => match.Type == PlayerCreationType.PHOTO && match.TrackId == track_id && match.PlayerId == player_id).ToList();

            Photos.Sort((curr, prev) => prev.CreatedAt.CompareTo(curr.CreatedAt));

            int pageEnd = PageCalculator.GetPageEnd(page, per_page);
            int pageStart = PageCalculator.GetPageStart(page, per_page);
            int totalPages = PageCalculator.GetTotalPages(per_page, Photos.Count);

            if (pageEnd > Photos.Count)
                pageEnd = Photos.Count;

            var PhotoList = new List<Photo> { };
            for (int i = pageStart; i < pageEnd; i++)
            {
                var Photo = Photos[i];
                PhotoList.Add(new Photo
                {
                    associated_usernames = Photo.AssociatedUsernames,
                    id = Photo.PlayerCreationId,
                    track_id = Photo.TrackId,
                    username = Photo.Username
                });
            }

            var resp = new Response<List<Photos>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<Photos> {
                    new Photos {
                        total = Photos.Count,
                        current_page = page,
                        row_start = pageStart,
                        row_end = pageEnd,
                        total_pages = totalPages,
                        PhotoList = PhotoList
                    }
                }
            };
            return resp.Serialize();
        }

        public static string GetTrackProfile(Database database, Guid SessionID, int id)
        {
            var session = Session.GetSession(SessionID);
            var Track = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == id);
            var requestedBy = database.Users.FirstOrDefault(match => match.Username == session.Username);
            var TrackPhotos = database.PlayerCreations.Where(match => match.TrackId == id && match.Type == PlayerCreationType.PHOTO).ToList();
            var TrackScores = database.Scores.Where(match => match.SubKeyId == id).ToList();
            var TrackComments = database.PlayerCreationComments.Where(match => match.PlayerCreationId == id).ToList();
            var TrackReviews = database.PlayerCreationReviews.Where(match => match.PlayerCreationId == id).ToList();
            var TrackActivity = database.ActivityLog.Where(match => match.PlayerCreationId == id).ToList();
            List<Photo> PhotoList = new List<Photo> { };
            List<SubLeaderboardPlayer> ScoresList = new List<SubLeaderboardPlayer> { };
            List<Comment> CommentsList = new List<Comment> { };
            List<Review> ReviewsList = new List<Review> { };
            List<Activity> ActivityList = new List<Activity> { };

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
                    player_creation_hearts = Track.Hearts,
                    player_creation_rating_up = Track.RatingUp,
                    player_creation_rating_down = Track.RatingDown,
                    player_creation_races_started = Track.RacesStarted,
                    player_creation_username = Track.Username,
                    player_creation_description = Track.Description,
                    player_creation_name = Track.Name,
                    player_creation_player_id = Track.PlayerId,
                    player_creation_associated_item_ids = Track.AssociatedItemIds,
                    player_creation_level_mode = Track.LevelMode,
                    player_creation_is_team_pick = Track.IsTeamPick,
                    type = "player_creation_activity",
                    events = new List<Event> {
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
                        }
                });
            }

            var resp = new Response<List<Track>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<Track> {
                    new Track
                    {
                        id = Track.PlayerCreationId,
                        ai = Track.AI,
                        associated_item_ids = Track.AssociatedItemIds,
                        auto_reset = Track.AutoReset,
                        battle_friendly_fire = Track.BattleFriendlyFire,
                        battle_kill_count = Track.BattleKillCount,
                        battle_time_limit = Track.BattleTimeLimit,
                        coolness = Track.Coolness,
                        created_at = Track.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        description = Track.Description,
                        difficulty = Track.Difficulty.ToString(),
                        dlc_keys = Track.DLCKeys,
                        downloads = Track.Downloads,
                        downloads_last_week = Track.DownloadsLastWeek,
                        downloads_this_week = Track.DownloadsThisWeek,
                        first_published = Track.FirstPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        last_published = Track.LastPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        hearts = Track.Hearts,
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
                        races_started = Track.RacesStarted,
                        races_started_this_month = Track.RacesStartedThisMonth,
                        races_started_this_week = Track.RacesStartedThisWeek,
                        races_won = Track.RacesWon,
                        race_type = Track.RaceType.ToString(),
                        rank = Track.Rank,
                        rating_down = Track.RatingDown,
                        rating_up = Track.RatingUp,
                        scoreboard_mode = Track.ScoreboardMode,
                        speed = Track.Speed.ToString(),
                        tags = Track.Tags,
                        track_theme = Track.TrackTheme,
                        unique_racer_count = Track.UniqueRacerCount,
                        updated_at = Track.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        username = Track.Username,
                        user_tags = Track.UserTags,
                        version = Track.Version,
                        views = Track.Views,
                        views_last_week = Track.ViewsLastWeek,
                        views_this_week = Track.ViewsThisWeek,
                        votes = Track.Votes,
                        weapon_set = Track.WeaponSet,
                        hearted_by_me = (requestedBy == null) ? "false" : Track.IsHeartedByMe(requestedBy.UserId).ToString().ToLower(),
                        queued_by_me = (requestedBy == null) ? "false" : Track.IsBookmarkedByMe(requestedBy.UserId).ToString().ToLower(),
                        reviewed_by_me = (requestedBy == null) ? "false" : Track.IsReviewedByMe(requestedBy.UserId).ToString().ToLower(),
                        activities = new List<Activities> { new Activities { total = TrackActivity.Count, ActivityList = ActivityList } },
                        comments = CommentsList,
                        leaderboard = new List<SubLeaderboard> { new SubLeaderboard { total = TrackScores.Count, LeaderboardPlayersList = ScoresList } },
                        photos = new List<Photos> { new Photos { total = PhotoList.Count, PhotoList = PhotoList } },
                        reviews = new List<Reviews> { new Reviews { total = TrackReviews.Count, ReviewList = ReviewsList } }
                    }
                }
            };
            return resp.Serialize();
        }

        public static string VerifyPlayerCreations(Database database, List<int> id, List<int> offline_id)
        {
            List<PlayerCreationToVerify> creations = new List<PlayerCreationToVerify> { };
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
                response = new List<PlayerCreationVerify> {
                    new PlayerCreationVerify { total = creations.Count, PlayerCreationsList = creations }
                }
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
                response = new List<Planet> { new Planet { id = Planet.PlayerCreationId } }
            };
            return resp.Serialize();
        }

        public static string GetPlanetProfile(Database database, int player_id)
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
            var trackList = new List<Track> { };
            var creations = database.PlayerCreations.Where(match => match.PlayerId == player_id && match.Type == PlayerCreationType.TRACK && !match.IsMNR).ToList();
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
                    coolness = Track.Coolness,
                    created_at = Track.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    description = Track.Description,
                    difficulty = Track.Difficulty.ToString(),
                    dlc_keys = Track.DLCKeys,
                    downloads = Track.Downloads,
                    downloads_last_week = Track.DownloadsLastWeek,
                    downloads_this_week = Track.DownloadsThisWeek,
                    first_published = Track.FirstPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    last_published = Track.LastPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    hearts = Track.Hearts,
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
                    races_started = Track.RacesStarted,
                    races_started_this_month = Track.RacesStartedThisMonth,
                    races_started_this_week = Track.RacesStartedThisWeek,
                    races_won = Track.RacesWon,
                    race_type = Track.RaceType.ToString(),
                    rank = Track.Rank,
                    rating_down = Track.RatingDown,
                    rating_up = Track.RatingUp,
                    scoreboard_mode = Track.ScoreboardMode,
                    speed = Track.Speed.ToString(),
                    tags = Track.Tags,
                    track_theme = Track.TrackTheme,
                    unique_racer_count = Track.UniqueRacerCount,
                    updated_at = Track.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    username = Track.Username,
                    user_tags = Track.UserTags,
                    version = Track.Version,
                    views = Track.Views,
                    views_last_week = Track.ViewsLastWeek,
                    views_this_week = Track.ViewsThisWeek,
                    votes = Track.Votes,
                    weapon_set = Track.WeaponSet
                });
            }

            var resp = new Response<List<Planet>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<Planet> { new Planet {
                    id = Planet.PlayerCreationId,
                    name = Planet.Name,
                    player_id = Planet.PlayerId,
                    username = Planet.Username,
                    tracks = new Tracks {
                        total = Planet.Author.TotalTracks,
                        TrackList = trackList
                    }
                } }
            };
            return resp.Serialize();
        }
    }
}
