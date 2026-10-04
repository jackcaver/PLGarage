using GameServer.Models;
using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using GameServer.Models.Response;
using GameServer.Utils;
using System.Collections.Generic;
using System.Linq;
using GameServer.Models.PlayerData.PlayerCreations;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using System.IO;
using GameServer.Models.Config;
using LinqToDB;
using LinqToDB.EntityFrameworkCore;

namespace GameServer.Implementation.Player_Creation
{
    public class PlayerCreations
    {
        public static string UpdatePlayerCreation(Database database, IUGCStorage storage, SessionData session, PlayerCreation PlayerCreation, int id = 0)
        {
            var user = session.User;

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
                return CreatePlayerCreation(database, storage, session, PlayerCreation);
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
                    PlayerId = null,
                    PlayerCreationId = Creation.PlayerCreationId,
                    CreatedAt = TimeUtils.Now,
                    AllusionId = Creation.PlayerCreationId,
                    AllusionType = "PlayerCreation::Track"
                });
            }

            database.SaveChanges();

            if (PlayerCreation.player_creation_type != PlayerCreationType.PLANET)
                storage.SavePlayerCreation(Creation.PlayerCreationId,
                   PlayerCreation.data.OpenReadStream(),
                   PlayerCreation.preview.OpenReadStream());
            else
                storage.SavePlayerCreation(Creation.PlayerCreationId, PlayerCreation.data.OpenReadStream());

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

        public static string CreatePlayerCreation(Database database, IUGCStorage storage, SessionData session, PlayerCreation Creation)
        {
            var user = session.User;
            
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
                && match.Type != PlayerCreationType.PHOTO && match.Type != PlayerCreationType.ITEM
                && match.Type != PlayerCreationType.DELETED && match.ModerationStatus != ModerationStatus.BANNED 
                && match.ModerationStatus != ModerationStatus.ILLEGAL
                && match.IsMNR == session.IsMNR && match.Platform == session.Platform);
            if (quota >= user.Quota && Creation.player_creation_type != PlayerCreationType.PHOTO 
                && Creation.player_creation_type != PlayerCreationType.ITEM)
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
                ParentPlayerId = database.Users.Any(match => match.UserId == Creation.parent_player_id) ? Creation.parent_player_id : 0,
                OriginalPlayerId = database.Users.Any(match => match.UserId == Creation.original_player_id) ? Creation.original_player_id : 0,
                BestLapTime = Creation.best_lap_time
            };
            database.PlayerCreations.Add(playerCreation);
            database.SaveChanges();

            if (playerCreation.PlayerCreationId < 10000)
            {
                database.PlayerCreations.Remove(playerCreation); //unfortunately .net ef can't update primary keys...
                database.SaveChanges();
                playerCreation.PlayerCreationId = 10000;
                database.PlayerCreations.Add(playerCreation);
                database.SaveChanges();
            }
            
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
                    PlayerId = null,
                    PlayerCreationId = id,
                    CreatedAt = TimeUtils.Now,
                    AllusionId = id,
                    AllusionType = "PlayerCreation::Track"
                });
            }

            database.SaveChanges();


            if (Creation.player_creation_type == PlayerCreationType.PHOTO)
                storage.SavePlayerPhoto(id, data);
            else if (Creation.player_creation_type == PlayerCreationType.PLANET)
                storage.SavePlayerCreation(id, data);
            else
                storage.SavePlayerCreation(id, data, preview);

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

        public static string RemovePlayerCreation(Database database, IUGCStorage storage, User user, int id)
        {
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

            Creation.Type = PlayerCreationType.DELETED;

            foreach (var item in database.PlayerCreations.Where(match => match.TrackId == Creation.PlayerCreationId)
                         .Select(item => item.PlayerCreationId).ToList())
            {
                var Photo = database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == item);
                Photo.TrackId = 4912;
            }

            database.PlayerCreationDownloads
                .Where(x => x.PlayerCreationId == Creation.PlayerCreationId)
                .ExecuteDelete();

            database.PlayerCreationViews
                .Where(x => x.PlayerCreationId == Creation.PlayerCreationId)
                .ExecuteDelete();

            database.PlayerCreationRatings
                .Where(x => x.PlayerCreationId == Creation.PlayerCreationId)
                .ExecuteDelete();

            database.PlayerCreationPoints
                .Where(x => x.PlayerCreationId == Creation.PlayerCreationId)
                .ExecuteDelete();

            database.PlayerCreationComments
                .Where(x => x.PlayerCreationId == Creation.PlayerCreationId)
                .ExecuteDelete();

            database.PlayerCreationReviews
                .Where(x => x.PlayerCreationId == Creation.PlayerCreationId)
                .ExecuteDelete();
            
            database.HeartedPlayerCreations
                .Where(h => h.HeartedPlayerCreationId == Creation.PlayerCreationId)
                .ExecuteDelete();
            
            database.PlayerCreationBookmarks
                .Where(b => b.BookmarkedPlayerCreationId == Creation.PlayerCreationId)
                .ExecuteDelete();
        
            database.ActivityLog
                .Where(match => match.PlayerCreationId == Creation.PlayerCreationId)
                .ExecuteDelete();

            database.SaveChanges();

            if (ServerConfig.Instance.DeleteCreationData)
                storage.RemovePlayerCreation(id);

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };
            return resp.Serialize();
        }

        public static string GetPlayerCreation(Database database, IUGCStorage storage, SessionData session, int id, bool IsCounted, bool download = false)
        {
            var creation = database.PlayerCreations
                .Select(creationData => new player_creation
                {
                    id = creationData.PlayerCreationId,
                    ai = creationData.AI,
                    associated_item_ids = creationData.AssociatedItemIds,
                    auto_reset = creationData.AutoReset,
                    battle_friendly_fire = creationData.BattleFriendlyFire,
                    battle_kill_count = creationData.BattleKillCount,
                    battle_time_limit = creationData.BattleTimeLimit,
                    coolness = creationData.Coolness,
                    created_at = creationData.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    description = creationData.Description,
                    difficulty = creationData.Difficulty.ToString(),
                    dlc_keys = creationData.DLCKeys ?? "",
                    downloads = creationData.Downloads,
                    downloads_last_week = creationData.DownloadsLastWeek,
                    downloads_this_week = creationData.DownloadsThisWeek,
                    first_published = creationData.FirstPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    last_published = creationData.LastPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    hearts = creationData.Hearts,
                    is_remixable = creationData.IsRemixable,
                    is_team_pick = creationData.IsTeamPick,
                    level_mode = creationData.LevelMode,
                    longest_drift = creationData.LongestDrift,
                    longest_hang_time = creationData.LongestHangTime,
                    max_humans = creationData.MaxHumans,
                    name = creationData.Name,
                    num_laps = creationData.NumLaps,
                    num_racers = creationData.NumRacers,
                    platform = creationData.Platform.ToString(),
                    player_creation_type = (creationData.Type == PlayerCreationType.STORY ? PlayerCreationType.TRACK : creationData.Type).ToString(),
                    player_id = creationData.PlayerId,
                    races_finished = creationData.RacesFinished,
                    races_started = creationData.RacesStarted,
                    races_started_this_month = creationData.RacesStartedThisMonth,
                    races_started_this_week = creationData.RacesStartedThisWeek,
                    races_won = creationData.RacesWon,
                    race_type = creationData.RaceType.ToString(),
                    rank = (int)Sql.Window.RowNumber(f => f.OrderBy(creationData.Points)),
                    rating_down = creationData.RatingDown,
                    rating_up = creationData.RatingUp,
                    scoreboard_mode = creationData.ScoreboardMode,
                    speed = creationData.Speed.ToString(),
                    tags = creationData.Tags,
                    track_theme = creationData.TrackTheme,
                    unique_racer_count = creationData.UniqueRacers,
                    updated_at = creationData.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    username = creationData.Author.Username,
                    user_tags = creationData.UserTags,
                    version = creationData.Version,
                    views = creationData.Views,
                    views_last_week = creationData.ViewsLastWeek,
                    views_this_week = creationData.ViewsThisWeek,
                    votes = creationData.Votes,
                    weapon_set = creationData.WeaponSet,
                    data_md5 = download ? storage.CalculateMD5(id, "data.bin") : null,
                    data_size = download ? storage.CalculateSize(id, "data.bin").ToString() : null,
                    preview_md5 = download ? storage.CalculateMD5(id, "preview_image.png") : null,
                    preview_size = download ? storage.CalculateSize(id, "preview_image.png").ToString() : null,
                    //MNR
                    // TODO: Remove some DB queries here and use one to many links in EF model
                    points = creationData.Points,
                    points_last_week = creationData.PointsLastWeek,
                    points_this_week = creationData.PointsThisWeek,
                    points_today = creationData.PointsToday,
                    points_yesterday = creationData.PointsYesterday,
                    rating = creationData.Rating.ToString("0.0", CultureInfo.InvariantCulture),
                    star_rating = creationData.StarRating,
                    original_player_id = creationData.ParentPlayerId != 0 ? creationData.ParentPlayerId.ToString() : "",
                    parent_creation_id = creationData.ParentCreationId != 0 ? creationData.ParentCreationId.ToString() : "",
                    parent_player_id = creationData.ParentPlayerId != 0 ? creationData.ParentPlayerId.ToString() : "",
                    best_lap_time = creationData.BestLapTime,
                    moderation_status = creationData.ModerationStatus.ToString(),
                    moderation_status_id = (int)creationData.ModerationStatus,
                })
                .ToLinqToDB()
                .FirstOrDefault(match => match.id == id);
            var user = session.User;

            if (creation == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            if (int.TryParse(creation.original_player_id, out int originalPlayerId))
                creation.original_player_username = database.Users
                    .Select(u => new { u.UserId, u.Username })
                    .FirstOrDefault(match => match.UserId == originalPlayerId)?.Username ?? "";
            
            if (int.TryParse(creation.parent_player_id, out int parentPlayerId))
                creation.parent_player_username = database.Users
                    .Select(u => new { u.UserId, u.Username })
                    .FirstOrDefault(match => match.UserId == parentPlayerId)?.Username ?? "";
            
            if (int.TryParse(creation.parent_creation_id, out int parentCreationId))
                creation.parent_player_username = database.PlayerCreations
                    .Select(c => new { c.PlayerCreationId, c.Name })
                    .FirstOrDefault(match => match.PlayerCreationId == parentCreationId)?.Name ?? "";

            if (user != null)
            {
                bool isOwner = user.UserId == creation.player_id;

                if (IsCounted && !download && !isOwner)
                {
                    if (!database.PlayerCreationViews
                        .Any(match =>
                            match.PlayerId == user.UserId &&
                            match.PlayerCreationId == creation.id))
                    {
                        database.PlayerCreationViews.Add(new PlayerCreationView
                        {
                            PlayerId = user.UserId,
                            PlayerCreationId = creation.id,
                            ViewedAt = TimeUtils.Now
                        });

                        database.SaveChanges();
                    }
                }

                if (IsCounted && download && !isOwner)
                {
                    if (!database.PlayerCreationDownloads
                        .Any(match =>
                            match.PlayerId == user.UserId &&
                            match.PlayerCreationId == creation.id))
                    {
                        database.PlayerCreationDownloads.Add(new PlayerCreationDownload
                        {
                            PlayerId = user.UserId,
                            PlayerCreationId = creation.id,
                            DownloadedAt = TimeUtils.Now
                        });

                        if (session.IsMNR)
                        {
                            var pointData = database.PlayerCreations
                                .Select(c => new { c.PlayerCreationId, c.PlayerId, c.Platform, c.Type })
                                .FirstOrDefault(match => match.PlayerCreationId == creation.id);
                            if (pointData != null)
                            {
                                database.PlayerCreationPoints.Add(new PlayerCreationPoint
                                {
                                    PlayerCreationId = pointData.PlayerCreationId,
                                    PlayerId = pointData.PlayerId,
                                    Platform = pointData.Platform,
                                    Type = pointData.Type,
                                    CreatedAt = TimeUtils.Now,
                                    Amount = 100
                                });
                            }
                        }

                        if (!session.IsMNR)
                        {
                            database.ActivityLog.Add(new ActivityEvent
                            {
                                AuthorId = user.UserId,
                                Type = ActivityType.player_creation_event,
                                List = ActivityList.activity_log,
                                Topic = "player_creation_downloaded",
                                Description = "",
                                PlayerId = null,
                                PlayerCreationId = creation.id,
                                CreatedAt = TimeUtils.Now,
                                AllusionId = creation.id,
                                AllusionType = "PlayerCreation::Track"
                            });
                        }

                        database.SaveChanges();
                    }
                }
            }

            var resp = new Response<List<player_creation>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [
                    creation
                ]
            };
            return resp.Serialize();
        }

        public static string PlayerCreationsFriendsPublished(Database database, string usernameFilter, PlayerCreationType type)
        {
            bool friendsPublished = false;

            if (usernameFilter != null)
            {
                var usernames = usernameFilter.Split(',');
                friendsPublished = database.PlayerCreations.Any(match => usernames.Contains(match.Username) 
                                                                          && match.Type == type);
            }

            var resp = new Response<List<player_creations>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [new player_creations { friends_published = friendsPublished }]
            };
            return resp.Serialize();
        }

        public static string SearchPlayerCreations(Database database, User user, int page, int per_page, SortColumn sort_column, SortOrder sort_order,
            int limit, Platform platform, Filters filters, string keyword = null, bool TeamPicks = false, 
            bool LuckyDip = false, bool IsMNR = false)
        {
            IQueryable<PlayerCreationData> creationQuery = database.PlayerCreations     // TODO: Is it an issue someone might be able to fudge the entire database out like this?
                .AsNoTracking()
                .Where(match => match.Platform == platform && match.IsMNR == IsMNR);
            if (filters.username == null && filters.id == null && filters.player_id == null)
                creationQuery = creationQuery.Where(match => match.Type == filters.player_creation_type);

            //filters
            if (filters.username != null)
                creationQuery = creationQuery.Where(match => match.Type == filters.player_creation_type && filters.username.Contains(match.Username));

            if (filters.id != null)
                creationQuery = creationQuery.Where(match => (match.Type == filters.player_creation_type || match.Type == PlayerCreationType.STORY) && filters.id.Contains(match.PlayerCreationId.ToString()));

            if (filters.player_id != null)
                creationQuery = creationQuery.Where(match => match.Type == filters.player_creation_type && filters.player_id.Contains(match.PlayerId.ToString()));

            if (filters.id == null && ServerConfig.Instance.HideUnmoderatedCreationsFromSearch)
                creationQuery = creationQuery.Where(match => match.ModerationStatus != ModerationStatus.PENDING);

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

            if (user != null && !user.ShowCreationsWithoutPreviews)
                creationQuery = creationQuery.Where(match => match.HasPreview);

            switch (sort_column)
            {
                //cool levels
                case SortColumn.coolness:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.Coolness) :
                            creationQuery.OrderByDescending(match => match.Coolness);
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
                            creationQuery.OrderBy(match => match.RacesStarted) : 
                            creationQuery.OrderByDescending(match => match.RacesStarted);
                    break;
                case SortColumn.races_started_this_week:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.RacesStartedThisWeek) : 
                            creationQuery.OrderByDescending(match => match.RacesStartedThisWeek);
                    break;
                case SortColumn.races_started_this_month:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.RacesStartedThisMonth) :
                            creationQuery.OrderByDescending(match => match.RacesStartedThisMonth);
                    break;

                //highest rated
                case SortColumn.rating_up:
                    creationQuery =
                        sort_order == SortOrder.asc ? 
                            creationQuery.OrderBy(match => match.RatingUp) : 
                            creationQuery.OrderByDescending(match => match.RatingUp);
                    break;
                case SortColumn.rating_up_this_week:
                    creationQuery =
                        sort_order == SortOrder.asc ? 
                            creationQuery.OrderBy(match => match.RatingUpThisWeek) : 
                            creationQuery.OrderByDescending(match => match.RatingUpThisWeek);
                    break;
                case SortColumn.rating_up_this_month:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.RatingUpThisMonth) :
                            creationQuery.OrderByDescending(match => match.RatingUpThisMonth);
                    break;

                //most hearted
                case SortColumn.hearts:
                    creationQuery =
                        sort_order == SortOrder.asc ? 
                            creationQuery.OrderBy(match => match.Hearts) : 
                            creationQuery.OrderByDescending(match => match.Hearts);
                    break;
                case SortColumn.hearts_this_week:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.HeartsThisWeek) :
                            creationQuery.OrderByDescending(match => match.HeartsThisWeek);
                    break;
                case SortColumn.hearts_this_month:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.HeartsThisMonth) :
                            creationQuery.OrderByDescending(match => match.HeartsThisMonth);
                    break;

                //MNR
                case SortColumn.rating:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.Rating) :   // Can we simplify this and remove the count check?
                            creationQuery.OrderByDescending(match => match.Rating);
                    break;

                //points
                case SortColumn.points:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.Points) :
                            creationQuery.OrderByDescending(match => match.Points);
                    break;
                case SortColumn.points_today:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.PointsToday) :
                            creationQuery.OrderByDescending(match => match.PointsToday);
                    break;
                case SortColumn.points_yesterday:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.PointsYesterday) :
                            creationQuery.OrderByDescending(match => match.PointsYesterday);
                    break;
                case SortColumn.points_this_week:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderByDescending(match => match.PointsThisWeek) :
                            creationQuery.OrderBy(match => match.PointsThisWeek);
                    break;
                case SortColumn.points_last_week:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.PointsLastWeek) :
                            creationQuery.OrderByDescending(match => match.PointsLastWeek);
                    break;

                //download
                case SortColumn.downloads:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.Downloads) :
                            creationQuery.OrderByDescending(match => match.Downloads);
                    break;
                case SortColumn.downloads_this_week:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.DownloadsThisWeek) :
                            creationQuery.OrderByDescending(match => match.DownloadsThisWeek);
                    break;
                case SortColumn.downloads_last_week:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.DownloadsLastWeek) :
                            creationQuery.OrderByDescending(match => match.DownloadsLastWeek);
                    break;

                //views
                case SortColumn.views:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.Views) :
                            creationQuery.OrderByDescending(match => match.Views);
                    break;
                case SortColumn.views_this_week:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.ViewsThisWeek) :
                            creationQuery.OrderByDescending(match => match.ViewsThisWeek);
                    break;
                case SortColumn.views_last_week:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.ViewsLastWeek) :
                            creationQuery.OrderByDescending(match => match.ViewsLastWeek);
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
                .Select(creation => new player_creation
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
                    downloads = creation.Downloads,
                    downloads_last_week = creation.DownloadsLastWeek,
                    downloads_this_week = creation.DownloadsThisWeek,
                    first_published = creation.FirstPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    last_published = creation.LastPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    hearts = creation.Hearts,
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
                    races_started = creation.RacesStarted,
                    races_started_this_month = creation.RacesStartedThisMonth,
                    races_started_this_week = creation.RacesStartedThisWeek,
                    races_won = creation.RacesWon,
                    race_type = creation.RaceType.ToString(),
                    rank = (int)Sql.Window.RowNumber(f => 
                        sort_column == SortColumn.coolness ? 
                            (sort_order == SortOrder.asc ? f.OrderBy(creation.Coolness) : f.OrderByDesc(creation.Coolness)) :
                        sort_column == SortColumn.created_at ? 
                            (sort_order == SortOrder.asc ? f.OrderBy(creation.CreatedAt) : f.OrderByDesc(creation.CreatedAt)) :
                        sort_column == SortColumn.races_started ? 
                            (sort_order == SortOrder.asc ? f.OrderBy(creation.RacesStarted) : f.OrderByDesc(creation.RacesStarted)) :
                        sort_column == SortColumn.races_started_this_week ? 
                            (sort_order == SortOrder.asc ? f.OrderBy(creation.RacesStartedThisWeek) : f.OrderByDesc(creation.RacesStartedThisWeek)) :
                        sort_column == SortColumn.races_started_this_month ? 
                            (sort_order == SortOrder.asc ? f.OrderBy(creation.RacesStartedThisMonth) : f.OrderByDesc(creation.RacesStartedThisMonth)) :
                        sort_column == SortColumn.rating_up ? 
                            (sort_order == SortOrder.asc ? f.OrderBy(creation.RatingUp) : f.OrderByDesc(creation.RatingUp)) :
                        sort_column == SortColumn.rating_up_this_week ? 
                            (sort_order == SortOrder.asc ? f.OrderBy(creation.RatingUpThisWeek) : f.OrderByDesc(creation.RatingUpThisWeek)) :
                        sort_column == SortColumn.rating_up_this_month ? 
                            (sort_order == SortOrder.asc ? f.OrderBy(creation.RatingUpThisMonth) : f.OrderByDesc(creation.RatingUpThisMonth)) :
                        sort_column == SortColumn.hearts ? 
                            (sort_order == SortOrder.asc ? f.OrderBy(creation.Hearts) : f.OrderByDesc(creation.Hearts)) :
                        sort_column == SortColumn.hearts_this_week ? 
                            (sort_order == SortOrder.asc ? f.OrderBy(creation.HeartsThisWeek) : f.OrderByDesc(creation.HeartsThisWeek)) :
                        sort_column == SortColumn.hearts_this_month ? 
                            (sort_order == SortOrder.asc ? f.OrderBy(creation.HeartsThisMonth) : f.OrderByDesc(creation.HeartsThisMonth)) :
                        sort_column == SortColumn.rating ? 
                            (sort_order == SortOrder.asc ? f.OrderBy(creation.Rating) : f.OrderByDesc(creation.Rating)) :
                        sort_column == SortColumn.downloads ? 
                            (sort_order == SortOrder.asc ? f.OrderBy(creation.Downloads) : f.OrderByDesc(creation.Downloads)) :
                        sort_column == SortColumn.downloads_last_week ? 
                            (sort_order == SortOrder.asc ? f.OrderBy(creation.DownloadsLastWeek) : f.OrderByDesc(creation.DownloadsLastWeek)) :
                        sort_column == SortColumn.downloads_this_week ? 
                            (sort_order == SortOrder.asc ? f.OrderBy(creation.DownloadsThisWeek) : f.OrderByDesc(creation.DownloadsThisWeek)) :
                        sort_column == SortColumn.views ? 
                            (sort_order == SortOrder.asc ? f.OrderBy(creation.Views) : f.OrderByDesc(creation.Views)) :
                        sort_column == SortColumn.views_last_week ? 
                            (sort_order == SortOrder.asc ? f.OrderBy(creation.ViewsLastWeek) : f.OrderByDesc(creation.ViewsLastWeek)) :
                        sort_column == SortColumn.views_this_week ? 
                            (sort_order == SortOrder.asc ? f.OrderBy(creation.ViewsThisWeek) : f.OrderByDesc(creation.ViewsThisWeek)) :
                        sort_column == SortColumn.points_yesterday ? 
                            (sort_order == SortOrder.asc ? f.OrderBy(creation.PointsYesterday) : f.OrderByDesc(creation.PointsYesterday)) :
                        sort_column == SortColumn.points_today ? 
                            (sort_order == SortOrder.asc ? f.OrderBy(creation.PointsToday) : f.OrderByDesc(creation.PointsToday)) :
                        sort_column == SortColumn.points_this_week ? 
                            (sort_order == SortOrder.asc ? f.OrderBy(creation.PointsThisWeek) : f.OrderByDesc(creation.PointsThisWeek)) :
                        sort_column == SortColumn.points_last_week ? 
                            (sort_order == SortOrder.asc ? f.OrderBy(creation.PointsLastWeek) : f.OrderByDesc(creation.PointsLastWeek)) : 
                            (sort_order == SortOrder.asc ? f.OrderBy(creation.Points) : f.OrderByDesc(creation.Points))),
                    rating_down = creation.RatingDown,
                    rating_up = creation.RatingUp,
                    scoreboard_mode = creation.ScoreboardMode,
                    speed = creation.Speed.ToString(),
                    tags = creation.Tags,
                    track_theme = creation.TrackTheme,
                    unique_racer_count = creation.UniqueRacers,
                    updated_at = creation.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    username = creation.Username,
                    user_tags = creation.UserTags,
                    version = creation.Version,
                    views = creation.Views,
                    views_last_week = creation.ViewsLastWeek,
                    views_this_week = creation.ViewsThisWeek,
                    votes = creation.Votes,
                    weapon_set = creation.WeaponSet,
                    //MNR
                    points = creation.Points,
                    points_last_week = creation.PointsLastWeek,
                    points_this_week = creation.PointsThisWeek,
                    points_today = creation.PointsToday,
                    points_yesterday = creation.PointsYesterday,
                    rating = creation.Rating.ToString("0.0", CultureInfo.InvariantCulture),
                    star_rating = creation.StarRating,
                    moderation_status = creation.ModerationStatus.ToString(),
                    moderation_status_id = (int)creation.ModerationStatus
                })
                .Skip(pageStart)
                .Take(per_page)
                .ToLinqToDB()
                .ToList();

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
                        PlayerCreationsList = creations
                    }
                ]
            };
            return resp.Serialize();
        }

        public static string Mine(Database database, SessionData session, int page, int per_page, SortColumn sort_column, SortOrder sort_order,
            int limit, Filters filters, string keyword = null, Platform? platformOverride = null) 
        {
            var user = session.User;
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
            return SearchPlayerCreations(database, user, page, per_page, sort_column, sort_order, limit, platform, filters, keyword, false, false, true);
        }

        public static string SearchPhotos(Database database, int? track_id, string username, string associated_usernames, int page, int per_page)
        {
            var photosQuery = database.PlayerCreations
                .Include(x => x.Author)
                .Where(match => match.Type == PlayerCreationType.PHOTO && match.ModerationStatus != ModerationStatus.ILLEGAL 
                                                                       && match.ModerationStatus != ModerationStatus.BANNED);

            if (associated_usernames != null)
                photosQuery = photosQuery.Where(match => match.AssociatedUsernames.Contains(associated_usernames));
            if (username != null)
                photosQuery = photosQuery.Where(match => match.Username == username);
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
                .Select(photo => new Photo
                {
                    associated_usernames = photo.AssociatedUsernames,
                    id = photo.PlayerCreationId,
                    track_id = photo.TrackId,
                    username = photo.Author.Username
                })
                .ToList();

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
                        PhotoList = photos
                    }
                ]
            };
            return resp.Serialize();
        }

        public static string GetTrackProfile(Database database, User requestedBy, int id)
        {
            var track = database.PlayerCreations
                .Select(creationData => new Track
                {
                    id = creationData.PlayerCreationId,
                    ai = creationData.AI,
                    associated_item_ids = creationData.AssociatedItemIds,
                    auto_reset = creationData.AutoReset,
                    battle_friendly_fire = creationData.BattleFriendlyFire,
                    battle_kill_count = creationData.BattleKillCount,
                    battle_time_limit = creationData.BattleTimeLimit,
                    coolness = creationData.Coolness,
                    created_at = creationData.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    description = creationData.Description,
                    difficulty = creationData.Difficulty.ToString(),
                    dlc_keys = creationData.DLCKeys,
                    downloads = creationData.Downloads,
                    downloads_last_week = creationData.DownloadsLastWeek,
                    downloads_this_week = creationData.DownloadsThisWeek,
                    first_published = creationData.FirstPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    last_published = creationData.LastPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    hearts = creationData.Hearts,
                    is_remixable = creationData.IsRemixable,
                    is_team_pick = creationData.IsTeamPick,
                    level_mode = creationData.LevelMode,
                    longest_drift = creationData.LongestDrift,
                    longest_hang_time = creationData.LongestHangTime,
                    max_humans = creationData.MaxHumans,
                    name = creationData.Name,
                    num_laps = creationData.NumLaps,
                    num_racers = creationData.NumRacers,
                    platform = creationData.Platform.ToString(),
                    player_creation_type = (creationData.Type == PlayerCreationType.STORY ? PlayerCreationType.TRACK : creationData.Type).ToString(),
                    player_id = creationData.PlayerId,
                    races_finished = creationData.RacesFinished,
                    races_started = creationData.RacesStarted,
                    races_started_this_month = creationData.RacesStartedThisMonth,
                    races_started_this_week = creationData.RacesStartedThisWeek,
                    races_won = creationData.RacesWon,
                    race_type = creationData.RaceType.ToString(),
                    rank = (int)Sql.Window.RowNumber(f => f.OrderBy(creationData.Points)),
                    rating_down = creationData.RatingDown,
                    rating_up = creationData.RatingUp,
                    scoreboard_mode = creationData.ScoreboardMode,
                    speed = creationData.Speed.ToString(),
                    tags = creationData.Tags,
                    track_theme = creationData.TrackTheme,
                    unique_racer_count = creationData.UniqueRacers,
                    updated_at = creationData.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    username = creationData.Username,
                    user_tags = creationData.UserTags,
                    version = creationData.Version,
                    views = creationData.Views,
                    views_last_week = creationData.ViewsLastWeek,
                    views_this_week = creationData.ViewsThisWeek,
                    votes = creationData.Votes,
                    weapon_set = creationData.WeaponSet,
                    hearted_by_me = (requestedBy == null) ? "false" : creationData.IsHeartedByMe(requestedBy.UserId).ToString().ToLower(),
                    queued_by_me = (requestedBy == null) ? "false" : creationData.IsBookmarkedByMe(requestedBy.UserId).ToString().ToLower(),
                    reviewed_by_me = (requestedBy == null) ? "false" : creationData.IsReviewedByMe(requestedBy.UserId).ToString().ToLower(),
                })
                .FirstOrDefault(match => match.id == id);
            var photos = database.PlayerCreations
                .Where(match => match.TrackId == id && match.Type == PlayerCreationType.PHOTO)
                .OrderByDescending(match => match.CreatedAt)
                .Select(photo => new Photo
                {
                    id = photo.PlayerCreationId
                });
            var scores = database.Scores
                .Where(match => match.SubKeyId == id)
                .Select(score => new SubLeaderboardPlayer
                {
                    player_id = score.PlayerId,
                    username = score.Username,
                    rank = (int)Sql.Window.RowNumber(f => track.scoreboard_mode == 1 ? f.OrderBy(score.FinishTime) : 
                                                              f.OrderByDesc(score.Points)),
                    score = score.Points,
                    finish_time = score.FinishTime
                })
                .ToLinqToDB();
            var comments = database.PlayerCreationComments
                .Where(match => match.PlayerCreationId == id)
                .OrderByDescending(match => match.CreatedAt)
                .Select(comment => new Comment
                {
                    id = comment.Id,
                    player_id = comment.PlayerId,
                    username = comment.Username,
                    body = comment.Body,
                    rating_up = comment.RatingUp,
                    rated_by_me = false,
                    updated_at = comment.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz")
                });
            var reviews = database.PlayerCreationReviews
                .Where(match => match.PlayerCreationId == id)
                .OrderByDescending(match => match.CreatedAt)
                .Select(review => new Review
                {
                    id = review.Id,
                    content = review.Content,
                    mine = (requestedBy == null) ? "false" : review.IsMine(requestedBy.UserId).ToString().ToLower(),
                    player_creation_id = review.PlayerCreationId,
                    player_creation_name = review.PlayerCreationName,
                    player_creation_username = review.PlayerCreationUsername,
                    player_id = review.PlayerId,
                    rated_by_me = (requestedBy == null) ? "false" : review.IsRatedByMe(requestedBy.UserId).ToString().ToLower(),
                    rating_down = review.RatingDown.ToString(),
                    rating_up = review.RatingUp.ToString(),
                    username = review.Username,
                    tags = review.Tags,
                    updated_at = review.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz")
                });
            var activityLog = database.ActivityLog
                .Where(match => match.PlayerCreationId == id)
                .Include(a => a.Author)
                .OrderByDescending(match => match.CreatedAt);

            if (track == null || id < 9000)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            if (track.scoreboard_mode == 1)
                scores = scores.OrderBy(score => score.finish_time);
            else
                scores = scores.OrderByDescending(score => score.score);

            List<Activity> activityList = [];

            foreach (var activity in activityLog.Take(3).ToList())
            {
                activityList.Add(new Activity
                {
                    player_creation_id = id,
                    player_creation_hearts = track.hearts,
                    player_creation_rating_up = track.rating_up,
                    player_creation_rating_down = track.rating_down,
                    player_creation_races_started = track.races_started,
                    player_creation_username = track.username,
                    player_creation_description = track.description,
                    player_creation_name = track.name,
                    player_creation_player_id = track.player_id,
                    player_creation_associated_item_ids = track.associated_item_ids,
                    player_creation_level_mode = track.level_mode,
                    player_creation_is_team_pick = track.is_team_pick,
                    type = "player_creation_activity",
                    events = [
                        new Event
                        {
                            topic = activity.Type.ToString(),
                            type = activity.Topic,
                            details = activity.Description,
                            creator_username = activity.Author.Username ?? "",
                            creator_id = activity.AuthorId ?? 0,
                            timestamp = activity.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                            seconds_ago = TimeUtils.SecondsAgo(activity.CreatedAt),
                            tags = activity.Tags,
                            allusion_type = activity.AllusionType,
                            allusion_id = activity.AllusionId,
                            player_id = activity.PlayerId ?? 0
                        }
                    ]
                });
            }
            
            track.activities = [new Activities { total = activityLog.Count(), ActivityList = activityList }];
            track.comments = comments.Take(3).ToList();
            track.leaderboard = [new SubLeaderboard { total = scores.Count(), LeaderboardPlayersList = scores.Take(3).ToList() }];
            track.photos = [new Photos { total = photos.Count(), PhotoList = photos.Take(3).ToList() }];
            track.reviews = [new Reviews { total = reviews.Count(), ReviewList = reviews.Take(3).ToList() }];
            
            var resp = new Response<List<Track>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [
                    track
                ]
            };
            return resp.Serialize();
        }

        public static string VerifyPlayerCreations(Database database, List<int> id, List<int> offline_id)
        {
            List<PlayerCreationToVerify> processedCreations = [];
            var creations = database.PlayerCreations
                .Select(c => new
                {
                    c.PlayerCreationId,
                    c.Type,
                    c.ModerationStatus
                })
                .Where(match => id.Contains(match.PlayerCreationId))
                .ToList();
            
            foreach (int item in id)
            {
                var creation = creations.FirstOrDefault(match => match.PlayerCreationId == item);
                if (creation != null 
                    && creation.ModerationStatus != ModerationStatus.BANNED
                    && creation.ModerationStatus != ModerationStatus.ILLEGAL)
                {
                    processedCreations.Add(new PlayerCreationToVerify
                    {
                        id = item,
                        type = creation.Type.ToString(),
                        suggested_action = creation.Type == PlayerCreationType.DELETED ? "destroy" : "allow"
                    });
                }
                else
                {
                    processedCreations.Add(new PlayerCreationToVerify
                    {
                        id = item,
                        type = nameof(PlayerCreationType.TRACK),
                        suggested_action = "ban"
                    });
                }
            }
            
            var resp = new Response<List<PlayerCreationVerify>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [
                    new PlayerCreationVerify { total = processedCreations.Count, PlayerCreationsList = processedCreations }
                ]
            };
            return resp.Serialize();
        }

        public static string GetPlanet(Database database, int player_id)
        {
            var planet = database.PlayerCreations.FirstOrDefault(match => match.PlayerId == player_id && match.Type == PlayerCreationType.PLANET);

            if (planet == null)
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
                response = [new Planet { id = planet.PlayerCreationId }]
            };
            return resp.Serialize();
        }

        public static string GetPlanetProfile(Database database, int player_id)
        {
            var planet = database.PlayerCreations
                .FirstOrDefault(match => match.PlayerId == player_id && match.Type == PlayerCreationType.PLANET);

            if (planet == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            var creations = database.PlayerCreations
                .Where(match => match.PlayerId == player_id && match.Type == PlayerCreationType.TRACK && !match.IsMNR)
                .Select(track => new Track
                {
                    id = track.PlayerCreationId,
                    ai = track.AI,
                    associated_item_ids = track.AssociatedItemIds,
                    auto_reset = track.AutoReset,
                    battle_friendly_fire = track.BattleFriendlyFire,
                    battle_kill_count = track.BattleKillCount,
                    battle_time_limit = track.BattleTimeLimit,
                    coolness = track.Coolness,
                    created_at = track.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    description = track.Description,
                    difficulty = track.Difficulty.ToString(),
                    dlc_keys = track.DLCKeys,
                    downloads = track.Downloads,
                    downloads_last_week = track.DownloadsLastWeek,
                    downloads_this_week = track.DownloadsThisWeek,
                    first_published = track.FirstPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    last_published = track.LastPublished.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    hearts = track.Hearts,
                    is_remixable = track.IsRemixable,
                    is_team_pick = track.IsTeamPick,
                    level_mode = track.LevelMode,
                    longest_drift = track.LongestDrift,
                    longest_hang_time = track.LongestHangTime,
                    max_humans = track.MaxHumans,
                    name = track.Name,
                    num_laps = track.NumLaps,
                    num_racers = track.NumRacers,
                    platform = track.Platform.ToString(),
                    player_creation_type = (track.Type == PlayerCreationType.STORY ? PlayerCreationType.TRACK : track.Type).ToString(),
                    player_id = track.PlayerId,
                    races_finished = track.RacesFinished,
                    races_started = track.RacesStarted,
                    races_started_this_month = track.RacesStartedThisMonth,
                    races_started_this_week = track.RacesStartedThisWeek,
                    races_won = track.RacesWon,
                    race_type = track.RaceType.ToString(),
                    rank = (int)Sql.Window.RowNumber(f => f.OrderBy(track.Points)),
                    rating_down = track.RatingDown,
                    rating_up = track.RatingUp,
                    scoreboard_mode = track.ScoreboardMode,
                    speed = track.Speed.ToString(),
                    tags = track.Tags,
                    track_theme = track.TrackTheme,
                    unique_racer_count = track.UniqueRacers,
                    updated_at = track.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                    username = track.Author.Username,
                    user_tags = track.UserTags,
                    version = track.Version,
                    views = track.Views,
                    views_last_week = track.ViewsLastWeek,
                    views_this_week = track.ViewsThisWeek,
                    votes = track.Votes,
                    weapon_set = track.WeaponSet
                })
                .ToLinqToDB()
                .ToList();

            var resp = new Response<List<Planet>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [ new Planet {
                    id = planet.PlayerCreationId,
                    name = planet.Name,
                    player_id = planet.PlayerId,
                    username = planet.Username,
                    tracks = new Tracks {
                        total = creations.Count,
                        TrackList = creations
                    }
                } ]
            };
            return resp.Serialize();
        }
    }
}
