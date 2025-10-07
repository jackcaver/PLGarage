using GameServer.Models.Response;
using GameServer.Models;
using GameServer.Utils;
using System.Collections.Generic;
using System;
using GameServer.Models.PlayerData;
using System.Linq;
using GameServer.Models.Request;
using GameServer.Implementation.Common;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Implementation.Player
{
    public class ActivityLog
    {
        public static string GetActivityLog(Database database, Guid SessionID, int page, int per_page, ActivityList list = ActivityList.news_feed,
            int? player_id = null, int? player_creation_id = null)
        {
            // TODO: Optimise
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);
            var Activities = new List<ActivityEvent> { };

            if (list == ActivityList.news_feed)
                Activities.AddRange(database.ActivityLog.Where(match => match.Type == ActivityType.system_event).ToList());

            if (player_id != null)
            {
                Activities.AddRange(database.ActivityLog.Where(match => (match.AuthorId == player_id || match.PlayerId == player_id) &&
                    (match.List == list || match.List == ActivityList.both)).ToList());
                foreach (var Creation in database.PlayerCreations.Where(match => match.PlayerId == player_id).ToList())
                {
                    Activities.AddRange(database.ActivityLog.Where(match => match.PlayerCreationId == Creation.PlayerCreationId &&
                        match.AuthorId != player_id && match.PlayerId != player_id && (match.List == list || match.List == ActivityList.both)).ToList());
                }
            }

            if (player_creation_id != null)
                Activities.AddRange(database.ActivityLog.Where(match => match.PlayerCreationId == player_creation_id &&
                    (match.List == list || match.List == ActivityList.both)).ToList());

            if (list == ActivityList.news_feed && user != null)
            {
                foreach (var Heart in database.HeartedProfiles.Where(match => match.UserId == user.UserId).ToList())
                {
                    Activities.AddRange(database.ActivityLog.Where(match => (match.AuthorId == Heart.HeartedUserId || match.PlayerId == Heart.HeartedUserId) &&
                        (match.List == list || match.List == ActivityList.both)).ToList());
                    foreach (var Creation in database.PlayerCreations.Where(match => match.PlayerId == Heart.HeartedUserId).ToList())
                    {
                        Activities.AddRange(database.ActivityLog.Where(match => match.PlayerCreationId == Creation.PlayerCreationId &&
                            match.AuthorId != Heart.HeartedUserId && match.PlayerId != Heart.HeartedUserId &&
                            (match.List == list || match.List == ActivityList.both)).ToList());
                    }
                }
            }

            Activities.Sort((curr, prev) => prev.CreatedAt.CompareTo(curr.CreatedAt));

            //calculating pages
            int pageEnd = PageCalculator.GetPageEnd(page, per_page);
            int pageStart = PageCalculator.GetPageStart(page, per_page);
            int totalPages = PageCalculator.GetTotalPages(per_page, Activities.Count);

            if (pageEnd > Activities.Count)
                pageEnd = Activities.Count;

            var activityList = new List<Activity> { };

            for (int i = pageStart; i < pageEnd; i++)
            {
                var Activity = Activities[i];
                var Author = database.Users
                    .Include(u => u.HeartedByProfiles)
                    .FirstOrDefault(match => match.UserId == Activity.AuthorId);
                var Player = database.Users
                    .Include(u => u.HeartedByProfiles)
                    .FirstOrDefault(match => match.UserId == Activity.PlayerId);
                UserGeneratedContentUtils.CheckStoryLevelName(database, Activity.PlayerCreationId);
                var PlayerCreation = database.PlayerCreations
                    .AsSplitQuery()
                    .Include(x => x.Hearts)
                    .Include(x => x.Ratings)
                    .Include(x => x.RacesStarted)
                    .Include(x => x.Author)
                    .FirstOrDefault(match => match.PlayerCreationId == Activity.PlayerCreationId);

                if (Activity.Type == ActivityType.system_event)
                {
                    activityList.Add(new Activity
                    {
                        type = "system_activity",
                        events = [
                            new Event
                            {
                                topic = Activity.Type.ToString(),
                                type = Activity.Topic,
                                creator_id = Activity.AuthorId,
                                creator_username = Author != null ? Author.Username : "",
                                details = Activity.Description,
                                timestamp = Activity.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                                seconds_ago = TimeUtils.SecondsAgo(Activity.CreatedAt),
                                tags = Activity.Tags,
                                subject = Activity.Subject,
                                image_url = Activity.ImageURL,
                                image_md5 = Activity.ImageMD5
                            }
                        ]
                    });
                }
                else if (Activity.Type == ActivityType.player_event && Activity.PlayerCreationId == 0 || Activity.Type == ActivityType.trophy_event)
                {
                    activityList.Add(new Activity
                    {
                        player_hearts = Player != null ? Player.Hearts : 0,
                        player_id = Activity.PlayerId,
                        player_username = Player != null ? Player.Username : "",
                        type = "player_activity",
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
                else if (Activity.Type == ActivityType.player_creation_event
                    || Activity.Type == ActivityType.race_event
                    || Activity.Type == ActivityType.player_event && Activity.PlayerCreationId != 0)
                {
                    activityList.Add(new Activity
                    {
                        player_creation_id = Activity.PlayerCreationId,
                        player_creation_hearts = PlayerCreation != null ? PlayerCreation.HeartsCount : 0,
                        player_creation_rating_up = PlayerCreation != null ? PlayerCreation.RatingUp : 0,
                        player_creation_rating_down = PlayerCreation != null ? PlayerCreation.RatingDown : 0,
                        player_creation_races_started = PlayerCreation != null ? PlayerCreation.RacesStartedCount : 0,
                        player_creation_username = PlayerCreation != null ? PlayerCreation.Author.Username : "",
                        player_creation_description = PlayerCreation != null ? PlayerCreation.Description : "",
                        player_creation_name = PlayerCreation != null ? PlayerCreation.Name : "",
                        player_creation_player_id = PlayerCreation != null ? PlayerCreation.PlayerId : 0,
                        player_creation_associated_item_ids = PlayerCreation != null ? PlayerCreation.AssociatedItemIds : "",
                        player_creation_level_mode = PlayerCreation != null ? PlayerCreation.LevelMode : 0,
                        player_creation_is_team_pick = PlayerCreation != null ? PlayerCreation.IsTeamPick : false,
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
            }

            var resp = new Response<List<Activities>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [
                    new Activities
                    {
                        total = Activities.Count,
                        page = page,
                        row_end = pageEnd,
                        row_start = pageStart,
                        total_pages = totalPages,
                        ActivityList = activityList
                    }
                ]
            };
            return resp.Serialize();
        }

        public static string NewsFeedTally(Database database, Guid SessionID)
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);
            var total = 0;

            total += database.ActivityLog.Count(match => match.Type == ActivityType.system_event);

            if (user != null)
            {
                foreach (var Heart in database.HeartedProfiles.Where(match => match.UserId == user.UserId).ToList())
                {
                    total += database.ActivityLog.Count(match => (match.AuthorId == Heart.HeartedUserId 
                        || match.PlayerId == Heart.HeartedUserId) 
                        && (match.List == ActivityList.news_feed || match.List == ActivityList.both));
                    foreach (var Creation in database.PlayerCreations.Where(match => match.PlayerId == Heart.HeartedUserId).ToList())
                    {
                        total += database.ActivityLog.Count(match => match.PlayerCreationId == Creation.PlayerCreationId &&
                            match.AuthorId != Heart.HeartedUserId && match.PlayerId != Heart.HeartedUserId &&
                            (match.List == ActivityList.news_feed || match.List == ActivityList.both));
                    }
                }
            }

            var resp = new Response<List<Activities>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [new Activities { total = total }]
            };

            return resp.Serialize();
        }

        public static string CreateEvent(Database database, Guid SessionID, ActivityType topic, int creator_id, PlayerEvent @event, ActivityList list_name)
        {
            var session = Session.GetSession(SessionID);
            var user = database.Users.FirstOrDefault(match => match.Username == session.Username);

            if (topic == ActivityType.system_event || user == null || creator_id != user.UserId)
            {
                var errorResp = new Response<EmptyResponse>
                {
                    status = new ResponseStatus { id = -130, message = "The player doesn't exist" },
                    response = new EmptyResponse { }
                };
                return errorResp.Serialize();
            }

            if (topic == ActivityType.trophy_event)
            {
                database.ActivityLog.Add(new ActivityEvent
                {
                    AuthorId = creator_id,
                    Type = topic,
                    List = list_name,
                    Topic = @event.type,
                    Description = @event.description,
                    PlayerId = int.Parse(@event.player_id.Split("\0")[0]),
                    PlayerCreationId = 0,
                    CreatedAt = TimeUtils.Now,
                    AllusionId = int.Parse(@event.player_id.Split("\0")[0]),
                    AllusionType = "Player"
                });
                database.SaveChanges();
            }

            var resp = new Response<EmptyResponse>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = new EmptyResponse { }
            };

            return resp.Serialize();
        }
    }
}
