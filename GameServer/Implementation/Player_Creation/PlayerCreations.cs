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
using System.IO;

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
            Creation.LastPublished = TimeUtils.Now;
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
            Creation.UpdatedAt = TimeUtils.Now;
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
                    CreatedAt = TimeUtils.Now,
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
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);
            
            Stream data = null;
            Stream preview = null;

            if (Creation.player_creation_type != PlayerCreationType.DELETED)
                data = Creation.data.OpenReadStream();

            if (Creation.player_creation_type != PlayerCreationType.PHOTO
                && Creation.player_creation_type != PlayerCreationType.PLANET
                && Creation.player_creation_type != PlayerCreationType.DELETED)
                preview = Creation.preview.OpenReadStream();

            if (user == null || Creation.player_creation_type == PlayerCreationType.DELETED
                || (Creation.player_creation_type == PlayerCreationType.PHOTO
                    && !UserGeneratedContentUtils.CheckImage(data)) //check if photo is a valid image
                || (Creation.player_creation_type != PlayerCreationType.PHOTO 
                    && UserGeneratedContentUtils.CheckImage(data)) //check if data for player creation is an image
                || (Creation.player_creation_type != PlayerCreationType.PLANET 
                    && Creation.player_creation_type != PlayerCreationType.PHOTO 
                    && !UserGeneratedContentUtils.CheckImage(preview, 256, 256)) //check if preview for player creation is valid image
                )
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

            var playerCreation = new PlayerCreationData
            {
                AI = Creation.ai,
                AssociatedCoordinates = Creation.associated_coordinates,
                AssociatedItemIds = Creation.associated_item_ids,
                AssociatedUsernames = Creation.associated_usernames,
                AutoReset = Creation.auto_reset,
                AutoTags = Creation.auto_tags,
                BattleFriendlyFire = Creation.battle_friendly_fire,
                BattleKillCount = Creation.battle_kill_count,
                BattleTimeLimit = Creation.battle_time_limit,
                CreatedAt = TimeUtils.Now,
                Description = Creation.description,
                Difficulty = Creation.difficulty,
                DLCKeys = Creation.dlc_keys,
                FirstPublished = TimeUtils.Now,
                IsRemixable = Creation.is_remixable,
                IsTeamPick = Creation.is_team_pick,
                LastPublished = TimeUtils.Now,
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
                UpdatedAt = TimeUtils.Now,
                UserTags = Creation.user_tags,
                WeaponSet = Creation.weapon_set,
                TrackId = Creation.track_id,
                Version = 1,
                //MNR
                IsMNR = session.IsMNR,
                ParentCreationId = database.PlayerCreations.Any(match => match.PlayerCreationId == Creation.parent_creation_id) || Creation.parent_creation_id < 10000 ? Creation.parent_creation_id : 0,
                ParentPlayerId = database.Users.Any(match => match.UserId == Creation.parent_player_id) ? Creation.parent_player_id : user.UserId,
                OriginalPlayerId = database.Users.Any(match => match.UserId == Creation.original_player_id) ? Creation.original_player_id : user.UserId,
                BestLapTime = Creation.best_lap_time
            };
            database.PlayerCreations.Add(playerCreation);
            database.SaveChanges();

            if (playerCreation.TrackId == 0)
            {
                playerCreation.TrackId = playerCreation.PlayerCreationId;
                database.PlayerCreations.Update(playerCreation);
                database.SaveChanges();
            }

            var id = playerCreation.PlayerCreationId;

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
                    CreatedAt = TimeUtils.Now,
                    AllusionId = id,
                    AllusionType = "PlayerCreation::Track"
                });
            }

            database.SaveChanges();


            if (Creation.player_creation_type == PlayerCreationType.PHOTO)
                UserGeneratedContentUtils.SavePlayerPhoto(id, data);
            else if (Creation.player_creation_type == PlayerCreationType.PLANET)
                UserGeneratedContentUtils.SavePlayerCreation(id, data);
            else
                UserGeneratedContentUtils.SavePlayerCreation(id, data, preview);

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
                .AsSplitQuery()
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
                    database.PlayerCreationViews.Add(new PlayerCreationView { PlayerCreationId = Creation.PlayerCreationId, ViewedAt = TimeUtils.Now });
                    database.SaveChanges();
                }

                if (IsCounted && download)
                {
                    var uniqueRacer = database.PlayerCreationUniqueRacers.FirstOrDefault(match => match.PlayerId == User.UserId);
                    database.PlayerCreationDownloads.Add(new PlayerCreationDownload { PlayerCreationId = Creation.PlayerCreationId, DownloadedAt = TimeUtils.Now });
                    database.PlayerCreationPoints.Add(new PlayerCreationPoint { PlayerCreationId = Creation.PlayerCreationId, PlayerId = Creation.PlayerId, Platform = Creation.Platform, Type = Creation.Type, CreatedAt = TimeUtils.Now, Amount = 100 });

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
                                CreatedAt = TimeUtils.Now,
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
                                CreatedAt = TimeUtils.Now,
                                AllusionId = Creation.PlayerCreationId,
                                AllusionType = "PlayerCreation::Track"
                            });
                        }
                    }

                    if (uniqueRacer != null && uniqueRacer.Version != Creation.Version)
                    {
                        database.PlayerCreationDownloads.Add(new PlayerCreationDownload { PlayerCreationId = Creation.PlayerCreationId, DownloadedAt = TimeUtils.Now });
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
                                CreatedAt = TimeUtils.Now,
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
                                CreatedAt = TimeUtils.Now,
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
                        coolness = Creation.Coolness,
                        created_at = Creation.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        description = Creation.Description,
                        difficulty = Creation.Difficulty.ToString(),
                        dlc_keys = Creation.DLCKeys != null ? Creation.DLCKeys : "",
                        downloads = Creation.DownloadsCount,
                        downloads_last_week = Creation.DownloadsLastWeek,
                        downloads_this_week = Creation.DownloadsThisWeek,
                        first_published = Creation.FirstPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        last_published = Creation.LastPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        hearts = Creation.HeartsCount,
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
                        player_creation_type = (Creation.Type == PlayerCreationType.STORY ? PlayerCreationType.TRACK : Creation.Type).ToString(),
                        player_id = Creation.PlayerId,
                        races_finished = Creation.RacesFinished,
                        races_started = Creation.RacesStartedCount,
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
                        username = Creation.Author.Username,
                        user_tags = Creation.UserTags,
                        version = Creation.Version,
                        views = Creation.ViewsCount,
                        views_last_week = Creation.ViewsLastWeek,
                        views_this_week = Creation.ViewsThisWeek,
                        votes = Creation.Votes,
                        weapon_set = Creation.WeaponSet,
                        data_md5 = download ? UserGeneratedContentUtils.CalculateMD5(id, "data.bin") : null,
                        data_size = download ? UserGeneratedContentUtils.CalculateSize(id, "data.bin").ToString() : null,
                        preview_md5 = download ? UserGeneratedContentUtils.CalculateMD5(id, "preview_image.png") : null,
                        preview_size = download ? UserGeneratedContentUtils.CalculateSize(id, "preview_image.png").ToString() : null,
                        //MNR
                        // TODO: Remove some DB queries here and use one to many links in EF model
                        points = Creation.PointsAmount,
                        points_last_week = Creation.PointsLastWeek,
                        points_this_week = Creation.PointsThisWeek,
                        points_today = Creation.PointsToday,
                        points_yesterday = Creation.PointsYesterday,
                        rating = Creation.Rating.ToString("0.0", CultureInfo.InvariantCulture),
                        star_rating = Creation.StarRating,
                        original_player_id = Creation.OriginalPlayerId,
                        original_player_username = database.Users.Any(match => match.UserId == Creation.OriginalPlayerId) ? database.Users.FirstOrDefault(match => match.UserId == Creation.OriginalPlayerId).Username : "",
                        parent_creation_id = Creation.ParentCreationId != 0 ? Creation.ParentCreationId.ToString() : "",
                        parent_creation_name = database.PlayerCreations.Any(match => match.PlayerCreationId == Creation.ParentCreationId) ? database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == Creation.ParentCreationId).Name : "",
                        parent_player_id = Creation.ParentPlayerId != 0 ? Creation.ParentPlayerId.ToString() : "",
                        parent_player_username = database.Users.Any(match => match.UserId == Creation.ParentPlayerId) ? database.Users.FirstOrDefault(match => match.UserId == Creation.ParentPlayerId).Username : "",
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
                .AsSplitQuery()
                .Include(x => x.Downloads)
                .Include(x => x.RacesStarted)
                .Include(x => x.UniqueRacers)
                .Include(x => x.Hearts)
                .Include(x => x.Points)
                .Include(x => x.Ratings)
                .Include(x => x.Views)
                .Include(x => x.Author)
                .Where(match => match.Platform == platform && match.IsMNR == IsMNR);
            var session = Session.GetSession(SessionID);
            var User = database.Users.FirstOrDefault(match => match.Username == session.Username);

            if (filters.username == null && filters.id == null && filters.player_id == null)
                creationQuery = creationQuery.Where(match => match.Type == filters.player_creation_type);

            //filters
            if (filters.username != null)
                creationQuery = creationQuery.Where(match => match.Type == filters.player_creation_type && filters.username.Any(x => match.Author.Username == x));

            if (filters.id != null)
                creationQuery = creationQuery.Where(match => (match.Type == filters.player_creation_type || match.Type == PlayerCreationType.STORY) && filters.id.Any(x => match.PlayerCreationId.ToString() == x));

            if (filters.player_id != null)
                creationQuery = creationQuery.Where(match => match.Type == filters.player_creation_type && filters.player_id.Any(x => match.PlayerId.ToString() == x));

            creationQuery = creationQuery.Where(match => match.ModerationStatus != ModerationStatus.BANNED
                && match.ModerationStatus != ModerationStatus.ILLEGAL);

            if (keyword != null)
                creationQuery = creationQuery.Where(match => match.Name.Contains(keyword));

            if (filters.race_type != null)
                creationQuery = creationQuery.Where(match => filters.race_type.Equals(match.RaceType.ToString()));

            if (filters.tags != null && filters.tags.Length != 0)
            {
                creationQuery = creationQuery.Where(match => match.Tags != null);
                foreach (var tag in filters.tags)
                    creationQuery = creationQuery.Where(match => match.Tags.Contains(tag));   // TODO: Optimise?
            }

            if (filters.auto_reset != null)
                creationQuery = creationQuery.Where(match => match.AutoReset == filters.auto_reset);

            if (filters.ai != null)
                creationQuery = creationQuery.Where(match => match.AI == filters.ai);

            if (filters.is_remixable != null)
                creationQuery = creationQuery.Where(match => match.IsRemixable == filters.is_remixable);

            if (TeamPicks)
                creationQuery = creationQuery.Where(match => match.IsTeamPick);

            if (User != null && !User.ShowCreationsWithoutPreviews)
                creationQuery = creationQuery.Where(match => match.HasPreview);

            switch (sort_column)
            {
                //cool levels
                case SortColumn.coolness:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => (match.Ratings.Count(match => match.Type == RatingType.YAY) - match.Ratings.Count(match => match.Type == RatingType.BOO)) +
                                ((match.RacesStarted.Count() + match.RacesFinished) / 2) + match.Hearts.Count()) :
                            creationQuery.OrderByDescending(match => (match.Ratings.Count(match => match.Type == RatingType.YAY) - match.Ratings.Count(match => match.Type == RatingType.BOO)) +
                                ((match.RacesStarted.Count() + match.RacesFinished) / 2) + match.Hearts.Count());
                    break;

                //newest levels
                case SortColumn.created_at:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.CreatedAt) :
                            creationQuery.OrderByDescending(match => match.CreatedAt);
                    break;

                //most played
                case SortColumn.races_started:
                    creationQuery =
                        sort_order == SortOrder.asc ? 
                            creationQuery.OrderBy(match => match.RacesStarted.Count()) : 
                            creationQuery.OrderByDescending(match => match.RacesStarted.Count());
                    break;
                case SortColumn.races_started_this_week:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.RacesStarted.Count(match => match.StartedAt >= TimeUtils.ThisWeekStart)) : 
                            creationQuery.OrderByDescending(match => match.RacesStarted.Count(match => match.StartedAt >= TimeUtils.ThisWeekStart));
                    break;
                case SortColumn.races_started_this_month:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.RacesStarted.Count(match => match.StartedAt >= TimeUtils.ThisMonthStart)) :
                            creationQuery.OrderByDescending(match => match.RacesStarted.Count(match => match.StartedAt >= TimeUtils.ThisMonthStart));
                    break;

                //highest rated
                case SortColumn.rating_up:
                    creationQuery =
                        sort_order == SortOrder.asc ? 
                            creationQuery.OrderBy(match => match.Ratings.Count(match => match.Type == RatingType.YAY)) : 
                            creationQuery.OrderByDescending(match => match.Ratings.Count(match => match.Type == RatingType.YAY));
                    break;
                case SortColumn.rating_up_this_week:
                    creationQuery =
                        sort_order == SortOrder.asc ? 
                            creationQuery.OrderBy(match => match.Ratings.Count(match => match.Type == RatingType.YAY && match.RatedAt >= TimeUtils.ThisWeekStart)) : 
                            creationQuery.OrderByDescending(match => match.Ratings.Count(match => match.Type == RatingType.YAY && match.RatedAt >= TimeUtils.ThisWeekStart));
                    break;
                case SortColumn.rating_up_this_month:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.Ratings.Count(match => match.Type == RatingType.YAY && match.RatedAt >= TimeUtils.ThisMonthStart)) :
                            creationQuery.OrderByDescending(match => match.Ratings.Count(match => match.Type == RatingType.YAY && match.RatedAt >= TimeUtils.ThisMonthStart));
                    break;

                //most hearted
                case SortColumn.hearts:
                    creationQuery =
                        sort_order == SortOrder.asc ? 
                            creationQuery.OrderBy(match => match.Hearts.Count()) : 
                            creationQuery.OrderByDescending(match => match.Hearts.Count());
                    break;
                case SortColumn.hearts_this_week:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.Hearts.Count(match => match.HeartedAt >= TimeUtils.ThisWeekStart)) :
                            creationQuery.OrderByDescending(match => match.Hearts.Count(match => match.HeartedAt >= TimeUtils.ThisWeekStart));
                    break;
                case SortColumn.hearts_this_month:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.Hearts.Count(match => match.HeartedAt >= TimeUtils.ThisMonthStart)) :
                            creationQuery.OrderByDescending(match => match.Hearts.Count(match => match.HeartedAt >= TimeUtils.ThisMonthStart));
                    break;

                //MNR
                case SortColumn.rating:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.Ratings.Count() != 0 ? (float)match.Ratings.Average(r => r.Rating) : 0) :   // Can we simplify this and remove the count check?
                            creationQuery.OrderByDescending(match => match.Ratings.Count() != 0 ? (float)match.Ratings.Average(r => r.Rating) : 0);
                    break;

                //points
                case SortColumn.points:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.Points.Sum(p => p.Amount)) :
                            creationQuery.OrderByDescending(match => match.Points.Sum(p => p.Amount));
                    break;
                case SortColumn.points_today:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.Points.Where(match => match.CreatedAt >= TimeUtils.DayStart).Sum(p => p.Amount)) :
                            creationQuery.OrderByDescending(match => match.Points.Where(match => match.CreatedAt >= TimeUtils.DayStart).Sum(p => p.Amount));
                    break;
                case SortColumn.points_yesterday:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.Points.Where(match => match.CreatedAt >= TimeUtils.YesterdayStart && match.CreatedAt < TimeUtils.DayStart).Sum(p => p.Amount)) :
                            creationQuery.OrderByDescending(match => match.Points.Where(match => match.CreatedAt >= TimeUtils.YesterdayStart && match.CreatedAt < TimeUtils.DayStart).Sum(p => p.Amount));
                    break;
                case SortColumn.points_this_week:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderByDescending(match => match.Points.Where(match => match.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount)) :
                            creationQuery.OrderBy(match => match.Points.Where(match => match.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount));
                    break;
                case SortColumn.points_last_week:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.Points.Where(match => match.CreatedAt >= TimeUtils.LastWeekStart && match.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount)) :
                            creationQuery.OrderByDescending(match => match.Points.Where(match => match.CreatedAt >= TimeUtils.LastWeekStart && match.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount));
                    break;

                //download
                case SortColumn.downloads:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.Downloads.Count()) :
                            creationQuery.OrderByDescending(match => match.Downloads.Count());
                    break;
                case SortColumn.downloads_this_week:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.Downloads.Count(match => match.DownloadedAt >= TimeUtils.ThisWeekStart)) :
                            creationQuery.OrderByDescending(match => match.Downloads.Count(match => match.DownloadedAt >= TimeUtils.ThisWeekStart));
                    break;
                case SortColumn.downloads_last_week:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.Downloads.Count(match => match.DownloadedAt >= TimeUtils.LastWeekStart && match.DownloadedAt < TimeUtils.ThisWeekStart)) :
                            creationQuery.OrderByDescending(match => match.Downloads.Count(match => match.DownloadedAt >= TimeUtils.LastWeekStart && match.DownloadedAt < TimeUtils.ThisWeekStart));
                    break;

                //views
                case SortColumn.views:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.Views.Count()) :
                            creationQuery.OrderByDescending(match => match.Views.Count());
                    break;
                case SortColumn.views_this_week:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.Views.Count(match => match.ViewedAt >= TimeUtils.ThisWeekStart)) :
                            creationQuery.OrderByDescending(match => match.Views.Count(match => match.ViewedAt >= TimeUtils.ThisWeekStart));
                    break;
                case SortColumn.views_last_week:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.Views.Count(match => match.ViewedAt >= TimeUtils.LastWeekStart && match.ViewedAt < TimeUtils.ThisWeekStart)) :
                            creationQuery.OrderByDescending(match => match.Views.Count(match => match.ViewedAt >= TimeUtils.LastWeekStart && match.ViewedAt < TimeUtils.ThisWeekStart));
                    break;
            }

            if (LuckyDip)
                creationQuery = creationQuery.OrderBy(match => EF.Functions.Random());  // TODO: is session.RandomSeed required? Will this even add onto the above, needs ThenBy?

            // TODO: sort_order

            var total = creationQuery.Count();

            //calculating pages
            int pageEnd = PageCalculator.GetPageEnd(page, per_page);
            int pageStart = PageCalculator.GetPageStart(page, per_page);
            int totalPages = PageCalculator.GetTotalPages(per_page, total);

            if (pageEnd > total)
                pageEnd = total;
            if (pageStart > pageEnd)
                pageStart = pageEnd;

            var creations = creationQuery
                .Skip(pageStart)
                .Take(per_page)
                .ToList();

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
                    coolness = creation.Coolness,
                    created_at = creation.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    description = creation.Description,
                    difficulty = creation.Difficulty.ToString(),
                    dlc_keys = creation.DLCKeys,
                    downloads = creation.DownloadsCount,
                    downloads_last_week = creation.DownloadsLastWeek,
                    downloads_this_week = creation.DownloadsThisWeek,
                    first_published = creation.FirstPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    last_published = creation.LastPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    hearts = creation.HeartsCount,
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
                    player_creation_type = (creation.Type == PlayerCreationType.STORY ? PlayerCreationType.TRACK : creation.Type).ToString(),
                    player_id = creation.PlayerId,
                    races_finished = creation.RacesFinished,
                    races_started = creation.RacesStartedCount,
                    races_started_this_month = creation.RacesStartedThisMonth,
                    races_started_this_week = creation.RacesStartedThisWeek,
                    races_won = creation.RacesWon,
                    race_type = creation.RaceType.ToString(),
                    rank = creation.Rank,
                    rating_down = creation.RatingDown,
                    rating_up = creation.RatingUp,
                    scoreboard_mode = creation.ScoreboardMode,
                    speed = creation.Speed.ToString(),
                    tags = creation.Tags,
                    track_theme = creation.TrackTheme,
                    unique_racer_count = creation.UniqueRacerCount,
                    updated_at = creation.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    username = creation.Author.Username,
                    user_tags = creation.UserTags,
                    version = creation.Version,
                    views = creation.ViewsCount,
                    views_last_week = creation.ViewsLastWeek,
                    views_this_week = creation.ViewsThisWeek,
                    votes = creation.Ratings.Count(match => !IsMNR || match.Rating != 0),
                    weapon_set = creation.WeaponSet,
                    //MNR
                    points = creation.PointsAmount,
                    points_last_week = creation.PointsLastWeek,
                    points_this_week = creation.PointsThisWeek,
                    points_today = creation.PointsToday,
                    points_yesterday = creation.PointsYesterday,
                    rating = creation.Rating.ToString("0.0", CultureInfo.InvariantCulture),
                    star_rating = creation.StarRating,
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

            if (associated_usernames != null)
                photosQuery = photosQuery.Where(match => match.AssociatedUsernames.Contains(associated_usernames));
            if (username != null)
                photosQuery = photosQuery.Where(match => match.Author.Username == username);
            if (track_id != null)
                photosQuery = photosQuery.Where(match => match.TrackId == track_id);

            photosQuery = photosQuery.OrderByDescending(match => match.CreatedAt);

            var total = photosQuery.Count();

            int pageEnd = PageCalculator.GetPageEnd(page, per_page);
            int pageStart = PageCalculator.GetPageStart(page, per_page);
            int totalPages = PageCalculator.GetTotalPages(per_page, total);

            if (pageEnd > total)
                pageEnd = total;
            if (pageStart > pageEnd)
                pageStart = pageEnd;

            var photos = photosQuery
                .Skip(pageStart)
                .Take(per_page)
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
            var session = Session.GetSession(SessionID);
            var requestedBy = database.Users
                .FirstOrDefault(match => match.Username == session.Username);

            var Track = database.PlayerCreations
                .AsSplitQuery()
                .Include(x => x.Hearts)
                .Include(x => x.Ratings)
                .Include(x => x.RacesStarted)
                .Include(x => x.Author)
                .Include(x => x.Points)
                .Include(x => x.Downloads)
                .Include(x => x.UniqueRacers)
                .Include(x => x.Views)
                .Include(x => x.Scores)
                .ThenInclude(s => s.User)
                .Include(x => x.Comments)
                .Include(x => x.Bookmarks)
                .Include(x => x.Reviews)
                .ThenInclude(r => r.ReviewRatings)
                .Include(x => x.Reviews)
                .ThenInclude(r => r.User)
                .FirstOrDefault(match => match.PlayerCreationId == id);
            var TrackPhotos = database.PlayerCreations
                .Where(match => match.TrackId == id && match.Type == PlayerCreationType.PHOTO)
                .OrderByDescending(match => match.CreatedAt)
                .Take(3)
                .ToList();
            var ActivityLog = database.ActivityLog
                .OrderByDescending(a => a.CreatedAt);

            List<Photo> PhotoList = [];
            List<SubLeaderboardPlayer> ScoresList = [];
            List<Comment> CommentsList = [];
            List<Review> ReviewsList = [];
            List<Activity> ActivityList = [];

            if (Track == null || id < 9000)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            Track.Comments.Sort((curr, prev) => prev.CreatedAt.CompareTo(curr.CreatedAt));
            Track.Reviews.Sort((curr, prev) => prev.CreatedAt.CompareTo(curr.CreatedAt));

            if (Track.ScoreboardMode == 1)
                Track.Scores.Sort((curr, prev) => curr.FinishTime.CompareTo(prev.FinishTime));
            else
                Track.Scores.Sort((curr, prev) => prev.Points.CompareTo(curr.Points));

            foreach (PlayerCreationData Photo in TrackPhotos)
            {
                PhotoList.Add(new Photo
                {
                    id = Photo.PlayerCreationId
                });
            }

            foreach (Score Score in Track.Scores.Take(3))
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

            foreach (PlayerCreationCommentData Comment in Track.Comments.Take(3))
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

            foreach (PlayerCreationReview Review in Track.Reviews.Take(3))
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

            var trackActivity = ActivityLog.Take(3).ToList();
            foreach (var Activity in trackActivity)
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
                                seconds_ago = TimeUtils.SecondsAgo(Activity.CreatedAt),
                                tags = Activity.Tags,
                                allusion_type = Activity.AllusionType,
                                allusion_id = Activity.AllusionId,
                                player_id = Activity.PlayerId
                            }
                        ]
                });
            }

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
                        coolness = Track.Coolness,
                        created_at = Track.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        description = Track.Description,
                        difficulty = Track.Difficulty.ToString(),
                        dlc_keys = Track.DLCKeys,
                        downloads = Track.DownloadsCount,
                        downloads_last_week = Track.DownloadsLastWeek,
                        downloads_this_week = Track.DownloadsThisWeek,
                        first_published = Track.FirstPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        last_published = Track.LastPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                        hearts = Track.HeartsCount,
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
                        player_creation_type = (Track.Type == PlayerCreationType.STORY ? PlayerCreationType.TRACK : Track.Type).ToString(),
                        player_id = Track.PlayerId,
                        races_finished = Track.RacesFinished,
                        races_started = Track.RacesStartedCount,
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
                        username = Track.Author.Username,
                        user_tags = Track.UserTags,
                        version = Track.Version,
                        views = Track.ViewsCount,
                        views_last_week = Track.ViewsLastWeek,
                        views_this_week = Track.ViewsThisWeek,
                        votes = Track.Ratings.Count(match => !Track.IsMNR || match.Rating != 0),
                        weapon_set = Track.WeaponSet,
                        hearted_by_me = (requestedBy == null) ? "false" : Track.IsHeartedByMe(requestedBy.UserId).ToString().ToLower(),
                        queued_by_me = (requestedBy == null) ? "false" : Track.IsBookmarkedByMe(requestedBy.UserId).ToString().ToLower(),
                        reviewed_by_me = (requestedBy == null) ? "false" : Track.IsReviewedByMe(requestedBy.UserId).ToString().ToLower(),
                        activities = [new Activities { total = ActivityLog.Count(), ActivityList = ActivityList }],
                        comments = CommentsList,
                        leaderboard = [new SubLeaderboard { total = Track.Scores.Count, LeaderboardPlayersList = ScoresList }],
                        photos = [new Photos { total = PhotoList.Count, PhotoList = PhotoList }],
                        reviews = [new Reviews { total = Track.Reviews.Count, ReviewList = ReviewsList }]
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
                .Include(x => x.Author)
                .Include(x => x.Author.PlayerCreations)
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
                .AsSplitQuery()
                .Include(x => x.Downloads)
                .Include(x => x.Hearts)
                .Include(x => x.RacesStarted)
                .Include(x => x.Ratings)
                .Include(x => x.UniqueRacers)
                .Include(x => x.Author)
                .Include(x => x.Views)
                .Where(match => match.PlayerId == player_id && match.Type == PlayerCreationType.TRACK && !match.IsMNR)
                .ToList();

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
                    downloads = Track.DownloadsCount,
                    downloads_last_week = Track.DownloadsLastWeek,
                    downloads_this_week = Track.DownloadsThisWeek,
                    first_published = Track.FirstPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    last_published = Track.LastPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    hearts = Track.HeartsCount,
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
                    player_creation_type = (Track.Type == PlayerCreationType.STORY ? PlayerCreationType.TRACK : Track.Type).ToString(),
                    player_id = Track.PlayerId,
                    races_finished = Track.RacesFinished,
                    races_started = Track.RacesStartedCount,
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
                    username = Track.Author.Username,
                    user_tags = Track.UserTags,
                    version = Track.Version,
                    views = Track.ViewsCount,
                    views_last_week = Track.ViewsLastWeek,
                    views_this_week = Track.ViewsThisWeek,
                    votes = Track.Votes,
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
