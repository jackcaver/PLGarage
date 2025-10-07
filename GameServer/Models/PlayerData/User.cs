using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Utils;
using System;
using System.Linq;
using GameServer.Implementation.Common;
using GameServer.Models.Request;
using System.Collections.Generic;
using System.Globalization;
using GameServer.Models.Config;
using GameServer.Models.GameBrowser;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Models.PlayerData
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }

        [InverseProperty(nameof(HeartedProfile.HeartedUser))]
        public List<HeartedProfile> HeartedByProfiles { get; set; }

        public int Hearts => HeartedByProfiles.Count;
        public Presence Presence => Session.GetPresence(this.Username);
        public int Quota { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Quote { get; set; }
        public ulong PSNID { get; set; }
        public ulong RPCNID { get; set; }

        public List<PlayerCreationData> PlayerCreations { get; set; }

        public int TotalTracks => PlayerCreations.Count(match => match.Type == PlayerCreationType.TRACK && !match.IsMNR);
        public int Rank => GetRank(GameType.OVERALL, LeaderboardType.LIFETIME, Platform.PS3, SortColumn.points);

        public List<PlayerPoint> PlayerPoints { get; set; }

        public int Points => PlayerPoints.Sum(p => p.Amount);

        public List<RaceStarted> RacesStarted { get; set; }
        public List<RaceFinished> RacesFinished { get; set; }

        public int OnlineRaces => RacesStarted.Count;
        public int OnlineWins => RacesFinished.Count(match => match.IsWinner);
        public int OnlineFinished => RacesFinished.Count;
        public int OnlineForfeit { get; set; }
        public int OnlineDisconnected { get; set; }
        public int WinStreak { get; set; }
        public int LongestWinStreak { get; set; }
        public int OnlineRacesThisWeek => RacesStarted.Count(match => match.StartedAt >= TimeUtils.ThisWeekStart);
        public int OnlineWinsThisWeek => RacesFinished.Count(match => match.IsWinner && match.FinishedAt >= TimeUtils.ThisWeekStart);
        public int OnlineFinishedThisWeek => RacesFinished.Count(match => match.FinishedAt >= TimeUtils.ThisWeekStart);
        public int OnlineRacesLastWeek => RacesStarted.Count(match => match.StartedAt >= TimeUtils.LastWeekStart && match.StartedAt < TimeUtils.ThisWeekStart);
        public int OnlineWinsLastWeek => RacesFinished.Count(match => match.IsWinner && match.FinishedAt >= TimeUtils.LastWeekStart && match.FinishedAt < TimeUtils.ThisWeekStart);
        public int OnlineFinishedLastWeek => RacesFinished.Count(match => match.FinishedAt >= TimeUtils.LastWeekStart && match.FinishedAt < TimeUtils.ThisWeekStart);
        public bool PolicyAccepted { get; set; }
        public bool IsBanned { get; set; }
        public bool ShowCreationsWithoutPreviews { get; set; }
        public bool AllowOppositePlatform { get; set; }

        //MNR
        public float LongestHangTime { get; set; }
        public float LongestDrift { get; set; }
        public int OnlineQuits => OnlineDisconnected+OnlineForfeit;
        public int CharacterIdx { get; set; }
        public int KartIdx { get; set; }
        public bool PlayedMNR { get; set; }
        public int TotalCharacters(Platform platform) => PlayerCreations.Count(match => match.Type == PlayerCreationType.CHARACTER && match.Platform == platform);
        public int TotalKarts(Platform platform) => PlayerCreations.Count(match => match.Type == PlayerCreationType.KART && match.Platform == platform);
        public int TotalPlayerCreations(Platform platform) => PlayerCreations.Count(match => match.Type != PlayerCreationType.PHOTO && match.Type != PlayerCreationType.DELETED && match.IsMNR && match.Platform == platform);
        public int TotalMNRTracks(Platform platform) => PlayerCreations.Count(match => match.Type == PlayerCreationType.TRACK && match.IsMNR && match.Platform == platform);
        
        public List<PlayerCreationPoint> PlayerCreationPoints { get; set; }

        public float CreatorPoints(Platform platform) => PlayerCreationPoints.Where(match => match.Platform == platform).Sum(p => p.Amount);
        public float CreatorPointsLastWeek(Platform platform) => PlayerCreationPoints.Where(match => match.Platform == platform && match.CreatedAt >= TimeUtils.LastWeekStart && match.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount);
        public float CreatorPointsThisWeek(Platform platform) => PlayerCreationPoints.Where(match => match.Platform == platform && match.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount);
        public float CreatorPoints(Platform platform, PlayerCreationType type) => PlayerCreationPoints.Where(match => match.Platform == platform && match.Type == type).Sum(p => p.Amount);
        public float CreatorPointsLastWeek(Platform platform, PlayerCreationType type) => PlayerCreationPoints.Where(match => match.Platform == platform && match.Type == type && match.CreatedAt >= TimeUtils.LastWeekStart && match.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount);
        public float CreatorPointsThisWeek(Platform platform, PlayerCreationType type) => PlayerCreationPoints.Where(match => match.Platform == platform && match.Type == type && match.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount);
        
        public List<PlayerExperiencePoint> PlayerExperiencePoints { get; set; }

        public float ExperiencePoints => PlayerExperiencePoints.Sum(p => p.Amount);
        public float ExperiencePointsLastWeek => PlayerExperiencePoints.Where(match => match.CreatedAt >= TimeUtils.LastWeekStart && match.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount);
        public float ExperiencePointsThisWeek => PlayerExperiencePoints.Where(match => match.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount);
        public int PointsLastWeek => PlayerPoints.Where(match => match.CreatedAt >= TimeUtils.LastWeekStart && match.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount);
        public int PointsThisWeek => PlayerPoints.Where(match => match.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount);

        [InverseProperty(nameof(PlayerRatingData.Player))]
        public List<PlayerRatingData> PlayerRatings { get; set; }

        public float Rating => PlayerRatings.Count > 0 ? (float)PlayerRatings.Average(r => r.Rating) : 0;
        public string StarRating => Rating.ToString("0.0", CultureInfo.InvariantCulture);
        public int SkillLevelId(Platform platform) => SkillConfig.Instance.GetSkillLevel((int)Math.Floor(TotalXP(platform))).Id;
        public string SkillLevelName(Platform platform) => SkillConfig.Instance.GetSkillLevel((int)Math.Floor(TotalXP(platform))).Name;
        public float TotalXP(Platform platform) => (platform == Platform.PSV ? 0 : ExperiencePoints) + CreatorPoints(platform);
        public float TotalXPLastWeek(Platform platform) => (platform == Platform.PSV ? 0 : ExperiencePoints) + CreatorPointsLastWeek(platform);
        public float TotalXPThisWeek(Platform platform) => (platform == Platform.PSV ? 0 : ExperiencePoints) + CreatorPointsThisWeek(platform);
        
        //MNR: Road Trip
        public List<TravelPoint> TravelPointsData { get; set; }

        public int TravelPoints => TravelPointsData.Sum(p => p.Amount);
        public int TravelPointsThisWeek => TravelPointsData.Where(match => match.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount);
        public int TravelPointsLastWeek => TravelPointsData.Where(match => match.CreatedAt >= TimeUtils.LastWeekStart && match.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount);
        
        public List<POIVisit> POIVisits { get; set; }

        public int Visits => POIVisits.Count;
        public int VisitsThisWeek => POIVisits.Count(match => match.CreatedAt >= TimeUtils.ThisWeekStart);
        public int VisitsLastWeek => POIVisits.Count(match => match.CreatedAt >= TimeUtils.LastWeekStart && match.CreatedAt < TimeUtils.ThisWeekStart);
        public bool HasCheckedInBefore { get; set; }
        public float ModMiles { get; set; }
        public float LastLatitude { get; set; }
        public float LastLongitude { get; set; }

        public bool IsHeartedByMe(int id, bool IsMNR) 
        {
            return HeartedByProfiles.Any(match => match.UserId == id && match.IsMNR == IsMNR);
        }

        public int GetRank(GameType game_type, LeaderboardType leaderboardType, Platform platform, SortColumn sort_column)
        {
            using var database = new Database();
            var users = database.Users
                .AsSplitQuery()
                .Include(x => x.RacesStarted)
                .Include(x => x.RacesFinished)
                .Include(x => x.PlayerCreationPoints)
                .Include(x => x.PlayerExperiencePoints)
                .Where(match => match.Username != "ufg" && match.PlayedMNR);

            //creator points
            if (game_type == GameType.OVERALL_CREATORS && leaderboardType == LeaderboardType.LIFETIME)
                users = users.OrderByDescending(u => u.PlayerCreationPoints.Where(match => match.Platform == platform).Sum(p => p.Amount));
            if (game_type == GameType.OVERALL_CREATORS && leaderboardType == LeaderboardType.WEEKLY)
                users = users.OrderByDescending(u => u.PlayerCreationPoints.Where(match => match.Platform == platform && match.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount));
            if (game_type == GameType.OVERALL_CREATORS && leaderboardType == LeaderboardType.LAST_WEEK)
                users = users.OrderByDescending(u => u.PlayerCreationPoints.Where(match => match.Platform == platform && match.CreatedAt >= TimeUtils.LastWeekStart && match.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount));

            //creator points for characters
            if (game_type == GameType.CHARACTER_CREATORS && leaderboardType == LeaderboardType.LIFETIME)
                users = users.OrderByDescending(u => u.PlayerCreationPoints.Where(match => match.Platform == platform && match.Type == PlayerCreationType.CHARACTER).Sum(p => p.Amount));
            if (game_type == GameType.CHARACTER_CREATORS && leaderboardType == LeaderboardType.WEEKLY)
                users = users.OrderByDescending(u => u.PlayerCreationPoints.Where(match => match.Platform == platform && match.Type == PlayerCreationType.CHARACTER && match.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount));
            if (game_type == GameType.CHARACTER_CREATORS && leaderboardType == LeaderboardType.LAST_WEEK)
                users = users.OrderByDescending(u => u.PlayerCreationPoints.Where(match => match.Platform == platform && match.Type == PlayerCreationType.CHARACTER && match.CreatedAt >= TimeUtils.LastWeekStart && match.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount));

            //creator points for karts
            if (game_type == GameType.CHARACTER_CREATORS && leaderboardType == LeaderboardType.LIFETIME)
                users = users.OrderByDescending(u => u.PlayerCreationPoints.Where(match => match.Platform == platform && match.Type == PlayerCreationType.KART).Sum(p => p.Amount));
            if (game_type == GameType.CHARACTER_CREATORS && leaderboardType == LeaderboardType.WEEKLY)
                users = users.OrderByDescending(u => u.PlayerCreationPoints.Where(match => match.Platform == platform && match.Type == PlayerCreationType.KART && match.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount));
            if (game_type == GameType.CHARACTER_CREATORS && leaderboardType == LeaderboardType.LAST_WEEK)
                users = users.OrderByDescending(u => u.PlayerCreationPoints.Where(match => match.Platform == platform && match.Type == PlayerCreationType.KART && match.CreatedAt >= TimeUtils.LastWeekStart && match.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount));

            //creator points for tracks
            if (game_type == GameType.CHARACTER_CREATORS && leaderboardType == LeaderboardType.LIFETIME)
                users = users.OrderByDescending(u => u.PlayerCreationPoints.Where(match => match.Platform == platform && match.Type == PlayerCreationType.TRACK).Sum(p => p.Amount));
            if (game_type == GameType.CHARACTER_CREATORS && leaderboardType == LeaderboardType.WEEKLY)
                users = users.OrderByDescending(u => u.PlayerCreationPoints.Where(match => match.Platform == platform && match.Type == PlayerCreationType.TRACK && match.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount));
            if (game_type == GameType.CHARACTER_CREATORS && leaderboardType == LeaderboardType.LAST_WEEK)
                users = users.OrderByDescending(u => u.PlayerCreationPoints.Where(match => match.Platform == platform && match.Type == PlayerCreationType.TRACK && match.CreatedAt >= TimeUtils.LastWeekStart && match.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount));

            //Experience points
            if (game_type == GameType.OVERALL && leaderboardType == LeaderboardType.LIFETIME)
                users = users.OrderByDescending(u => (platform == Platform.PSV ? 0 : u.PlayerExperiencePoints.Sum(p => p.Amount)) + u.PlayerCreationPoints.Where(match => match.Platform == platform).Sum(p => p.Amount));
            if (game_type == GameType.OVERALL && leaderboardType == LeaderboardType.WEEKLY)
                users = users.OrderByDescending(u => (platform == Platform.PSV ? 0 : u.PlayerExperiencePoints.Where(match => match.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount)) + u.PlayerCreationPoints.Where(match => match.Platform == platform && match.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount));
            if (game_type == GameType.OVERALL && leaderboardType == LeaderboardType.LAST_WEEK)
                users = users.OrderByDescending(u => (platform == Platform.PSV ? 0 : u.PlayerExperiencePoints.Where(match => match.CreatedAt >= TimeUtils.LastWeekStart && match.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount)) + u.PlayerCreationPoints.Where(match => match.Platform == platform && match.CreatedAt >= TimeUtils.LastWeekStart && match.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount));

            if (game_type == GameType.OVERALL_RACE)
            {
                switch (sort_column)
                {
                    case SortColumn.experience_points:
                        if (leaderboardType == LeaderboardType.LIFETIME)
                            users = users.OrderByDescending(u => u.PlayerExperiencePoints.Sum(p => p.Amount));
                        if (leaderboardType == LeaderboardType.WEEKLY)
                            users = users.OrderByDescending(u => u.PlayerExperiencePoints.Where(match => match.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount));
                        if (leaderboardType == LeaderboardType.LAST_WEEK)
                            users = users.OrderByDescending(u => u.PlayerExperiencePoints.Where(match => match.CreatedAt >= TimeUtils.LastWeekStart && match.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount));
                        break;

                    case SortColumn.online_races:
                        users = users.OrderByDescending(u => u.RacesStarted.Count);
                        break;

                    case SortColumn.online_wins:
                        users = users.OrderByDescending(u => u.RacesFinished.Count(match => match.IsWinner));
                        break;

                    case SortColumn.longest_win_streak:
                        users = users.OrderByDescending(u => u.LongestWinStreak);
                        break;

                    case SortColumn.win_streak:
                        users = users.OrderByDescending(u => u.WinStreak);
                        break;

                    case SortColumn.longest_hang_time:
                        users = users.OrderByDescending(u => u.LongestHangTime);
                        break;

                    case SortColumn.longest_drift:
                        users = users.OrderByDescending(u => u.LongestDrift);
                        break;

                    default:
                        break;
                }
            }

            return users.Select(u => u.UserId).ToList().FindIndex(match => match == UserId) + 1;
        }
    }
}
