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
using GameServer.Models.Common;
using AutoMapper.QueryableExtensions;
using GameServer.Models.Profiles;
using System.Data;

namespace GameServer.Implementation.Player_Creation
{
    public class PlayerCreationsImpl
    {
        public static string UpdatePlayerCreation(Database database, Guid SessionID, Models.Request.PlayerCreation reqCreation, int id = 0)
        {
            var session = SessionImpl.GetSession(SessionID);
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

            var dbCreation = database.PlayerCreations
                .Include(x => x.Author)
                .FirstOrDefault(match => 
                    match.Id == id && 
                    match.Author.UserId == user.UserId &&
                    (reqCreation.player_creation_type == PlayerCreationType.PLANET ? match.Type == PlayerCreationType.PLANET : true));

            if (dbCreation == null && reqCreation.player_creation_type == PlayerCreationType.PLANET)
            {
                return CreatePlayerCreation(database, SessionID, reqCreation);
            }

            if (dbCreation == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            database.MapperConfig.CreateMapper().Map<PlayerCreationData>(reqCreation);  // TODO: Change request naming scheme so this works
            dbCreation.LastPublished = DateTime.UtcNow;
            dbCreation.UpdatedAt = DateTime.UtcNow;
            dbCreation.Version++;

            if (dbCreation.Type == PlayerCreationType.TRACK && !session.IsMNR)
            {
                database.ActivityLog.Add(new ActivityEvent
                {
                    Author = user,
                    Type = ActivityType.player_creation_event,
                    List = ActivityList.both,
                    Topic = "player_creation_updated",
                    Description = "",
                    Creation = dbCreation,
                    CreatedAt = DateTime.UtcNow,
                    AllusionId = dbCreation.Id,
                    AllusionType = "PlayerCreation::Track"
                });
            }

            database.SaveChanges();

            if (reqCreation.player_creation_type != PlayerCreationType.PLANET)
            {
                using (var dataStream = reqCreation.data.OpenReadStream())
                using (var previewStream = reqCreation.preview.OpenReadStream())
                    UserGeneratedContentUtils.SavePlayerCreation(dbCreation.Id, dataStream, previewStream);
            }
            else
            {
                using (var dataStream = reqCreation.data.OpenReadStream())
                    UserGeneratedContentUtils.SavePlayerCreation(dbCreation.Id, dataStream);
            }

            if (reqCreation.player_creation_type == PlayerCreationType.PLANET)
            {
                var planetUpdateResp = new Response<List<Planet>>
                {
                    status = new ResponseStatus { id = 0, message = "Successful completion" },
                    response = [new Planet { Id = id }]
                };
                return planetUpdateResp.Serialize();
            }

            var resp = new Response<List<Models.Response.PlayerCreation>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [new Models.Response.PlayerCreation { Id = id }]
            };
            return resp.Serialize();
        }

