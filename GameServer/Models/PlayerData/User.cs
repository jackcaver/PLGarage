using EntityFrameworkCore.Projectables;
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

        [Projectable]
        public int Hearts => HeartedByProfiles.Count;
        public Presence Presence => Session.GetPresence(Username);
        public int Quota { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Quote { get; set; }
        public ulong PSNID { get; set; }
        public ulong RPCNID { get; set; }

        public List<PlayerCreationData> PlayerCreations { get; set; }

        [Projectable]
        public int TotalTracks => PlayerCreations.Count(match => match.Type == PlayerCreationType.TRACK && !match.IsMNR);
        //[Projectable]
        public int Rank => GetRank(GameType.OVERALL, LeaderboardType.LIFETIME, Platform.PS3, SortColumn.points);

        public List<PlayerPoint> PlayerPoints { get; set; }

        [Projectable]
        public int Points(Platform platform) => PlayerPoints.Count >= 10 ? int.Clamp((int)PlayerPoints.Where(match => match.Platform == platform).Average(p => p.Amount), 0, 3000) : 1500;

        public List<RaceStarted> RacesStarted { get; set; }
        public List<RaceFinished> RacesFinished { get; set; }

        [Projectable]
        public int OnlineRaces => RacesStarted.Count;
        [Projectable]
        public int OnlineWins => RacesFinished.Count(match => match.IsWinner);
        [Projectable]
        public int OnlineFinished => RacesFinished.Count;
        public int OnlineForfeit { get; set; }
        public int OnlineDisconnected { get; set; }
        public int WinStreak { get; set; }
        public int LongestWinStreak { get; set; }
        [Projectable]
        public int OnlineRacesThisWeek => RacesStarted.Count(match => match.StartedAt >= TimeUtils.ThisWeekStart);
        [Projectable]
        public int OnlineWinsThisWeek => RacesFinished.Count(match => match.IsWinner && match.FinishedAt >= TimeUtils.ThisWeekStart);
        [Projectable]
        public int OnlineFinishedThisWeek => RacesFinished.Count(match => match.FinishedAt >= TimeUtils.ThisWeekStart);
        [Projectable]
        public int OnlineRacesLastWeek => RacesStarted.Count(match => match.StartedAt >= TimeUtils.LastWeekStart && match.StartedAt < TimeUtils.ThisWeekStart);
        [Projectable]
        public int OnlineWinsLastWeek => RacesFinished.Count(match => match.IsWinner && match.FinishedAt >= TimeUtils.LastWeekStart && match.FinishedAt < TimeUtils.ThisWeekStart);
        [Projectable]
        public int OnlineFinishedLastWeek => RacesFinished.Count(match => match.FinishedAt >= TimeUtils.LastWeekStart && match.FinishedAt < TimeUtils.ThisWeekStart);
        public bool PolicyAccepted { get; set; }
        public bool IsBanned { get; set; }
        public bool ShowCreationsWithoutPreviews { get; set; }
        public bool AllowOppositePlatform { get; set; }

        //MNR
        public float LongestHangTime { get; set; }
        public float LongestDrift { get; set; }
        [Projectable]
        public int OnlineQuits => OnlineDisconnected+OnlineForfeit;
        public int CharacterIdx { get; set; }
        public int KartIdx { get; set; }
        public bool PlayedMNR { get; set; }
        [Projectable]
        public int TotalCharacters(Platform platform) => PlayerCreations.Count(match => match.Type == PlayerCreationType.CHARACTER && match.Platform == platform);
        [Projectable]
        public int TotalKarts(Platform platform) => PlayerCreations.Count(match => match.Type == PlayerCreationType.KART && match.Platform == platform);
        [Projectable]
        public int TotalPlayerCreations(Platform platform) => PlayerCreations.Count(match => match.Type != PlayerCreationType.PHOTO && match.Type != PlayerCreationType.DELETED && match.IsMNR && match.Platform == platform);
        [Projectable]
        public int TotalMNRTracks(Platform platform) => PlayerCreations.Count(match => match.Type == PlayerCreationType.TRACK && match.IsMNR && match.Platform == platform);
        
        public List<PlayerCreationPoint> PlayerCreationPoints { get; set; }

        [Projectable]
        public float CreatorPoints(Platform platform) => PlayerCreationPoints.Where(match => match.Platform == platform).Sum(p => p.Amount);
        [Projectable]
        public float CreatorPointsLastWeek(Platform platform) => PlayerCreationPoints.Where(match => match.Platform == platform && match.CreatedAt >= TimeUtils.LastWeekStart && match.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount);
        [Projectable]
        public float CreatorPointsThisWeek(Platform platform) => PlayerCreationPoints.Where(match => match.Platform == platform && match.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount);
        [Projectable]
        public float CreatorPoints(Platform platform, PlayerCreationType type) => PlayerCreationPoints.Where(match => match.Platform == platform && match.Type == type).Sum(p => p.Amount);
        [Projectable]
        public float CreatorPointsLastWeek(Platform platform, PlayerCreationType type) => PlayerCreationPoints.Where(match => match.Platform == platform && match.Type == type && match.CreatedAt >= TimeUtils.LastWeekStart && match.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount);
        [Projectable]
        public float CreatorPointsThisWeek(Platform platform, PlayerCreationType type) => PlayerCreationPoints.Where(match => match.Platform == platform && match.Type == type && match.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount);
        
        public List<PlayerExperiencePoint> PlayerExperiencePoints { get; set; }

        [Projectable]
        public float ExperiencePoints(Platform platform) => PlayerExperiencePoints.Where(match => match.Platform == platform).Sum(p => p.Amount);
        [Projectable]
        public float ExperiencePointsLastWeek(Platform platform) => PlayerExperiencePoints.Where(match => match.Platform == platform && match.CreatedAt >= TimeUtils.LastWeekStart && match.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount);
        [Projectable]
        public float ExperiencePointsThisWeek(Platform platform) => PlayerExperiencePoints.Where(match => match.Platform == platform && match.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount);

        [InverseProperty(nameof(PlayerRatingData.Player))]
        public List<PlayerRatingData> PlayerRatings { get; set; }

        [Projectable]
        public float Rating => PlayerRatings.Count > 0 ? (float)PlayerRatings.Average(r => r.Rating) : 0;
        [Projectable]
        public string StarRating => Rating.ToString("0.0", CultureInfo.InvariantCulture);
        [Projectable]
        public int SkillLevelId(Platform platform) => SkillConfig.Instance.GetSkillLevel((int)Math.Floor(TotalXP(platform))).Id;
        [Projectable]
        public string SkillLevelName(Platform platform) => SkillConfig.Instance.GetSkillLevel((int)Math.Floor(TotalXP(platform))).Name;
        [Projectable]
        public float TotalXP(Platform platform) => ExperiencePoints(platform) + CreatorPoints(platform);
        [Projectable]
        public float TotalXPLastWeek(Platform platform) => ExperiencePointsLastWeek(platform) + CreatorPointsLastWeek(platform);
        [Projectable]
        public float TotalXPThisWeek(Platform platform) => ExperiencePointsThisWeek(platform) + CreatorPointsThisWeek(platform);
        
        //MNR: Road Trip
        public List<TravelPoint> TravelPointsData { get; set; }

        [Projectable]
        public int TravelPoints => TravelPointsData.Sum(p => p.Amount);
        [Projectable]
        public int TravelPointsThisWeek => TravelPointsData.Where(match => match.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount);
        [Projectable]
        public int TravelPointsLastWeek => TravelPointsData.Where(match => match.CreatedAt >= TimeUtils.LastWeekStart && match.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount);
        
        public List<POIVisit> POIVisits { get; set; }

        [Projectable]
        public int Visits => POIVisits.Count;
        [Projectable]
        public int VisitsThisWeek => POIVisits.Count(match => match.CreatedAt >= TimeUtils.ThisWeekStart);
        [Projectable]
        public int VisitsLastWeek => POIVisits.Count(match => match.CreatedAt >= TimeUtils.LastWeekStart && match.CreatedAt < TimeUtils.ThisWeekStart);
        public bool HasCheckedInBefore { get; set; }
        public float ModMiles { get; set; }
        public float LastLatitude { get; set; }
        public float LastLongitude { get; set; }
        [Projectable]
        public bool IsHeartedByMe(int id, bool IsMNR) => HeartedByProfiles.Any(match => match.UserId == id && match.IsMNR == IsMNR);

        public int GetRank(GameType game_type, LeaderboardType leaderboardType, Platform platform, SortColumn sort_column)
        {
            using var database = new Database();
            var users = database.Users
                .AsSplitQuery()
                .Include(x => x.PlayerPoints)
                .Include(x => x.RacesStarted)
                .Include(x => x.RacesFinished)
                .Include(x => x.PlayerCreationPoints)
                .Include(x => x.PlayerExperiencePoints)
                .Where(match => match.Username != "ufg" && match.PlayedMNR);

            //creator points
            if (game_type == GameType.OVERALL_CREATORS && leaderboardType == LeaderboardType.LIFETIME)
                users = users.OrderByDescending(u => u.CreatorPoints(platform));
            if (game_type == GameType.OVERALL_CREATORS && leaderboardType == LeaderboardType.WEEKLY)
                users = users.OrderByDescending(u => u.CreatorPointsThisWeek(platform));
            if (game_type == GameType.OVERALL_CREATORS && leaderboardType == LeaderboardType.LAST_WEEK)
                users = users.OrderByDescending(u => u.CreatorPointsLastWeek(platform));

            //creator points for characters
            if (game_type == GameType.CHARACTER_CREATORS && leaderboardType == LeaderboardType.LIFETIME)
                users = users.OrderByDescending(u => u.CreatorPoints(platform, PlayerCreationType.CHARACTER));
            if (game_type == GameType.CHARACTER_CREATORS && leaderboardType == LeaderboardType.WEEKLY)
                users = users.OrderByDescending(u => u.CreatorPointsThisWeek(platform, PlayerCreationType.CHARACTER));
            if (game_type == GameType.CHARACTER_CREATORS && leaderboardType == LeaderboardType.LAST_WEEK)
                users = users.OrderByDescending(u => u.CreatorPointsLastWeek(platform, PlayerCreationType.CHARACTER));

            //creator points for karts
            if (game_type == GameType.KART_CREATORS && leaderboardType == LeaderboardType.LIFETIME)
                users = users.OrderByDescending(u => u.CreatorPoints(platform, PlayerCreationType.KART));
            if (game_type == GameType.KART_CREATORS && leaderboardType == LeaderboardType.WEEKLY)
                users = users.OrderByDescending(u => u.CreatorPointsThisWeek(platform, PlayerCreationType.KART));
            if (game_type == GameType.KART_CREATORS && leaderboardType == LeaderboardType.LAST_WEEK)
                users = users.OrderByDescending(u => u.CreatorPointsLastWeek(platform, PlayerCreationType.KART));

            //creator points for tracks
            if (game_type == GameType.TRACK_CREATORS && leaderboardType == LeaderboardType.LIFETIME)
                users = users.OrderByDescending(u => u.CreatorPoints(platform, PlayerCreationType.TRACK));
            if (game_type == GameType.TRACK_CREATORS && leaderboardType == LeaderboardType.WEEKLY)
                users = users.OrderByDescending(u => u.CreatorPointsThisWeek(platform, PlayerCreationType.TRACK));
            if (game_type == GameType.TRACK_CREATORS && leaderboardType == LeaderboardType.LAST_WEEK)
                users = users.OrderByDescending(u => u.CreatorPointsLastWeek(platform, PlayerCreationType.TRACK));

            if (game_type == GameType.OVERALL_RACE)
            {
                switch (sort_column)
                {
                    case SortColumn.experience_points:
                        if (leaderboardType == LeaderboardType.LIFETIME)
                            users = users.OrderByDescending(u => u.ExperiencePoints(platform));
                        if (leaderboardType == LeaderboardType.WEEKLY)
                            users = users.OrderByDescending(u => u.ExperiencePointsThisWeek(platform));
                        if (leaderboardType == LeaderboardType.LAST_WEEK)
                            users = users.OrderByDescending(u => u.ExperiencePointsLastWeek(platform));
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

                    case SortColumn.points:
                        if (leaderboardType == LeaderboardType.LIFETIME)
                            users = users.OrderByDescending(u => u.PlayerPoints.Where(match => match.Platform == platform).Sum(p => p.Amount));
                        if (leaderboardType == LeaderboardType.WEEKLY)
                            users = users.OrderByDescending(u => u.PlayerPoints.Where(match => match.Platform == platform && match.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount));
                        if (leaderboardType == LeaderboardType.LAST_WEEK)
                            users = users.OrderByDescending(u => u.PlayerPoints.Where(match => match.Platform == platform && match.CreatedAt >= TimeUtils.LastWeekStart && match.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount));
                        break;

                    default:
                        break;
                }
            }

            return users.Select(u => u.UserId).ToList().FindIndex(match => match == UserId) + 1;
        }
    }
}
