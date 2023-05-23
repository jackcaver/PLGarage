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
            Creation.UniqueRacerCount = PlayerCreation.unique_racer_count;
            Creation.UpdatedAt = DateTime.UtcNow;
            Creation.UserTags = PlayerCreation.user_tags;
            Creation.WeaponSet = PlayerCreation.weapon_set;
            Creation.Votes = PlayerCreation.votes;
            Creation.Version++;

            database.SaveChanges();

            if (PlayerCreation.player_creation_type != PlayerCreationType.PLANET)
                UserGeneratedContentUtils.SavePlayerCreation(Creation.PlayerCreationId,
                   PlayerCreation.data.OpenReadStream(),
                   PlayerCreation.preview.OpenReadStream());
            else
                UserGeneratedContentUtils.SavePlayerCreation(Creation.PlayerCreationId, PlayerCreation.data.OpenReadStream());

            if (PlayerCreation.player_creation_type == PlayerCreationType.PLANET)
            {
                var planetUpdateResp = new Response<List<planet>>
                {
                    status = new ResponseStatus { id = 0, message = "Successful completion" },
                    response = new List<planet> { new planet { id = id } }
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

            int quota = database.PlayerCreations.Count(match => match.PlayerId == user.UserId && match.Type == PlayerCreationType.TRACK);
            if (quota >= user.Quota && Creation.player_creation_type == PlayerCreationType.TRACK)
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
                UniqueRacerCount = Creation.unique_racer_count,
                UpdatedAt = DateTime.UtcNow,
                UserTags = Creation.user_tags,
                WeaponSet = Creation.weapon_set,
                Votes = Creation.votes,
                TrackId = Creation.track_id == 0 ? id : Creation.track_id,
                Version = 1
            });

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
                var planetUpdateResp = new Response<List<planet>>
                {
                    status = new ResponseStatus { id = 0, message = "Successful completion" },
                    response = new List<planet> { new planet { id = id } }
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

        public static string GetPlayerCreation(Database database, Guid SessionID, int id, bool download = false)
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
                if (User.UserId != Creation.PlayerId && !download)
                {
                    database.PlayerCreationViews.Add(new PlayerCreationView { PlayerCreationId = Creation.PlayerCreationId, ViewedAt = DateTime.UtcNow });
                    database.SaveChanges();
                }

                if (User.UserId != Creation.PlayerId && download)
                {
                    database.PlayerCreationDownloads.Add(new PlayerCreationDownload { PlayerCreationId = Creation.PlayerCreationId, DownloadedAt = DateTime.UtcNow });
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
                        weapon_set = Creation.WeaponSet
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
            int limit, Platform platform, Filters filters, string keyword = null, bool TeamPicks = false, bool LuckyDip = false)
        {
            var Creations = new List<PlayerCreationData> { };

            if (filters.username == null && filters.id == null)
                Creations = database.PlayerCreations.Where(match => match.Type == filters.player_creation_type && match.Platform == platform).ToList();

            //filters
            if (filters.username != null)
            {
                foreach (string username in filters.username)
                {
                    var user = database.Users.FirstOrDefault(match => match.Username == username);
                    if (user != null)
                    {
                        var userTracks = database.PlayerCreations.Where(match => match.PlayerId == user.UserId
                            && match.Type == filters.player_creation_type && match.Platform == platform).ToList();
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
                        (match.Type == filters.player_creation_type || match.Type == PlayerCreationType.STORY));
                    if (Creation != null)
                        Creations.Add(Creation);
                }
            }

            if (keyword != null)
                Creations.RemoveAll(match => !match.Name.Contains(keyword));

            if (filters.race_type != null)
                Creations.RemoveAll(match => !filters.race_type.Equals(match.RaceType.ToString()));

            if (filters.tags != null && filters.tags.Count() != 0)
            {
                Creations.RemoveAll(match => match.Tags == null);
                foreach (string tag in filters.tags)
                {
                    Creations.RemoveAll(match => !match.Tags.Split(',').Contains(tag));
                }
            }

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
                        weapon_set = Creation.WeaponSet
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

            var PhotoList = new List<photo> { };
            for (int i = pageStart; i < pageEnd; i++)
            {
                var Photo = Photos[i];
                PhotoList.Add(new photo
                {
                    associated_usernames = Photo.AssociatedUsernames,
                    id = Photo.PlayerCreationId,
                    track_id = Photo.TrackId,
                    username = Photo.Username
                });
            }

            var resp = new Response<List<photos>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<photos> {
                    new photos {
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
            List<photo> PhotoList = new List<photo> { };
            List<LeaderboardPlayer> ScoresList = new List<LeaderboardPlayer> { };
            List<comment> CommentsList = new List<comment> { };
            List<review> ReviewsList = new List<review> { };

            TrackPhotos.Sort((curr, prev) => prev.CreatedAt.CompareTo(curr.CreatedAt));
            TrackComments.Sort((curr, prev) => prev.CreatedAt.CompareTo(curr.CreatedAt));
            TrackReviews.Sort((curr, prev) => prev.CreatedAt.CompareTo(curr.CreatedAt));

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
                TrackScores.Sort((curr, prev) => prev.FinishTime.CompareTo(curr.FinishTime));
            else
                TrackScores.Sort((curr, prev) => prev.Points.CompareTo(curr.Points));

            if (requestedBy == null)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            foreach (PlayerCreationData Photo in TrackPhotos.Take(3))
            {
                PhotoList.Add(new photo
                {
                    id = Photo.PlayerCreationId
                });
            }

            foreach (Score Score in TrackScores.Take(3))
            {
                ScoresList.Add(new LeaderboardPlayer
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
                    CommentsList.Add(new comment
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
                    ReviewsList.Add(new review
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

            var resp = new Response<List<track>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<track> {
                    new track
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
                        hearted_by_me = Track.IsHeartedByMe(requestedBy.UserId).ToString().ToLower(),
                        queued_by_me = Track.IsBookmarkedByMe(requestedBy.UserId).ToString().ToLower(),
                        reviewed_by_me = Track.IsReviewedByMe(requestedBy.UserId).ToString().ToLower(),
                        activities = new List<activities> { new activities { total = 0 } },
                        comments = CommentsList,
                        leaderboard = new List<leaderboard> { new leaderboard { total = TrackScores.Count, LeaderboardPlayersList = ScoresList } },
                        photos = new List<photos> { new photos { total = PhotoList.Count, PhotoList = PhotoList } },
                        reviews = new List<reviews> { new reviews { total = TrackReviews.Count, ReviewList = ReviewsList } }
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
                if (creation != null)
                {
                    creations.Add(new PlayerCreationToVerify
                    {
                        id = item,
                        type = creation.Type.ToString(),
                        suggested_action = "allow"
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

            var resp = new Response<List<planet>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<planet> { new planet { id = Planet.PlayerCreationId } }
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
            var trackList = new List<track> { };
            var creations = database.PlayerCreations.Where(match => match.PlayerId == player_id && match.Type == PlayerCreationType.TRACK).ToList();
            foreach (PlayerCreationData Track in creations)
            {
                trackList.Add(new track
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

            var resp = new Response<List<planet>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new List<planet> { new planet {
                    id = Planet.PlayerCreationId,
                    name = Planet.Name,
                    player_id = Planet.PlayerId,
                    username = Planet.Username,
                    tracks = new tracks {
                        total = Planet.Author.TotalTracks,
                        TrackList = trackList
                    }
                } }
            };
            return resp.Serialize();
        }
    }
}
