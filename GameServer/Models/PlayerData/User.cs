using EntityFrameworkCore.Projectables;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Utils;
using System;
using System.Linq;
using GameServer.Implementation.Common;
using System.Collections.Generic;
using System.Globalization;
using GameServer.Models.Config;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models.PlayerData
{
    //TODO: figure out indexing on this table
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }

        [InverseProperty(nameof(HeartedProfile.HeartedUser))]
        public List<HeartedProfile> HeartedByProfiles { get; set; }

        [Projectable]
        public int Hearts => HeartedByProfiles.Count(match => !match.IsMNR);
        public Presence Presence(Database database, Platform platform, bool isMNR) => Session.GetPresence(database, UserId, platform, isMNR);
        public int Quota { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }
        public string Quote { get; set; }
        public ulong PSNID { get; set; }
        public ulong RPCNID { get; set; }

        public List<PlayerCreationData> PlayerCreations { get; set; }

        public int TotalTracks { get; set; }

        public List<PlayerPoint> PlayerPoints { get; set; }

        public float PointsPS3 { get; set; }
        public float PointsPSV { get; set; }
        public float PointsPSP { get; set; }
        [Projectable]
        public float Points(Platform platform) => platform switch
        {
            Platform.PSV => PointsPSV,
            Platform.PSP => PointsPSP,
            _ => PointsPS3 
        };
        
        public float PointsThisWeekPS3 { get; set; }
        public float PointsThisWeekPSV { get; set; }
        public float PointsThisWeekPSP { get; set; }
        [Projectable]
        public float PointsThisWeek(Platform platform) => platform switch
        {
            Platform.PSV => PointsThisWeekPSV,
            Platform.PSP => PointsThisWeekPSP,
            _ => PointsThisWeekPS3 
        };
        
        public float PointsLastWeekPS3 { get; set; }
        public float PointsLastWeekPSV { get; set; }
        public float PointsLastWeekPSP { get; set; }
        [Projectable]
        public float PointsLastWeek(Platform platform) => platform switch
        {
            Platform.PSV => PointsLastWeekPSV,
            Platform.PSP => PointsLastWeekPSP,
            _ => PointsLastWeekPS3 
        };
        
        public List<RaceStarted> RacesStarted { get; set; }
        public List<RaceFinished> RacesFinished { get; set; }

        public int OnlineRaces { get; set; }
        public int OnlineWins { get; set; }
        public int OnlineFinished { get; set; }
        public int OnlineForfeit { get; set; }
        public int OnlineDisconnected { get; set; }
        public int WinStreak { get; set; }
        public int LongestWinStreak { get; set; }
        public int OnlineRacesThisWeek { get; set; }
        public int OnlineWinsThisWeek { get; set; }
        public int OnlineFinishedThisWeek { get; set; }
        public int OnlineRacesLastWeek { get; set; }
        public int OnlineWinsLastWeek { get; set; }
        public int OnlineFinishedLastWeek { get; set; }
        public bool PolicyAccepted { get; set; }
        public bool IsBanned { get; set; }
        public bool ShowCreationsWithoutPreviews { get; set; }
        public bool AllowOppositePlatform { get; set; }
        public PrivacyType ProfilePrivacy { get; set; } = PrivacyType.AllowAll;
        public PrivacyType PlayerCreationsPrivacy { get; set; } = PrivacyType.AllowAll;

        //MNR
        public float LongestHangTime { get; set; }
        public float LongestDrift { get; set; }
        [Projectable]
        public int OnlineQuits => OnlineDisconnected+OnlineForfeit;
        public int CharacterIdx { get; set; }
        public int KartIdx { get; set; }
        public bool PlayedMNR { get; set; }
        public int TotalCharactersPS3 { get; set; }
        public int TotalCharactersPSV { get; set; }
        public int TotalCharactersPSP { get; set; }
        [Projectable]
        public int TotalCharacters(Platform platform) => platform switch
        {
            Platform.PSV => TotalCharactersPSV,
            Platform.PSP => TotalCharactersPSP,
            _ => TotalCharactersPS3 
        };
        public int TotalKartsPS3 { get; set; }
        public int TotalKartsPSV { get; set; }
        public int TotalKartsPSP { get; set; }
        [Projectable]
        public int TotalKarts(Platform platform) => platform switch
        {
            Platform.PSV => TotalKartsPSV,
            Platform.PSP => TotalKartsPSP,
            _ => TotalKartsPS3
        };
        public int TotalMNRTracksPS3 { get; set; }
        public int TotalMNRTracksPSV { get; set; }
        public int TotalMNRTracksPSP { get; set; }
        [Projectable]
        public int TotalMNRTracks(Platform platform) => platform switch
        {
            Platform.PSV => TotalMNRTracksPSV,
            Platform.PSP => TotalMNRTracksPSP,
            _ => TotalMNRTracksPS3
        };
        [Projectable]
        public int TotalPlayerCreations(Platform platform) => TotalCharacters(platform) + TotalKarts(platform) + TotalMNRTracks(platform);
        
        public List<PlayerCreationPoint> PlayerCreationPoints { get; set; }

        public float CharacterCreatorPointsPS3 { get; set; }
        public float CharacterCreatorPointsPSV { get; set; }
        public float CharacterCreatorPointsPSP { get; set; }
        public float KartCreatorPointsPS3 { get; set; }
        public float KartCreatorPointsPSV { get; set; }
        public float KartCreatorPointsPSP { get; set; }
        public float TrackCreatorPointsPS3 { get; set; }
        public float TrackCreatorPointsPSV { get; set; }
        public float TrackCreatorPointsPSP { get; set; }
        [Projectable]
        public float CreatorPoints(Platform platform, PlayerCreationType type) => type switch
        {
            PlayerCreationType.CHARACTER => platform switch
            {
                Platform.PSV => CharacterCreatorPointsPSV,
                Platform.PSP => CharacterCreatorPointsPSP,
                _ => CharacterCreatorPointsPS3
            },
            PlayerCreationType.KART => platform switch
            {
                Platform.PSV => KartCreatorPointsPSV,
                Platform.PSP => KartCreatorPointsPSP,
                _ => KartCreatorPointsPS3
            },
            _ => platform switch
            {
                Platform.PSV => TrackCreatorPointsPSV,
                Platform.PSP => TrackCreatorPointsPSP,
                _ => TrackCreatorPointsPS3
            }
        };
        
        public float CharacterCreatorPointsLastWeekPS3 { get; set; }
        public float CharacterCreatorPointsLastWeekPSV { get; set; }
        public float CharacterCreatorPointsLastWeekPSP { get; set; }
        public float KartCreatorPointsLastWeekPS3 { get; set; }
        public float KartCreatorPointsLastWeekPSV { get; set; }
        public float KartCreatorPointsLastWeekPSP { get; set; }
        public float TrackCreatorPointsLastWeekPS3 { get; set; }
        public float TrackCreatorPointsLastWeekPSV { get; set; }
        public float TrackCreatorPointsLastWeekPSP { get; set; }
        [Projectable]
        public float CreatorPointsLastWeek(Platform platform, PlayerCreationType type) => type switch
        {
            PlayerCreationType.CHARACTER => platform switch
            {
                Platform.PSV => CharacterCreatorPointsLastWeekPSV,
                Platform.PSP => CharacterCreatorPointsLastWeekPSP,
                _ => CharacterCreatorPointsLastWeekPS3
            },
            PlayerCreationType.KART => platform switch
            {
                Platform.PSV => KartCreatorPointsLastWeekPSV,
                Platform.PSP => KartCreatorPointsLastWeekPSP,
                _ => KartCreatorPointsLastWeekPS3
            },
            _ => platform switch
            {
                Platform.PSV => TrackCreatorPointsLastWeekPSV,
                Platform.PSP => TrackCreatorPointsLastWeekPSP,
                _ => TrackCreatorPointsLastWeekPS3
            }
        };
        
        public float CharacterCreatorPointsThisWeekPS3 { get; set; }
        public float CharacterCreatorPointsThisWeekPSV { get; set; }
        public float CharacterCreatorPointsThisWeekPSP { get; set; }
        public float KartCreatorPointsThisWeekPS3 { get; set; }
        public float KartCreatorPointsThisWeekPSV { get; set; }
        public float KartCreatorPointsThisWeekPSP { get; set; }
        public float TrackCreatorPointsThisWeekPS3 { get; set; }
        public float TrackCreatorPointsThisWeekPSV { get; set; }
        public float TrackCreatorPointsThisWeekPSP { get; set; }
        [Projectable]
        public float CreatorPointsThisWeek(Platform platform, PlayerCreationType type) => type switch
        {
            PlayerCreationType.CHARACTER => platform switch
            {
                Platform.PSV => CharacterCreatorPointsThisWeekPSV,
                Platform.PSP => CharacterCreatorPointsThisWeekPSP,
                _ => CharacterCreatorPointsThisWeekPS3
            },
            PlayerCreationType.KART => platform switch
            {
                Platform.PSV => KartCreatorPointsThisWeekPSV,
                Platform.PSP => KartCreatorPointsThisWeekPSP,
                _ => KartCreatorPointsThisWeekPS3
            },
            _ => platform switch
            {
                Platform.PSV => TrackCreatorPointsThisWeekPSV,
                Platform.PSP => TrackCreatorPointsThisWeekPSP,
                _ => TrackCreatorPointsThisWeekPS3
            }
        };
        
        public float CreatorPointsPS3 { get; set; }
        public float CreatorPointsPSV { get; set; }
        public float CreatorPointsPSP { get; set; }
        [Projectable]
        public float CreatorPoints(Platform platform) => platform switch
        {
            Platform.PSV => CreatorPointsPSV,
            Platform.PSP => CreatorPointsPSP,
            _ => CreatorPointsPS3
        };
        
        public float CreatorPointsLastWeekPS3 { get; set; }
        public float CreatorPointsLastWeekPSV { get; set; }
        public float CreatorPointsLastWeekPSP { get; set; }
        [Projectable]
        public float CreatorPointsLastWeek(Platform platform) => platform switch
        {
            Platform.PSV => CreatorPointsLastWeekPSV,
            Platform.PSP => CreatorPointsLastWeekPSP,
            _ => CreatorPointsLastWeekPS3
        };
        
        public float CreatorPointsThisWeekPS3 { get; set; }
        public float CreatorPointsThisWeekPSV { get; set; }
        public float CreatorPointsThisWeekPSP { get; set; }
        [Projectable]
        public float CreatorPointsThisWeek(Platform platform) => platform switch
        {
            Platform.PSV => CreatorPointsThisWeekPSV,
            Platform.PSP => CreatorPointsThisWeekPSP,
            _ => CreatorPointsThisWeekPS3
        };
        
        public List<PlayerExperiencePoint> PlayerExperiencePoints { get; set; }

        public float ExperiencePointsPS3 { get; set; }
        public float ExperiencePointsPSV { get; set; }
        public float ExperiencePointsPSP { get; set; }
        [Projectable]
        public float ExperiencePoints(Platform platform) => platform switch
        {
            Platform.PSV => ExperiencePointsPSV,
            Platform.PSP => ExperiencePointsPSP,
            _ => ExperiencePointsPS3
        };
        
        public float ExperiencePointsLastWeekPS3 { get; set; }
        public float ExperiencePointsLastWeekPSV { get; set; }
        public float ExperiencePointsLastWeekPSP { get; set; }
        [Projectable]
        public float ExperiencePointsLastWeek(Platform platform) => platform switch
        {
            Platform.PSV => ExperiencePointsLastWeekPSV,
            Platform.PSP => ExperiencePointsLastWeekPSP,
            _ => ExperiencePointsLastWeekPS3
        };
        
        public float ExperiencePointsThisWeekPS3 { get; set; }
        public float ExperiencePointsThisWeekPSV { get; set; }
        public float ExperiencePointsThisWeekPSP { get; set; }
        [Projectable]
        public float ExperiencePointsThisWeek(Platform platform) => platform switch
        {
            Platform.PSV => ExperiencePointsThisWeekPSV,
            Platform.PSP => ExperiencePointsThisWeekPSP,
            _ => ExperiencePointsThisWeekPS3
        };

        [InverseProperty(nameof(PlayerRatingData.Player))]
        public List<PlayerRatingData> PlayerRatings { get; set; }

        [Projectable]
        public float Rating => PlayerRatings.Count > 0 ? (float)PlayerRatings.Average(r => r.Rating) : 0;
        [Projectable]
        public string StarRating => Rating.ToString("0.0", CultureInfo.InvariantCulture);
        
        public float TotalXPPS3 { get; set; }
        public float TotalXPPSV { get; set; }
        public float TotalXPPSP { get; set; }
        [Projectable]
        public float TotalXP(Platform platform) => platform switch
        {
            Platform.PSV => TotalXPPSV,
            Platform.PSP => TotalXPPSP,
            _ => TotalXPPS3
        };
        
        public float TotalXPLastWeekPS3 { get; set; }
        public float TotalXPLastWeekPSV { get; set; }
        public float TotalXPLastWeekPSP { get; set; }
        [Projectable]
        public float TotalXPLastWeek(Platform platform) => platform switch
        {
            Platform.PSV => TotalXPLastWeekPSV,
            Platform.PSP => TotalXPLastWeekPSP,
            _ => TotalXPLastWeekPS3
        };
        
        public float TotalXPThisWeekPS3 { get; set; }
        public float TotalXPThisWeekPSV { get; set; }
        public float TotalXPThisWeekPSP { get; set; }
        [Projectable]
        public float TotalXPThisWeek(Platform platform) => platform switch
        {
            Platform.PSV => TotalXPThisWeekPSV,
            Platform.PSP => TotalXPThisWeekPSP,
            _ => TotalXPThisWeekPS3
        };
        [Projectable]
        public int SkillLevelId(Platform platform) => SkillConfig.Instance.GetSkillLevel((int)float.Floor(TotalXP(platform))).Id;
        [Projectable]
        public string SkillLevelName(Platform platform) => SkillConfig.Instance.GetSkillLevel((int)float.Floor(TotalXP(platform))).Name;
        
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

        [InverseProperty(nameof(BlockedUser.BlockedPlayer))]
        public List<BlockedUser> BlockedByUsers { get; set; }
        
        [Projectable]
        public bool IsBlockedByMe(int id) => BlockedByUsers.Any(match => match.UserId == id);
        
        [InverseProperty(nameof(Buddy.BuddyUser))]
        public List<Buddy> BuddiesWithUsers { get; set; }
        
        [Projectable]
        public bool IsBuddyWithMe(int id) => BuddiesWithUsers.Any(match => match.UserId == id);
<<<<<<< HEAD

        public int GetRank(Database database, GameType game_type, LeaderboardType leaderboardType, Platform platform, SortColumn sort_column)
        {
            var users = database.Users
                .AsNoTracking()
                .AsSplitQuery()
                .Include(x => x.PlayerPoints)
                .Include(x => x.RacesStarted)
                .Include(x => x.RacesFinished)
                .Include(x => x.PlayerCreationPoints)
                .Include(x => x.PlayerExperiencePoints)
                .Where(match => match.Username != "ufg" && match.PlayedMNR && !match.IsBanned);

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
=======
>>>>>>> upstream/dev
    }
}
