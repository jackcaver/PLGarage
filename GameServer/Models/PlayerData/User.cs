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

namespace GameServer.Models.PlayerData
{
    public class User
    {
        private Database _database;
        private Database database
        {
            get
            {
                if (this._database != null) return this._database;
                return this._database = new Database();
            }
            set => this._database = value;
        }

        public int UserId { get; set; }
        public string Username { get; set; }
        public int Hearts => this.database.HeartedProfiles.Count(match => match.HeartedUserId == this.UserId);
        public Presence Presence => Session.GetPresence(this.Username);
        public int Quota { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Quote { get; set; }
        public ulong PSNID { get; set; }
        public ulong RPCNID { get; set; }
        public int TotalTracks => this.database.PlayerCreations.Count(match => match.PlayerId == this.UserId && match.Type == PlayerCreationType.TRACK && !match.IsMNR);
        public int Rank => this.GetRank(GameType.OVERALL, LeaderboardType.LIFETIME, Platform.PS3, SortColumn.points);
        public int Points => this.database.PlayerPoints.Where(match => match.PlayerId == UserId).Sum(p => p.Amount);
        public int OnlineRaces => this.database.OnlineRacesStarted.Count(match => match.PlayerId == UserId);
        public int OnlineWins => this.database.OnlineRacesFinished.Count(match => match.PlayerId == UserId && match.IsWinner);
        public int OnlineFinished => this.database.OnlineRacesFinished.Count(match => match.PlayerId == UserId);
        public int OnlineForfeit { get; set; }
        public int OnlineDisconnected { get; set; }
        public int WinStreak { get; set; }
        public int LongestWinStreak { get; set; }
        public int OnlineRacesThisWeek => this.database.OnlineRacesStarted.Count(match => match.PlayerId == UserId && match.StartedAt >= DateTime.UtcNow.AddDays(-7) && match.StartedAt <= DateTime.UtcNow);
        public int OnlineWinsThisWeek => this.database.OnlineRacesFinished.Count(match => match.PlayerId == UserId && match.IsWinner && match.FinishedAt >= DateTime.UtcNow.AddDays(-7) && match.FinishedAt <= DateTime.UtcNow);
        public int OnlineFinishedThisWeek => this.database.OnlineRacesFinished.Count(match => match.PlayerId == UserId && match.FinishedAt >= DateTime.UtcNow.AddDays(-7) && match.FinishedAt <= DateTime.UtcNow);
        public int OnlineRacesLastWeek => this.database.OnlineRacesStarted.Count(match => match.PlayerId == UserId && match.StartedAt >= DateTime.UtcNow.AddDays(-14) && match.StartedAt <= DateTime.UtcNow.AddDays(-7));
        public int OnlineWinsLastWeek => this.database.OnlineRacesFinished.Count(match => match.PlayerId == UserId && match.IsWinner && match.FinishedAt >= DateTime.UtcNow.AddDays(-14) && match.FinishedAt <= DateTime.UtcNow.AddDays(-7));
        public int OnlineFinishedLastWeek => this.database.OnlineRacesFinished.Count(match => match.PlayerId == UserId && match.FinishedAt >= DateTime.UtcNow.AddDays(-14) && match.FinishedAt <= DateTime.UtcNow.AddDays(-7));
        public bool PolicyAccepted { get; set; }
        public bool IsBanned { get; set; }
        //MNR
        public float LongestHangTime { get; set; }
        public float LongestDrift { get; set; }
        public int OnlineQuits => OnlineDisconnected+OnlineForfeit;
        public int CharacterIdx { get; set; }
        public int KartIdx { get; set; }
        public bool PlayedMNR { get; set; }
        public int TotalCharacters(Platform platform) => this.database.PlayerCreations.Count(match => match.PlayerId == this.UserId && match.Type == PlayerCreationType.CHARACTER && match.Platform == platform);
        public int TotalKarts(Platform platform) => this.database.PlayerCreations.Count(match => match.PlayerId == this.UserId && match.Type == PlayerCreationType.KART && match.Platform == platform);
        public int TotalPlayerCreations(Platform platform) => this.database.PlayerCreations.Count(match => match.PlayerId == this.UserId && match.Type != PlayerCreationType.PHOTO && match.IsMNR && match.Platform == platform);
        public int TotalMNRTracks(Platform platform) => this.database.PlayerCreations.Count(match => match.PlayerId == this.UserId && match.Type == PlayerCreationType.TRACK && match.IsMNR && match.Platform == platform);
        public float CreatorPoints(Platform platform) => this.database.PlayerCreationPoints.Where(match => match.PlayerId == this.UserId && match.Platform == platform).Sum(p => p.Amount);
        public float CreatorPointsLastWeek(Platform platform) => this.database.PlayerCreationPoints.Where(match => match.PlayerId == this.UserId && match.Platform == platform && match.CreatedAt >= DateTime.UtcNow.AddDays(-14) && match.CreatedAt <= DateTime.UtcNow.AddDays(-7)).Sum(p => p.Amount);
        public float CreatorPointsThisWeek(Platform platform) => this.database.PlayerCreationPoints.Where(match => match.PlayerId == this.UserId && match.Platform == platform && match.CreatedAt >= DateTime.UtcNow.AddDays(-7) && match.CreatedAt <= DateTime.UtcNow).Sum(p => p.Amount);
        public float CreatorPoints(Platform platform, PlayerCreationType type) => this.database.PlayerCreationPoints.Where(match => match.PlayerId == this.UserId && match.Platform == platform && match.Type == type).Sum(p => p.Amount);
        public float CreatorPointsLastWeek(Platform platform, PlayerCreationType type) => this.database.PlayerCreationPoints.Where(match => match.PlayerId == this.UserId && match.Platform == platform && match.Type == type && match.CreatedAt >= DateTime.UtcNow.AddDays(-14) && match.CreatedAt <= DateTime.UtcNow.AddDays(-7)).Sum(p => p.Amount);
        public float CreatorPointsThisWeek(Platform platform, PlayerCreationType type) => this.database.PlayerCreationPoints.Where(match => match.PlayerId == this.UserId && match.Platform == platform && match.Type == type && match.CreatedAt >= DateTime.UtcNow.AddDays(-7) && match.CreatedAt <= DateTime.UtcNow).Sum(p => p.Amount);
        public float ExperiencePoints => this.database.PlayerExperiencePoints.Where(match => match.PlayerId == UserId).Sum(p => p.Amount);
        public float ExperiencePointsLastWeek => this.database.PlayerExperiencePoints.Where(match => match.PlayerId == UserId && match.CreatedAt >= DateTime.UtcNow.AddDays(-14) && match.CreatedAt <= DateTime.UtcNow.AddDays(-7)).Sum(p => p.Amount);
        public float ExperiencePointsThisWeek => this.database.PlayerExperiencePoints.Where(match => match.PlayerId == UserId && match.CreatedAt >= DateTime.UtcNow.AddDays(-7) && match.CreatedAt <= DateTime.UtcNow).Sum(p => p.Amount);
        public int PointsLastWeek => this.database.PlayerPoints.Where(match => match.PlayerId == UserId && match.CreatedAt >= DateTime.UtcNow.AddDays(-14) && match.CreatedAt <= DateTime.UtcNow.AddDays(-7)).Sum(p => p.Amount);
        public int PointsThisWeek => this.database.PlayerPoints.Where(match => match.PlayerId == UserId && match.CreatedAt >= DateTime.UtcNow.AddDays(-7) && match.CreatedAt <= DateTime.UtcNow).Sum(p => p.Amount);
        public float Rating => this.database.PlayerRatings.Count(match => match.PlayerId == this.UserId) != 0 ? (float)this.database.PlayerRatings.Where(match => match.PlayerId == this.UserId).Average(r => r.Rating) : 0;
        public string StarRating => Rating.ToString("0.0", CultureInfo.InvariantCulture);
        public int SkillLevelId(Platform platform) => SkillConfig.Instance.GetSkillLevel((int)Math.Floor(TotalXP(platform))).Id;
        public string SkillLevelName(Platform platform) => SkillConfig.Instance.GetSkillLevel((int)Math.Floor(TotalXP(platform))).Name;
        public float TotalXP(Platform platform) => (platform == Platform.PSV ? 0 : ExperiencePoints) + CreatorPoints(platform);
        public float TotalXPLastWeek(Platform platform) => (platform == Platform.PSV ? 0 : ExperiencePoints) + CreatorPointsLastWeek(platform);
        public float TotalXPThisWeek(Platform platform) => (platform == Platform.PSV ? 0 : ExperiencePoints) + CreatorPointsThisWeek(platform);
        //MNR: Road Trip
        public int TravelPoints => this.database.TravelPoints.Where(match => match.PlayerId == UserId).Sum(p => p.Amount);
        public int TravelPointsThisWeek => this.database.TravelPoints.Where(match => match.PlayerId == UserId && match.CreatedAt >= DateTime.UtcNow.AddDays(-7) && match.CreatedAt <= DateTime.UtcNow).Sum(p => p.Amount);
        public int TravelPointsLastWeek => this.database.TravelPoints.Where(match => match.PlayerId == UserId && match.CreatedAt >= DateTime.UtcNow.AddDays(-14) && match.CreatedAt <= DateTime.UtcNow.AddDays(-7)).Sum(p => p.Amount);
        public int Visits => this.database.POIVisits.Count(match => match.PlayerId == UserId);
        public int VisitsThisWeek => this.database.POIVisits.Count(match => match.PlayerId == UserId && match.CreatedAt >= DateTime.UtcNow.AddDays(-7) && match.CreatedAt <= DateTime.UtcNow);
        public int VisitsLastWeek => this.database.POIVisits.Count(match => match.PlayerId == UserId && match.CreatedAt >= DateTime.UtcNow.AddDays(-14) && match.CreatedAt <= DateTime.UtcNow.AddDays(-7));
        public bool HasCheckedInBefore { get; set; }
        public float ModMiles { get; set; }
        public float LastLatitude { get; set; }
        public float LastLongitude { get; set; }

        public bool IsHeartedByMe(int id, bool IsMNR) 
        {
            var entry = this.database.HeartedProfiles.FirstOrDefault(match => match.HeartedUserId == this.UserId && match.UserId == id && match.IsMNR == IsMNR);
            return entry != null;
        }
        public int GetRank(GameType game_type, LeaderboardType leaderboardType, Platform platform, SortColumn sort_column)
        {
            List<User> users = database.Users.Where(match => match.Username != "ufg" && match.PlayedMNR).ToList();
            //creator points
            if (game_type == GameType.OVERALL_CREATORS && leaderboardType == LeaderboardType.LIFETIME)
                users.Sort((curr, prev) => prev.CreatorPoints(platform).CompareTo(curr.CreatorPoints(platform)));
            if (game_type == GameType.OVERALL_CREATORS && leaderboardType == LeaderboardType.WEEKLY)
                users.Sort((curr, prev) => prev.CreatorPointsThisWeek(platform).CompareTo(curr.CreatorPointsThisWeek(platform)));
            if (game_type == GameType.OVERALL_CREATORS && leaderboardType == LeaderboardType.LAST_WEEK)
                users.Sort((curr, prev) => prev.CreatorPointsLastWeek(platform).CompareTo(curr.CreatorPointsLastWeek(platform)));

            //creator points for characters
            if (game_type == GameType.CHARACTER_CREATORS && leaderboardType == LeaderboardType.LIFETIME)
                users.Sort((curr, prev) => prev.CreatorPoints(platform, PlayerCreationType.CHARACTER).CompareTo(curr.CreatorPoints(platform, PlayerCreationType.CHARACTER)));
            if (game_type == GameType.CHARACTER_CREATORS && leaderboardType == LeaderboardType.WEEKLY)
                users.Sort((curr, prev) => prev.CreatorPointsThisWeek(platform, PlayerCreationType.CHARACTER).CompareTo(curr.CreatorPointsThisWeek(platform, PlayerCreationType.CHARACTER)));
            if (game_type == GameType.CHARACTER_CREATORS && leaderboardType == LeaderboardType.LAST_WEEK)
                users.Sort((curr, prev) => prev.CreatorPointsLastWeek(platform, PlayerCreationType.CHARACTER).CompareTo(curr.CreatorPointsLastWeek(platform, PlayerCreationType.CHARACTER)));

            //creator points for karts
            if (game_type == GameType.KART_CREATORS && leaderboardType == LeaderboardType.LIFETIME)
                users.Sort((curr, prev) => prev.CreatorPoints(platform, PlayerCreationType.KART).CompareTo(curr.CreatorPoints(platform, PlayerCreationType.KART)));
            if (game_type == GameType.KART_CREATORS && leaderboardType == LeaderboardType.WEEKLY)
                users.Sort((curr, prev) => prev.CreatorPointsThisWeek(platform, PlayerCreationType.KART).CompareTo(curr.CreatorPointsThisWeek(platform, PlayerCreationType.KART)));
            if (game_type == GameType.KART_CREATORS && leaderboardType == LeaderboardType.LAST_WEEK)
                users.Sort((curr, prev) => prev.CreatorPointsLastWeek(platform, PlayerCreationType.KART).CompareTo(curr.CreatorPointsLastWeek(platform, PlayerCreationType.KART)));

            //creator points for tracks
            if (game_type == GameType.TRACK_CREATORS && leaderboardType == LeaderboardType.LIFETIME)
                users.Sort((curr, prev) => prev.CreatorPoints(platform, PlayerCreationType.TRACK).CompareTo(curr.CreatorPoints(platform, PlayerCreationType.TRACK)));
            if (game_type == GameType.TRACK_CREATORS && leaderboardType == LeaderboardType.WEEKLY)
                users.Sort((curr, prev) => prev.CreatorPointsThisWeek(platform, PlayerCreationType.TRACK).CompareTo(curr.CreatorPointsThisWeek(platform, PlayerCreationType.TRACK)));
            if (game_type == GameType.TRACK_CREATORS && leaderboardType == LeaderboardType.LAST_WEEK)
                users.Sort((curr, prev) => prev.CreatorPointsLastWeek(platform, PlayerCreationType.TRACK).CompareTo(curr.CreatorPointsLastWeek(platform, PlayerCreationType.TRACK)));

            //Experience points
            if (game_type == GameType.OVERALL && leaderboardType == LeaderboardType.LIFETIME)
                users.Sort((curr, prev) => prev.TotalXP(platform).CompareTo(curr.TotalXP(platform)));
            if (game_type == GameType.OVERALL && leaderboardType == LeaderboardType.WEEKLY)
                users.Sort((curr, prev) => prev.TotalXPThisWeek(platform).CompareTo(curr.TotalXPThisWeek(platform)));
            if (game_type == GameType.OVERALL && leaderboardType == LeaderboardType.LAST_WEEK)
                users.Sort((curr, prev) => prev.TotalXPLastWeek(platform).CompareTo(curr.TotalXPLastWeek(platform)));

            if (game_type == GameType.OVERALL_RACE)
            {
                switch (sort_column)
                {
                    case SortColumn.experience_points:
                        if (leaderboardType == LeaderboardType.LIFETIME)
                            users.Sort((curr, prev) => prev.ExperiencePoints.CompareTo(curr.ExperiencePoints));
                        if (leaderboardType == LeaderboardType.WEEKLY)
                            users.Sort((curr, prev) => prev.ExperiencePointsThisWeek.CompareTo(curr.ExperiencePointsThisWeek));
                        if (leaderboardType == LeaderboardType.LAST_WEEK)
                            users.Sort((curr, prev) => prev.ExperiencePointsLastWeek.CompareTo(curr.ExperiencePointsLastWeek));
                        break;

                    case SortColumn.online_races:
                        users.Sort((curr, prev) => prev.OnlineRaces.CompareTo(curr.OnlineRaces));
                        break;

                    case SortColumn.online_wins:
                        users.Sort((curr, prev) => prev.OnlineWins.CompareTo(curr.OnlineWins));
                        break;

                    case SortColumn.longest_win_streak:
                        users.Sort((curr, prev) => prev.LongestWinStreak.CompareTo(curr.LongestWinStreak));
                        break;

                    case SortColumn.win_streak:
                        users.Sort((curr, prev) => prev.WinStreak.CompareTo(curr.WinStreak));
                        break;

                    case SortColumn.longest_hang_time:
                        users.Sort((curr, prev) => prev.LongestHangTime.CompareTo(curr.LongestHangTime));
                        break;

                    case SortColumn.longest_drift:
                        users.Sort((curr, prev) => prev.LongestDrift.CompareTo(curr.LongestDrift));
                        break;

                    default:
                        break;
                }
            }

            return users.FindIndex(match => match == this) + 1;
        }
    }
}
