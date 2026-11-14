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
    //TODO: calculated params
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

        [Projectable]
        public int TotalTracks => PlayerCreations.Count(match => match.Type == PlayerCreationType.TRACK && !match.IsMNR);
        //[Projectable]
        //public int Rank(Database database) => GetRank(database, GameType.OVERALL, LeaderboardType.LIFETIME, Platform.PS3, SortColumn.points);

        public List<PlayerPoint> PlayerPoints { get; set; }

        public int PointsPS3 { get; set; }
        public int PointsPSV { get; set; }
        public int PointsPSP { get; set; }
        [Projectable]
        public float Points(Platform platform) => platform switch
        {
            Platform.PSV => PointsPSV,
            Platform.PSP => PointsPSP,
            _ => PointsPS3 
        };//PlayerPoints.Count >= 10 ? float.Clamp((float)PlayerPoints.Where(match => match.Platform == platform).Average(p => p.Amount), 0, 3000) : 1500;
        
        public int PointsThisWeekPS3 { get; set; }
        public int PointsThisWeekPSV { get; set; }
        public int PointsThisWeekPSP { get; set; }
        [Projectable]
        public float PointsThisWeek(Platform platform) => platform switch
        {
            Platform.PSV => PointsThisWeekPSV,
            Platform.PSP => PointsThisWeekPSP,
            _ => PointsThisWeekPS3 
        };
        
        public int PointsLastWeekPS3 { get; set; }
        public int PointsLastWeekPSV { get; set; }
        public int PointsLastWeekPSP { get; set; }
        [Projectable]
        public float PointsLastWeek(Platform platform) => platform switch
        {
            Platform.PSV => PointsLastWeekPSV,
            Platform.PSP => PointsLastWeekPSP,
            _ => PointsLastWeekPS3 
        };
        
        public List<RaceStarted> RacesStarted { get; set; }
        public List<RaceFinished> RacesFinished { get; set; }

        //[Projectable]
        public int OnlineRaces { get; set; }//=> RacesStarted.Count;
        //[Projectable]
        public int OnlineWins { get; set; }//=> RacesFinished.Count(match => match.IsWinner);
        //[Projectable]
        public int OnlineFinished { get; set; }//=> RacesFinished.Count;
        public int OnlineForfeit { get; set; }
        public int OnlineDisconnected { get; set; }
        public int WinStreak { get; set; }
        public int LongestWinStreak { get; set; }
        //[Projectable]
        public int OnlineRacesThisWeek { get; set; }//=> RacesStarted.Count(match => match.StartedAt >= TimeUtils.ThisWeekStart);
        //[Projectable]
        public int OnlineWinsThisWeek { get; set; }//=> RacesFinished.Count(match => match.IsWinner && match.FinishedAt >= TimeUtils.ThisWeekStart);
        //[Projectable]
        public int OnlineFinishedThisWeek { get; set; }//=> RacesFinished.Count(match => match.FinishedAt >= TimeUtils.ThisWeekStart);
        //[Projectable]
        public int OnlineRacesLastWeek { get; set; }//=> RacesStarted.Count(match => match.StartedAt >= TimeUtils.LastWeekStart && match.StartedAt < TimeUtils.ThisWeekStart);
        //[Projectable]
        public int OnlineWinsLastWeek { get; set; }//=> RacesFinished.Count(match => match.IsWinner && match.FinishedAt >= TimeUtils.LastWeekStart && match.FinishedAt < TimeUtils.ThisWeekStart);
        //[Projectable]
        public int OnlineFinishedLastWeek { get; set; }//=> RacesFinished.Count(match => match.FinishedAt >= TimeUtils.LastWeekStart && match.FinishedAt < TimeUtils.ThisWeekStart);
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
        };//PlayerCreations.Count(match => match.Type == PlayerCreationType.CHARACTER && match.Platform == platform);
        public int TotalKartsPS3 { get; set; }
        public int TotalKartsPSV { get; set; }
        public int TotalKartsPSP { get; set; }
        [Projectable]
        public int TotalKarts(Platform platform) => platform switch
        {
            Platform.PSV => TotalKartsPSV,
            Platform.PSP => TotalKartsPSP,
            _ => TotalKartsPS3
        };//PlayerCreations.Count(match => match.Type == PlayerCreationType.KART && match.Platform == platform);
        public int TotalTracksPS3 { get; set; }
        public int TotalTracksPSV { get; set; }
        public int TotalTracksPSP { get; set; }
        [Projectable]
        public int TotalMNRTracks(Platform platform) => platform switch
        {
            Platform.PSV => TotalTracksPSV,
            Platform.PSP => TotalTracksPSP,
            _ => TotalTracksPS3
        };//PlayerCreations.Count(match => match.Type == PlayerCreationType.TRACK && match.IsMNR && match.Platform == platform);
        [Projectable]
        public int TotalPlayerCreations(Platform platform) => TotalCharacters(platform) + TotalKarts(platform) + TotalMNRTracks(platform);//PlayerCreations.Count(match => match.Type != PlayerCreationType.PHOTO && match.Type != PlayerCreationType.DELETED && match.IsMNR && match.Platform == platform);
        
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
        };//PlayerCreationPoints.Where(match => match.Platform == platform && match.Type == type).Sum(p => p.Amount);
        
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
        };//PlayerCreationPoints.Where(match => match.Platform == platform && match.Type == type && match.CreatedAt >= TimeUtils.LastWeekStart && match.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount);
        
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
        };//PlayerCreationPoints.Where(match => match.Platform == platform && match.Type == type && match.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount);
        
        public float CreatorPointsPS3 { get; set; }
        public float CreatorPointsPSV { get; set; }
        public float CreatorPointsPSP { get; set; }
        [Projectable]
        public float CreatorPoints(Platform platform) => platform switch
        {
            Platform.PSV => CreatorPointsPSV,
            Platform.PSP => CreatorPointsPSP,
            _ => CreatorPointsPS3
        };//PlayerCreationPoints.Where(match => match.Platform == platform).Sum(p => p.Amount);
        
        public float CreatorPointsLastWeekPS3 { get; set; }
        public float CreatorPointsLastWeekPSV { get; set; }
        public float CreatorPointsLastWeekPSP { get; set; }
        [Projectable]
        public float CreatorPointsLastWeek(Platform platform) => platform switch
        {
            Platform.PSV => CreatorPointsLastWeekPSV,
            Platform.PSP => CreatorPointsLastWeekPSP,
            _ => CreatorPointsLastWeekPS3
        };//PlayerCreationPoints.Where(match => match.Platform == platform && match.CreatedAt >= TimeUtils.LastWeekStart && match.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount);
        
        public float CreatorPointsThisWeekPS3 { get; set; }
        public float CreatorPointsThisWeekPSV { get; set; }
        public float CreatorPointsThisWeekPSP { get; set; }
        [Projectable]
        public float CreatorPointsThisWeek(Platform platform) => platform switch
        {
            Platform.PSV => CreatorPointsThisWeekPSV,
            Platform.PSP => CreatorPointsThisWeekPSP,
            _ => CreatorPointsThisWeekPS3
        };//PlayerCreationPoints.Where(match => match.Platform == platform && match.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount);
        
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
        };//PlayerExperiencePoints.Where(match => match.Platform == platform).Sum(p => p.Amount);
        
        public float ExperiencePointsLastWeekPS3 { get; set; }
        public float ExperiencePointsLastWeekPSV { get; set; }
        public float ExperiencePointsLastWeekPSP { get; set; }
        [Projectable]
        public float ExperiencePointsLastWeek(Platform platform) => platform switch
        {
            Platform.PSV => ExperiencePointsLastWeekPSV,
            Platform.PSP => ExperiencePointsLastWeekPSP,
            _ => ExperiencePointsLastWeekPS3
        };//PlayerExperiencePoints.Where(match => match.Platform == platform && match.CreatedAt >= TimeUtils.LastWeekStart && match.CreatedAt < TimeUtils.ThisWeekStart).Sum(p => p.Amount);
        
        public float ExperiencePointsThisWeekPS3 { get; set; }
        public float ExperiencePointsThisWeekPSV { get; set; }
        public float ExperiencePointsThisWeekPSP { get; set; }
        [Projectable]
        public float ExperiencePointsThisWeek(Platform platform) => platform switch
        {
            Platform.PSV => ExperiencePointsThisWeekPSV,
            Platform.PSP => ExperiencePointsThisWeekPSP,
            _ => ExperiencePointsThisWeekPS3
        };//PlayerExperiencePoints.Where(match => match.Platform == platform && match.CreatedAt >= TimeUtils.ThisWeekStart).Sum(p => p.Amount);

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
        };//ExperiencePoints(platform) + CreatorPoints(platform);
        
        public float TotalXPLastWeekPS3 { get; set; }
        public float TotalXPLastWeekPSV { get; set; }
        public float TotalXPLastWeekPSP { get; set; }
        [Projectable]
        public float TotalXPLastWeek(Platform platform) => platform switch
        {
            Platform.PSV => TotalXPLastWeekPSV,
            Platform.PSP => TotalXPLastWeekPSP,
            _ => TotalXPLastWeekPS3
        };//ExperiencePointsLastWeek(platform) + CreatorPointsLastWeek(platform);
        
        public float TotalXPThisWeekPS3 { get; set; }
        public float TotalXPThisWeekPSV { get; set; }
        public float TotalXPThisWeekPSP { get; set; }
        [Projectable]
        public float TotalXPThisWeek(Platform platform) => platform switch
        {
            Platform.PSV => TotalXPThisWeekPSV,
            Platform.PSP => TotalXPThisWeekPSP,
            _ => TotalXPThisWeekPS3
        };//ExperiencePointsThisWeek(platform) + CreatorPointsThisWeek(platform);
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
    }
}