        public static string CreatePlayerCreation(Database database, Guid SessionID, Models.Request.PlayerCreation creation)
        {
            var session = SessionImpl.GetSession(SessionID);
            int id = database.PlayerCreations.Count(match => match.Type != PlayerCreationType.STORY) + 10000;   // TODO?
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);
            var deletedCreation = database.PlayerCreations.FirstOrDefault(match => match.Type == PlayerCreationType.DELETED 
                && ((match.Name == creation.player_creation_type.ToString() && match.IsMNR == session.IsMNR
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

            int quota = database.PlayerCreations.Count(match => match.Author.UserId == user.UserId 
                && match.Type != PlayerCreationType.PHOTO && match.Type != PlayerCreationType.DELETED 
                && match.IsMNR == session.IsMNR && match.Platform == session.Platform);
            if (quota >= user.Quota && creation.player_creation_type != PlayerCreationType.PHOTO)
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
                id = deletedCreation.Id;
                database.Remove(deletedCreation);
            }
            else
            {
                //Check if id is not used by something...
                bool IsAvailable = false;
                while (!IsAvailable)
                {
                    var check = database.PlayerCreations.FirstOrDefault(match => match.Id == id);
                    IsAvailable = check == null || (check != null && check.Type == PlayerCreationType.DELETED);
                    if (!IsAvailable) id++;
                }
            }

            var dbCreation = new PlayerCreationData // TODO: Automap
            {
                Id = id,
                AI = creation.ai,
                AssociatedCoordinates = creation.associated_coordinates,
                AssociatedItemIds = creation.associated_item_ids,
                AssociatedUsernames = creation.associated_usernames,
                AutoReset = creation.auto_reset,
                AutoTags = creation.auto_tags,
                BattleFriendlyFire = creation.battle_friendly_fire,
                BattleKillCount = creation.battle_kill_count,
                BattleTimeLimit = creation.battle_time_limit,
                CreatedAt = DateTime.UtcNow,
                Description = creation.description,
                Difficulty = creation.difficulty,
                DLCKeys = creation.dlc_keys,
                FirstPublished = DateTime.UtcNow,
                IsRemixable = creation.is_remixable,
                IsTeamPick = creation.is_team_pick,
                LastPublished = DateTime.UtcNow,
                LevelMode = creation.level_mode,
                LongestDrift = creation.longest_drift,
                LongestHangTime = creation.longest_hang_time,
                MaxHumans = creation.max_humans,
                Name = creation.name,
                NumLaps = creation.num_laps,
                NumRacers = creation.num_racers,
                Platform = creation.platform,
                Author = user,
                RaceType = creation.race_type,
                RequiresDLC = creation.requires_dlc,
                ScoreboardMode = creation.scoreboard_mode,
                Speed = creation.speed,
                Tags = creation.tags,
                TrackTheme = creation.track_theme,
                Type = creation.player_creation_type,
                UpdatedAt = DateTime.UtcNow,
                UserTags = creation.user_tags,
                WeaponSet = creation.weapon_set,
                TrackId = creation.track_id == 0 ? id : creation.track_id,
                Version = 1,
                //MNR
                IsMNR = session.IsMNR,
                // TODO: optimise below
                ParentCreation = creation.parent_creation_id < 10000 ? database.PlayerCreations.FirstOrDefault(match => match.Id == creation.parent_creation_id) : null,    // TODO: Why is there <10000 here, bug?
                ParentPlayer = database.Users.FirstOrDefault(match => match.UserId == creation.parent_player_id),
                OriginalPlayer = database.Users.FirstOrDefault(match => match.UserId == creation.original_player_id, user),
                BestLapTime = creation.best_lap_time
            };

            database.PlayerCreations.Add(dbCreation);

            if (creation.player_creation_type == PlayerCreationType.TRACK && !session.IsMNR)
            {
                database.ActivityLog.Add(new ActivityEvent
                {
                    Author = user,
                    Type = ActivityType.player_creation_event,
                    List = ActivityList.both,
                    Topic = "player_creation_created",
                    Description = "",
                    Creation =  dbCreation,
                    CreatedAt = DateTime.UtcNow,
                    AllusionId = id,
                    AllusionType = "PlayerCreation::Track"
                });
            }

            database.SaveChanges(); // TODO: This might also need to come after PlayerCreations.Add


            if (creation.player_creation_type == PlayerCreationType.PHOTO)
            {
                using (var dataStream = creation.data.OpenReadStream())
                    UserGeneratedContentUtils.SavePlayerPhoto(id, dataStream);
            }
            else if (creation.player_creation_type == PlayerCreationType.PLANET)
            {
                using (var dataStream = creation.data.OpenReadStream())
                    UserGeneratedContentUtils.SavePlayerCreation(id, dataStream);
            }
            else
            {
                using (var dataStream = creation.data.OpenReadStream())
                using (var previewStream = creation.preview.OpenReadStream())
                    UserGeneratedContentUtils.SavePlayerCreation(id, dataStream, previewStream);
            }

            if (creation.player_creation_type == PlayerCreationType.PLANET)
            {
                var planetUpdateResp = new Response<List<Planet>>
                {
                    status = new ResponseStatus { id = 0, message = "Successful completion" },
                    response = [new Planet { Id = id }]
                };
                return planetUpdateResp.Serialize();
            }

            var resp = new Response<List<Models.Response.PlayerCreation>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [new Models.Response.PlayerCreation { Id = id }]
            };
            return resp.Serialize();
        }

