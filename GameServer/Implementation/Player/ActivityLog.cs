using GameServer.Models.Response;
using GameServer.Models;
using GameServer.Utils;
using System.Collections.Generic;
using GameServer.Models.PlayerData;
using System.Linq;
using GameServer.Models.Request;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Implementation.Player
{
    public class ActivityLog
    {
        public static string GetActivityLog(Database database, User user, int page, int per_page, ActivityList list = ActivityList.news_feed,
            int? player_id = null, int? player_creation_id = null)
        {
            var query = database.ActivityLog
                .AsSplitQuery()
                .AsNoTracking()
                .Include(e => e.Author)
                .ThenInclude(u => u.HeartedByProfiles)
                .Include(e => e.Player)
                .ThenInclude(u => u.HeartedByProfiles)
                .Include(e => e.PlayerCreation)
                .ThenInclude(c => c.Hearts)
                .Include(e => e.PlayerCreation)
                .ThenInclude(c => c.Ratings)
                .Include(e => e.PlayerCreation)
                .ThenInclude(c => c.RacesStarted)
                .Where(match => (user == null || ((match.AuthorId == null || !match.Author.IsBlockedByMe(user.UserId)) 
                            && (match.PlayerId == null || !match.Player.IsBlockedByMe(user.UserId))
                            && (match.PlayerCreationId == null || !match.PlayerCreation.Author.IsBlockedByMe(user.UserId))))
                        && (match.List == list || match.List == ActivityList.both));

            if (player_id != null)
                query = query.Where(match => match.AuthorId == player_id || match.PlayerId == player_id 
                   || (match.PlayerCreationId != null && match.PlayerCreation.PlayerId == player_id));

            if (player_creation_id != null)
                query = query.Where(match => match.PlayerCreationId == player_creation_id);
            
            if (list == ActivityList.news_feed)
                query = query.Where(match => (user != null && 
                        ((match.AuthorId != null && (match.Author.IsHeartedByMe(user.UserId, false) || match.Author.IsBuddyWithMe(user.UserId)))
                        || (match.PlayerId != null && (match.Player.IsHeartedByMe(user.UserId, false) || match.Player.IsBuddyWithMe(user.UserId)))
                        || (match.PlayerCreationId != null && (match.PlayerCreation.IsHeartedByMe(user.UserId) 
                            || match.PlayerCreation.Author.IsHeartedByMe(user.UserId, false) || match.PlayerCreation.Author.IsBuddyWithMe(user.UserId)))))
                    || match.Type == ActivityType.system_event);

            query = query.OrderByDescending(e => e.CreatedAt);

            var total = query.Count();
            
            //calculating pages
            int pageEnd = PageCalculator.GetPageEnd(page, per_page);
            int pageStart = PageCalculator.GetPageStart(page, per_page);
            int totalPages = PageCalculator.GetTotalPages(per_page, total);

            if (pageEnd > total)
                pageEnd = total;

            var activityList = new List<Activity>();

            foreach (var activity in query.Skip(pageStart).Take(per_page).ToList())
            {
                switch (activity.Type)
                {
                    case ActivityType.system_event:
                        activityList.Add(new Activity
                        {
                            type = "system_activity",
                            events = [
                                new Event
                                {
                                    topic = activity.Type.ToString(),
                                    type = activity.Topic,
                                    creator_id = activity.AuthorId ?? 0,
                                    creator_username = activity.Author?.Username ?? "",
                                    details = activity.Description,
                                    timestamp = activity.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz"),
                                    seconds_ago = TimeUtils.SecondsAgo(activity.CreatedAt),
                                    tags = activity.Tags,
                                    subject = activity.Subject,
                                    image_url = activity.ImageURL,
                                    image_md5 = activity.ImageMD5
                                }
                            ]
                        });
                        break;

                    case ActivityType.player_event when !activity.PlayerCreationId.HasValue:
                    case ActivityType.trophy_event:
                        activityList.Add(new Activity
                        {
                            player_hearts = activity.Player?.Hearts ?? 0,
                            player_id = activity.PlayerId ?? 0,
                            player_username = activity.Player?.Username ?? "",
                            type = "player_activity",
                            events = [
                                new Event
                                {
                                    topic = activity.Type.ToString(),
                                    type = activity.Topic,
                                    details = activity.Description,
                                    creator_username = activity.Author?.Username ?? "",
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
                        break;

                    case ActivityType.player_creation_event:
                    case ActivityType.race_event:
                    case ActivityType.player_event when activity.PlayerCreationId.HasValue:
                        activityList.Add(new Activity
                        {
                            player_creation_id = activity.PlayerCreationId ?? 0,
                            player_creation_hearts = activity.PlayerCreation?.HeartsCount ?? 0,
                            player_creation_rating_up = activity.PlayerCreation?.RatingUp ?? 0,
                            player_creation_rating_down = activity.PlayerCreation?.RatingDown ?? 0,
                            player_creation_races_started = activity.PlayerCreation?.RacesStartedCount ?? 0,
                            player_creation_username = activity.PlayerCreation?.Author?.Username ?? "",
                            player_creation_description = activity.PlayerCreation?.Description ?? "",
                            player_creation_name = activity.PlayerCreation?.Name ?? "",
                            player_creation_player_id = activity.PlayerCreation?.PlayerId ?? 0,
                            player_creation_associated_item_ids = activity.PlayerCreation?.AssociatedItemIds ?? "",
                            player_creation_level_mode = activity.PlayerCreation?.LevelMode ?? 0,
                            player_creation_is_team_pick = activity.PlayerCreation?.IsTeamPick ?? false,
                            type = "player_creation_activity",
                            events = [
                                new Event
                                {
                                    topic = activity.Type.ToString(),
                                    type = activity.Topic,
                                    details = activity.Description,
                                    creator_username = activity.Author?.Username ?? "",
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
                        break;
                }
            }

            var resp = new Response<List<Activities>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [
                    new Activities
                    {
                        total = total,
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

        public static string NewsFeedTally(Database database, User user)
        {
            var total = database.ActivityLog
                .AsSplitQuery()
                .AsNoTracking()
                .Where(match => (user == null || ((match.AuthorId == null || !match.Author.IsBlockedByMe(user.UserId)) 
                                    && (match.PlayerId == null || !match.Player.IsBlockedByMe(user.UserId))
                                    && (match.PlayerCreationId == null || !match.PlayerCreation.Author.IsBlockedByMe(user.UserId))))
                                && (match.List == ActivityList.news_feed || match.List == ActivityList.both))
                .Count(match => (user != null && 
                      ((match.AuthorId != null && (match.Author.IsHeartedByMe(user.UserId, false) || match.Author.IsBuddyWithMe(user.UserId)))
                       || (match.PlayerId != null && (match.Player.IsHeartedByMe(user.UserId, false) || match.Player.IsBuddyWithMe(user.UserId)))
                       || (match.PlayerCreationId != null && (match.PlayerCreation.IsHeartedByMe(user.UserId) 
                           || match.PlayerCreation.Author.IsHeartedByMe(user.UserId, false) || match.PlayerCreation.Author.IsBuddyWithMe(user.UserId)))))
                      | match.Type == ActivityType.system_event);

            var resp = new Response<List<Activities>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response = [new Activities { total = total }]
            };

            return resp.Serialize();
        }

        public static string CreateEvent(Database database, User user, ActivityType topic, int creator_id, PlayerEvent @event, ActivityList list_name)
        {
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
                    PlayerId = int.Parse(@event.player_id.TrimEnd('\0')),
                    PlayerCreationId = null,
                    CreatedAt = TimeUtils.Now,
                    AllusionId = int.Parse(@event.player_id.TrimEnd('\0')),
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