        public static string RemovePlayerCreation(Database database, Guid SessionID, int id)
        {
            var session = SessionImpl.GetSession(SessionID);
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

            var creation = database.PlayerCreations.FirstOrDefault(match => match.Id == id && match.Author.UserId == user.UserId);

            if (creation == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            database.PlayerCreations.Remove(creation);

            foreach (var item in database.PlayerCreations.Where(match => match.TrackId == id).ToList())
            {
                var Photo = database.PlayerCreations.FirstOrDefault(match => match.Id == item.Id);  // TODO: Redundant DB fetch (use item)?
                Photo.TrackId = 4912;
            }

            foreach (var item in database.ActivityLog.Where(match => match.Id == id).ToList())    // TODO: Cascade delete in EF?
            {
                var Activity = database.ActivityLog.FirstOrDefault(match => match.Id == item.Id);
                database.ActivityLog.Remove(Activity);  // TODO: RemoveRange
            }

            database.SaveChanges();

            UserGeneratedContentUtils.RemovePlayerCreation(id);

            database.PlayerCreations.Add(new PlayerCreationData // TODO: Can we not just update the original PlayerCreation object
            {
                Id = id,
                Name = creation.Type.ToString(),
                Author = user,
                Platform = creation.Platform,
                Type = PlayerCreationType.DELETED,
                IsMNR = creation.IsMNR
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
            var session = SessionImpl.GetSession(SessionID);
            var creation = database.PlayerCreations
                .Include(x => x.Downloads)
                .Include(x => x.RacesStarted)
                .Include(x => x.UniqueRacers)
                .Include(x => x.Hearts)
                .Include(x => x.Points)
                .Include(x => x.Ratings)
                .Include(x => x.Views)
                .Include(x => x.Author)
                .Include(x => x.OriginalPlayer)
                .Include(x => x.ParentCreation)
                .Include(x => x.ParentPlayer)
                .FirstOrDefault(match => match.Id == id);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);

            if (creation == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            if (user != null)
            {
                if (IsCounted && !download)
                {
                    database.PlayerCreationViews.Add(new PlayerCreationView { Creation = creation, ViewedAt = DateTime.UtcNow });
                    database.SaveChanges();
                }

                if (IsCounted && download)
                { 
                    // TODO: Can we consolidate these adds into less calls?
                    var uniqueRacer = database.PlayerCreationUniqueRacers.FirstOrDefault(match => match.User.UserId == user.UserId);
                    database.PlayerCreationDownloads.Add(new PlayerCreationDownload { Creation = creation, DownloadedAt = DateTime.UtcNow });
                    database.PlayerCreationPoints.Add(new PlayerCreationPoint { Creation = creation, Player = creation.Author, Platform = creation.Platform, Type = creation.Type, CreatedAt = DateTime.UtcNow, Amount = 100 });

                    if (uniqueRacer == null)
                    {
                        database.PlayerCreationUniqueRacers.Add(new PlayerCreationUniqueRacer
                        {
                            User = user,
                            Creation = creation,
                            Version = creation.Version  // TODO: This is redundant, can we just reference the creation record?
                        });
                        if (!session.IsMNR)
                        {
                            database.ActivityLog.Add(new ActivityEvent
                            {
                                Author = user,
                                Type = ActivityType.player_creation_event,
                                List = ActivityList.activity_log,
                                Topic = "player_creation_downloaded",
                                Description = "",
                                Creation = creation,
                                CreatedAt = DateTime.UtcNow,
                                AllusionId = creation.Id,
                                AllusionType = "PlayerCreation::Track"
                            });
                            database.ActivityLog.Add(new ActivityEvent
                            {
                                Author = user,
                                Type = ActivityType.player_creation_event,
                                List = ActivityList.activity_log,
                                Topic = "player_creation_played",
                                Description = "",
                                Creation = creation,
                                CreatedAt = DateTime.UtcNow,
                                AllusionId = creation.Id,
                                AllusionType = "PlayerCreation::Track"
                            });
                        }
                    }

                    if (uniqueRacer != null && uniqueRacer.Version != creation.Version)
                    {
                        database.PlayerCreationDownloads.Add(new PlayerCreationDownload { Creation = creation, DownloadedAt = DateTime.UtcNow });
                        if (!session.IsMNR)
                        {
                            database.ActivityLog.Add(new ActivityEvent
                            {
                                Author = user,
                                Type = ActivityType.player_creation_event,
                                List = ActivityList.activity_log,
                                Topic = "player_creation_downloaded",
                                Description = "",
                                Creation = creation,
                                CreatedAt = DateTime.UtcNow,
                                AllusionId = creation.Id,
                                AllusionType = "PlayerCreation::Track"
                            });
                            database.ActivityLog.Add(new ActivityEvent
                            {
                                Author = user,
                                Type = ActivityType.player_creation_event,
                                List = ActivityList.activity_log,
                                Topic = "player_creation_played",
                                Description = "",
                                Creation = creation,
                                CreatedAt = DateTime.UtcNow,
                                AllusionId = creation.Id,
                                AllusionType = "PlayerCreation::Track"
                            });
                        }
                        uniqueRacer.Version = creation.Version;
                    }

                    database.SaveChanges();
                }
            }

            // TODO
            //var allPlayerCreations = database.PlayerCreations           // I dont really know how to optimise this any more without breaking the rank system?
            //                .Include(x => x.Points)
            //                .Where(match => match.Type == Creation.Type)
            //                .OrderByDescending(match => match.Points.Count())
            //                .Select(match => match.PlayerCreationId)    // To optimise the amount of data we get back, this is a particularly tricky situation
            //                .AsEnumerable();                            // Evaluate our query, find row index after

            var resp = new Response<List<Models.Response.PlayerCreation>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [
                    database.MapperConfig.CreateMapper().Map<Models.Response.PlayerCreation>(creation)
                ]
            };
            return resp.Serialize();
        }

        public static string PlayerCreationsFriendsPublished(Database database, string usernameFilter, PlayerCreationType type)
        {
            var resp = new Response<List<PlayerCreations>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                // TODO: !!! IMPORTANT !!! We need a one-to-many to PlayerCreations table here, commented out until implemented
                response = [new PlayerCreations    // TODO: Does this not need playercreation objects?
                {
                    //FriendsPublished = database.Users
                    //    .Where(user => usernameFilter.Contains(user.Username))
                    //    .SelectMany(user => user.Creations.Where(creation => creation.PlayerId == user.UserId && creation.Type == type))
                    //    .Any()
                }]
            };
            return resp.Serialize();
        }

        public static string SearchPlayerCreations(Database database, Guid SessionID, int page, int per_page, SortColumn sort_column, SortOrder sort_order,
            int limit, Platform platform, Filters filters, string keyword = null, bool TeamPicks = false, 
            bool LuckyDip = false, bool IsMNR = false)
        {
            var creationQuery = database.PlayerCreations     // TODO: Is it an issue someone might be able to fudge the entire database out like this?
                .Include(x => x.Downloads)
                .Include(x => x.RacesStarted)
                .Include(x => x.UniqueRacers)
                .Include(x => x.Hearts)
                .Include(x => x.Points)
                .Include(x => x.Ratings)
                .Include(x => x.Views)
                .Include(x => x.Author)
                .Where(match => match.Platform == platform && match.IsMNR == IsMNR);
            var session = SessionImpl.GetSession(SessionID);
            var User = database.Users.FirstOrDefault(match => match.Username == session.Username);

            if (filters.username == null && filters.id == null && filters.player_id == null)
                creationQuery = creationQuery.Where(match => match.Type == filters.player_creation_type);

            //filters
            if (filters.username != null)
                creationQuery = creationQuery.Where(match => match.Type == filters.player_creation_type && filters.username.Any(x => match.Author.Username == x));

            if (filters.id != null)
                creationQuery = creationQuery.Where(match => (match.Type == filters.player_creation_type || match.Type == PlayerCreationType.STORY) && filters.id.Any(x => match.Id.ToString() == x));

            if (filters.player_id != null)
                creationQuery = creationQuery.Where(match => match.Type == filters.player_creation_type && filters.player_id.Any(x => match.Author.UserId.ToString() == x));

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
                            creationQuery.OrderBy(match => match.RacesStarted.Count(match => match.StartedAt >= DateTime.UtcNow.AddDays(-7) && match.StartedAt <= DateTime.UtcNow)) : 
                            creationQuery.OrderByDescending(match => match.RacesStarted.Count(match => match.StartedAt >= DateTime.UtcNow.AddDays(-7) && match.StartedAt <= DateTime.UtcNow));
                    break;
                case SortColumn.races_started_this_month:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.RacesStarted.Count(match => match.StartedAt >= DateTime.UtcNow.AddMonths(-1) && match.StartedAt <= DateTime.UtcNow)) :
                            creationQuery.OrderByDescending(match => match.RacesStarted.Count(match => match.StartedAt >= DateTime.UtcNow.AddMonths(-1) && match.StartedAt <= DateTime.UtcNow));
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
                            creationQuery.OrderBy(match => match.Ratings.Count(match => match.Type == RatingType.YAY && match.RatedAt >= DateTime.UtcNow.AddDays(-7) && match.RatedAt <= DateTime.UtcNow)) : 
                            creationQuery.OrderByDescending(match => match.Ratings.Count(match => match.Type == RatingType.YAY && match.RatedAt >= DateTime.UtcNow.AddDays(-7) && match.RatedAt <= DateTime.UtcNow));
                    break;
                case SortColumn.rating_up_this_month:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.Ratings.Count(match => match.Type == RatingType.YAY && match.RatedAt >= DateTime.UtcNow.AddMonths(-1) && match.RatedAt <= DateTime.UtcNow)) :
                            creationQuery.OrderByDescending(match => match.Ratings.Count(match => match.Type == RatingType.YAY && match.RatedAt >= DateTime.UtcNow.AddMonths(-1) && match.RatedAt <= DateTime.UtcNow));
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
                            creationQuery.OrderBy(match => match.Hearts.Count(match => match.HeartedAt >= DateTime.UtcNow.AddDays(-7) && match.HeartedAt <= DateTime.UtcNow)) :
                            creationQuery.OrderByDescending(match => match.Hearts.Count(match => match.HeartedAt >= DateTime.UtcNow.AddDays(-7) && match.HeartedAt <= DateTime.UtcNow));
                    break;
                case SortColumn.hearts_this_month:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.Hearts.Count(match => match.HeartedAt >= DateTime.UtcNow.AddMonths(-1) && match.HeartedAt <= DateTime.UtcNow)) :
                            creationQuery.OrderByDescending(match => match.Hearts.Count(match => match.HeartedAt >= DateTime.UtcNow.AddMonths(-1) && match.HeartedAt <= DateTime.UtcNow));
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
                            creationQuery.OrderBy(match => match.Points.Where(match => match.CreatedAt >= DateTime.UtcNow.Date && match.CreatedAt <= DateTime.UtcNow).Sum(p => p.Amount)) :
                            creationQuery.OrderByDescending(match => match.Points.Where(match => match.CreatedAt >= DateTime.UtcNow.Date && match.CreatedAt <= DateTime.UtcNow).Sum(p => p.Amount));
                    break;
                case SortColumn.points_yesterday:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.Points.Where(match => match.CreatedAt >= DateTime.UtcNow.AddDays(-1).Date && match.CreatedAt <= DateTime.UtcNow.Date).Sum(p => p.Amount)) :
                            creationQuery.OrderByDescending(match => match.Points.Where(match => match.CreatedAt >= DateTime.UtcNow.AddDays(-1).Date && match.CreatedAt <= DateTime.UtcNow.Date).Sum(p => p.Amount));
                    break;
                case SortColumn.points_this_week:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderByDescending(match => match.Points.Where(match => match.CreatedAt >= DateTime.UtcNow.AddDays(-7) && match.CreatedAt <= DateTime.UtcNow).Sum(p => p.Amount)) :
                            creationQuery.OrderBy(match => match.Points.Where(match => match.CreatedAt >= DateTime.UtcNow.AddDays(-7) && match.CreatedAt <= DateTime.UtcNow).Sum(p => p.Amount));
                    break;
                case SortColumn.points_last_week:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.Points.Where(match => match.CreatedAt >= DateTime.UtcNow.AddDays(-14) && match.CreatedAt <= DateTime.UtcNow.AddDays(-7)).Sum(p => p.Amount)) :
                            creationQuery.OrderByDescending(match => match.Points.Where(match => match.CreatedAt >= DateTime.UtcNow.AddDays(-14) && match.CreatedAt <= DateTime.UtcNow.AddDays(-7)).Sum(p => p.Amount));
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
                            creationQuery.OrderBy(match => match.Downloads.Count(match => match.DownloadedAt >= DateTime.UtcNow.AddDays(-7) && match.DownloadedAt <= DateTime.UtcNow)) :
                            creationQuery.OrderByDescending(match => match.Downloads.Count(match => match.DownloadedAt >= DateTime.UtcNow.AddDays(-7) && match.DownloadedAt <= DateTime.UtcNow));
                    break;
                case SortColumn.downloads_last_week:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.Downloads.Count(match => match.DownloadedAt >= DateTime.UtcNow.AddDays(-14) && match.DownloadedAt <= DateTime.UtcNow.AddDays(-7))) :
                            creationQuery.OrderByDescending(match => match.Downloads.Count(match => match.DownloadedAt >= DateTime.UtcNow.AddDays(-14) && match.DownloadedAt <= DateTime.UtcNow.AddDays(-7)));
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
                            creationQuery.OrderBy(match => match.Views.Count(match => match.ViewedAt >= DateTime.UtcNow.AddDays(-7) && match.ViewedAt <= DateTime.UtcNow)) :
                            creationQuery.OrderByDescending(match => match.Views.Count(match => match.ViewedAt >= DateTime.UtcNow.AddDays(-7) && match.ViewedAt <= DateTime.UtcNow));
                    break;
                case SortColumn.views_last_week:
                    creationQuery =
                        sort_order == SortOrder.asc ?
                            creationQuery.OrderBy(match => match.Views.Count(match => match.ViewedAt >= DateTime.UtcNow.AddDays(-14) && match.ViewedAt <= DateTime.UtcNow.AddDays(-7))) :
                            creationQuery.OrderByDescending(match => match.Views.Count(match => match.ViewedAt >= DateTime.UtcNow.AddDays(-14) && match.ViewedAt <= DateTime.UtcNow.AddDays(-7)));
                    break;
            }

            if (LuckyDip)
                creationQuery = creationQuery.OrderBy(match => EF.Functions.Random());  // TODO: is session.RandomSeed required? Will this even add onto the above, needs ThenBy?

            // TODO: sort_order

            var pageStart = PageCalculator.GetPageStart(page, per_page);
            var pageEnd = PageCalculator.GetPageStart(page, per_page);
            var total = creationQuery.Count();
            var totalPages = PageCalculator.GetTotalPages(total, per_page);

            var creations = creationQuery
                .Skip(pageStart)
                .Take(per_page)
                .ProjectTo<Models.Response.PlayerCreation>(database.MapperConfig)   // TODO: Can this go at beginning?
                .ToList();

            // TODO: !!! MOVE TO AUTOMAPPER !!!
            //var allPlayerCreations = database.PlayerCreations           // I dont really know how to optimise this any more without breaking the rank system?
            //                .Include(x => x.Points)
            //                .Where(match => match.Type == filters.player_creation_type)
            //                .OrderByDescending(match => match.Points.Count())
            //                .Select(match => match.PlayerCreationId)    // To optimise the amount of data we get back, this is a particularly tricky situation
            //                .AsEnumerable();                            // Evaluate our query, find row index after

            var resp = new Response<List<PlayerCreations>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [
                    new PlayerCreations
                    {
                        Page = page,
                        RowEnd = pageEnd,
                        RowStart = pageStart,
                        Total = total,
                        TotalPages = totalPages,
                        PlayerCreationsList = creations
                    }
                ]
            };
            return resp.Serialize();
        }

        public static string Mine(Database database, Guid SessionID, int page, int per_page, SortColumn sort_column, SortOrder sort_order,
            int limit, Filters filters, string keyword = null, Platform? platformOverride = null) 
        {
            var session = SessionImpl.GetSession(SessionID);
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
            var user = database.Users.FirstOrDefault(match => match.Username == username);
            var photosQuery = database.PlayerCreations
                                .Include(x => x.Author)
                                .Where(match =>
                                    match.Type == PlayerCreationType.PHOTO &&
                                    (associated_usernames != null ? match.AssociatedUsernames.Contains(associated_usernames) : true) &&
                                    (username != null ? match.Author.UserId == user.UserId : true) &&
                                    (track_id != null ? match.TrackId == track_id : true))
                                .OrderByDescending(match => match.CreatedAt);

            var pageStart = PageCalculator.GetPageStart(page, per_page);
            var pageEnd = PageCalculator.GetPageStart(page, per_page);
            var total = photosQuery.Count();
            var totalPages = PageCalculator.GetTotalPages(total, per_page);

            var photos = photosQuery
                .Skip(pageStart)
                .Take(per_page)
                .ProjectTo<Photo>(database.MapperConfig)
                .ToList();

            var resp = new Response<List<Photos>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [
                    new Photos {
                        Total = total,
                        CurrentPage = page,
                        RowStart = pageStart,
                        RowEnd = pageEnd,
                        TotalPages = totalPages,
                        PhotoList = photos
                    }
                ]
            };
            return resp.Serialize();
        }

        public static string GetTrackProfile(Database database, Guid SessionID, int id)
        {
            var session = SessionImpl.GetSession(SessionID);
            var requestedBy = database.Users
                .FirstOrDefault(match => match.Username == session.Username);

            var track = database.PlayerCreations
                .Include(x => x.Hearts) // TODO: Can we cut down some of these includes?
                .Include(x => x.Ratings)
                .Include(x => x.RacesStarted)
                .Include(x => x.Author)
                .Include(x => x.Points)
                .Include(x => x.Downloads)
                .Include(x => x.UniqueRacers)
                .Include(x => x.Views)
                .Include(x => x.ScoreboardMode == 1 ? x.Scores.OrderBy(x => x.FinishTime) : x.Scores.OrderByDescending(x => x.Points))
                .Include(x => x.Comments.OrderByDescending(x => x.CreatedAt))
                .Include(x => x.Bookmarks)
                .Include(x => x.Reviews.OrderByDescending(x => x.CreatedAt))
                .Include(x => x.ActivityLog.OrderByDescending(x => x.CreatedAt))
                .FirstOrDefault(match => match.Id == id);
            var photoList = database.PlayerCreations
                .Where(match => match.TrackId == id && match.Type == PlayerCreationType.PHOTO)
                .OrderByDescending(match => match.CreatedAt)
                .Take(3)
                .ProjectTo<Photo>(database.MapperConfig)
                .ToList();

            List<SubLeaderboardPlayer> scoresList = [];
            List<Comment> commentsList = [];
            List<Review> reviewsList = [];
            List<Activity> activityList = [];

            if (track == null || id < 9000)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -620, message = "No player creation exists for the given ID" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            foreach (var score in track.Scores.Take(3))   // TODO
            {
                scoresList.Add(new SubLeaderboardPlayer
                {
                    PlayerId = score.User.UserId,
                    Username = score.User.Username,
                    Rank = score.GetRank(track.ScoreboardMode == 1 ? SortColumn.finish_time : SortColumn.score),
                    Score = score.Points,
                    FinishTime = score.FinishTime
                });
            }

            foreach (var comment in track.Comments.Take(3))   // TODO
            {
                if (comment != null)
                {
                    commentsList.Add(new Comment    // TODO: Automapper
                    {
                        Id = comment.Id,
                        PlayerId = comment.Player.UserId,
                        Username = comment.Player.Username,
                        Body = comment.Body,
                        RatingUp = 0,
                        RatedByMe = false,
                        UpdatedAt = comment.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz")
                    });
                }
            }

            foreach (var review in track.Reviews.Take(3))  // TODO
            {
                if (review != null)
                {
                    reviewsList.Add(new Review    // TODO: Automapper
                    {
                        Id = review.Id,
                        Content = review.Content,
                        Mine = "false",
                        PlayerCreationId = review.Creation.Id,
                        PlayerCreationName = review.Creation.Name,
                        PlayerCreationUsername = review.Creation.Author.Username,
                        PlayerId = review.User.UserId,
                        RatedByMe = "false",
                        RatingDown = review.ReviewRatings.Count(match => match.Type == RatingType.BOO).ToString(),
                        RatingUp = review.ReviewRatings.Count(match => match.Type == RatingType.YAY).ToString(),
                        Username = review.User.Username,
                        Tags = review.Tags,
                        UpdatedAt = review.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz")
                    });
                }
            }

            foreach (var activity in track.ActivityLog.Take(3)) // TODO
            {
                activityList.Add(new Activity    // TODO: Automapper
                {
                    PlayerCreationId = id,
                    PlayerCreationHearts = track.Hearts.Count(),
                    PlayerCreationRatingUp = track.Ratings.Count(match => match.Type == RatingType.YAY),
                    PlayerCreationRatingDown = track.Ratings.Count(match => match.Type == RatingType.BOO),
                    PlayerCreationRacesStarted = track.RacesStarted.Count(),
                    PlayerCreationUsername = track.Author.Username,
                    PlayerCreationDescription = track.Description,
                    PlayerCreationName = track.Name,
                    PlayerCreationPlayerId = track.Author.UserId,
                    PlayerCreationAssociatedItemIds = track.AssociatedItemIds,
                    PlayerCreationLevelMode = track.LevelMode,
                    PlayerCreationIsTeamPick = track.IsTeamPick,
                    Type = "player_creation_activity",
                    Events = [
                            new Event    // TODO: Automapper
                            {
                                Topic = activity.Type.ToString(),
                                Type = activity.Topic,
                                Details = activity.Description,
                                CreatorUsername = activity.Author != null ? activity.Author.Username : "",
                                CreatorId = activity.Author.UserId, // TODO: null?
                                Timestamp = activity.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                                SecondsAgo = (int)new TimeSpan(DateTime.UtcNow.Ticks - activity.CreatedAt.Ticks).TotalSeconds,
                                Tags = activity.Tags,
                                AllusionType = activity.AllusionType,
                                AllusionId = activity.AllusionId,
                                PlayerId = activity.Player.UserId
                            }
                        ]
                });
            }

            //var allPlayerCreations = database.PlayerCreations           // I dont really know how to optimise this any more without breaking the rank system?
            //                .Include(x => x.Points)
            //                .Where(match => match.Type == track.Type)
            //                .OrderByDescending(match => match.Points.Count())
            //                .Select(match => match.Id)    // To optimise the amount of data we get back, this is a particularly tricky situation
            //                .AsEnumerable();                            // Evaluate our query, find row index after

            var resp = new Response<List<Track>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [    // TODO TODO
                    database.MapperConfig.CreateMapper().Map<Track>(track)
                ]
            };
            return resp.Serialize();
        }

        public static string VerifyPlayerCreations(Database database, List<int> id, List<int> offline_id)
        {   // TODO: !!! IMPORTANT !!! Change List<int>s to int array so it can be fed to EF
            var creations = database.PlayerCreations
                .Where(match => id.Contains(match.Id))
                .ProjectTo<PlayerCreationToVerify>(database.MapperConfig)
                .ToList();

            var resp = new Response<List<PlayerCreationVerify>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [
                    new PlayerCreationVerify { Total = creations.Count, PlayerCreationsList = creations }
                ]
            };
            return resp.Serialize();
        }

        public static string GetPlanet(Database database, int player_id)
        {
            var planet = database.PlayerCreations
                .FirstOrDefault(match => match.Author.UserId == player_id && match.Type == PlayerCreationType.PLANET);

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
                response = [new Planet { Id = planet.Id }]
            };
            return resp.Serialize();
        }

        public static string GetPlanetProfile(Database database, int player_id)
        {
            var planet = database.PlayerCreations
                .Include(x => x.Author)
                .FirstOrDefault(match => match.Author.UserId == player_id && match.Type == PlayerCreationType.PLANET);

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
                .Include(x => x.Downloads)
                .Include(x => x.Hearts)
                .Include(x => x.RacesStarted)
                .Include(x => x.Ratings)
                .Include(x => x.UniqueRacers)
                .Include(x => x.Author)
                .Include(x => x.Views)
                .Where(match => match.Author.UserId == player_id && match.Type == PlayerCreationType.TRACK && !match.IsMNR)
                .ProjectTo<Track>(database.MapperConfig)
                .ToList();

            //var allPlayerCreations = database.PlayerCreations           // I dont really know how to optimise this any more without breaking the rank system?
            //                .Include(x => x.Points)
            //                .Where(match => match.Type == PlayerCreationType.TRACK)
            //                .OrderByDescending(match => match.Points.Count())
            //                .Select(match => match.PlayerCreationId)    // To optimise the amount of data we get back, this is a particularly tricky situation
            //                .AsEnumerable();                            // Evaluate our query, find row index after

            var resp = new Response<List<Planet>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [ new Planet {
                    Id = planet.Id,
                    Name = planet.Name,
                    PlayerId = planet.Author.UserId,
                    Username = planet.Author.Username,
                    Tracks = new Tracks {
                        Total = planet.Author.TotalTracks,  // TODO: Can we not just use the count from our results?
                        TrackList = creations
                    }
                } ]
            };
            return resp.Serialize();
        }
    }
}
