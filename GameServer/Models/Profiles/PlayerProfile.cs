using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using AutoMapper;
using GameServer.Models.Common;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Response;

namespace GameServer.Models.Profiles
{
    public class PlayerProfile : Profile
    {
        public PlayerProfile()
        {
            #region Variables

            User requestedBy = null;
            SessionInfo session = null;

            #endregion

            #region FavoritePlayers

            CreateMap<HeartedProfile, FavoritePlayer>()
                .ForMember(dto => dto.FavoritePlayerId, cfg => cfg.MapFrom(db => db.HeartedUser.UserId))

                // TODO: Include?
                .ForMember(dto => dto.HeartedByMe, cfg => cfg.MapFrom(db => requestedBy != null && db.HeartedUser.HeartedProfileFromOthers.Any(match => match.User.UserId == requestedBy.UserId) ? 1 : 0))
                .ForMember(dto => dto.Hearts, cfg => cfg.MapFrom(db => db.HeartedUser.HeartedProfileFromOthers.Count()))

                // TODO: Do we need Players.Count + 1 for id in favorite_player response object, can it not just be databse id for the hearted player?
                .ForMember(dto => dto.Quote, cfg => cfg.MapFrom(db => db.HeartedUser.Quote))
                .ForMember(dto => dto.TotalTracks, cfg => cfg.MapFrom(db => db.HeartedUser.PlayerCreations.Count(match => match.Type == PlayerCreationType.TRACK && !match.IsMNR)))
                .ForMember(dto => dto.Username, cfg => cfg.MapFrom(db => db.HeartedUser.Username));

            #endregion

            #region MailMessages

            CreateMap<MailMessageData, mailMessage>()    // TODO: Update naming on mailMessage
                                                         // TODO: !!! IMPORTANT !!! NAMING CONSISTENCY ISSUES BETWEEN MODELS
                .ForMember(dto => dto.CreatedAt, cfg => cfg.MapFrom(db => db.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz")))
                .ForMember(dto => dto.MailMessageType, cfg => cfg.MapFrom(db => db.Type.ToString()))    // Naming different

                .ForMember(dto => dto.RecipientId, cfg => cfg.MapFrom(db => db.Recipient.UserId))
                .ForMember(dto => dto.RecipientList, cfg => cfg.MapFrom(db => string.Join(", ", db.RecipientList.Select(match => match.Username))))   // TODO: Will this compile with string.Join?
                .ForMember(dto => dto.SenderId, cfg => cfg.MapFrom(db => db.Sender.UserId))
                .ForMember(dto => dto.SenderName, cfg => cfg.MapFrom(db => db.Sender.Username))

                .ForMember(dto => dto.UpdatedAt, cfg => cfg.MapFrom(db => db.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz")));
            CreateMap<MailMessageData, MailMessage>()    // TODO: Why is this different?
                .ForMember(dto => dto.CreatedAt, cfg => cfg.MapFrom(db => db.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz")))
                
                .ForMember(dto => dto.SenderId, cfg => cfg.MapFrom(db => db.Sender.UserId))
                .ForMember(dto => dto.SenderName, cfg => cfg.MapFrom(db => db.Sender.Username))

                .ForMember(dto => dto.UpdatedAt, cfg => cfg.MapFrom(db => db.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz")));

            #endregion

            #region ModMile

            Timespan timespan;

            CreateMap<POIVisit, ModMileLeaderboardStat>()
                .ForMember(dto => dto.CreatedAt, cfg => cfg.MapFrom(db => db.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz")))

            #endregion

            #region PlayerComments

            CreateMap<PlayerCommentData, PlayerComment>()
                .ForMember(dto => dto.AuthorId, cfg => cfg.MapFrom(db => db.Author.UserId))
                .ForMember(dto => dto.AuthorUsername, cfg => cfg.MapFrom(db => db.Author.Username))

                .ForMember(dto => dto.CreatedAt, cfg => cfg.MapFrom(db => db.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz")))
                .ForMember(dto => dto.UpdatedAt, cfg => cfg.MapFrom(db => db.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz")))

                .ForMember(dto => dto.PlayerId, cfg => cfg.MapFrom(db => db.Player.UserId))
                .ForMember(dto => dto.Username, cfg => cfg.MapFrom(db => db.Player.Username))

                .ForMember(dto => dto.RatingDown, cfg => cfg.MapFrom(db => db.Ratings.Count(match => match.Type == RatingType.BOO)))
                .ForMember(dto => dto.RatingUp, cfg => cfg.MapFrom(db => db.Ratings.Count(match => match.Type == RatingType.YAY)))
                .ForMember(dto => dto.RatedByMe, cfg => cfg.MapFrom(db => requestedBy != null ? db.Ratings.Any(match => match.Player.UserId == requestedBy.UserId) : false));   // TODO: does bool serialise correctly? Why is everything not bool?

            #endregion

            #region PlayerProfiles

            CreateMap<User, PlayerProfileResponse>()
                .ForMember(dto => dto.Id, cfg => cfg.MapFrom(db => db.UserId))
                .ForMember(dto => dto.PlayerId, cfg => cfg.MapFrom(db => db.UserId))    // TODO: Should both of these be UserId?

                .ForMember(dto => dto.City, cfg => cfg.MapFrom(db => ""))   // TODO
                .ForMember(dto => dto.State, cfg => cfg.MapFrom(db => ""))   // TODO
                .ForMember(dto => dto.Province, cfg => cfg.MapFrom(db => ""))   // TODO
                .ForMember(dto => dto.Country, cfg => cfg.MapFrom(db => ""))   // TODO

                .ForMember(dto => dto.CreatedAt, cfg => cfg.MapFrom(db => db.CreatedAt.ToString("yyyy-MM-ddThh:mm:sszzz")))

                .ForMember(dto => dto.HeartedByMe, cfg => cfg.MapFrom(db => ))
                .ForMember(dto => dto.Hearts, cfg => cfg.MapFrom(db => db.HeartedProfileFromOthers.Count()))

                .ForMember(dto => dto.OnlineFinished, cfg => cfg.MapFrom(db => db.OnlineRacesFinished.Count()))
                .ForMember(dto => dto.OnlineFinishedLastWeek, cfg => cfg.MapFrom(db => db.OnlineRacesFinished.Count(match => match.FinishedAt >= DateTime.UtcNow.AddDays(-14) && match.FinishedAt <= DateTime.UtcNow.AddDays(-7))))
                .ForMember(dto => dto.OnlineFinishedThisWeek, cfg => cfg.MapFrom(db => db.OnlineRacesFinished.Count(match => match.FinishedAt >= DateTime.UtcNow.AddDays(-7) && match.FinishedAt <= DateTime.UtcNow)))

                .ForMember(dto => dto.OnlineRaces, cfg => cfg.MapFrom(db => db.OnlineRacesStarted.Count()))
                .ForMember(dto => dto.OnlineRacesLastWeek, cfg => cfg.MapFrom(db => db.OnlineRacesStarted.Count(match => match.StartedAt >= DateTime.UtcNow.AddDays(-14) && match.StartedAt <= DateTime.UtcNow.AddDays(-7))))
                .ForMember(dto => dto.OnlineRacesThisWeek, cfg => cfg.MapFrom(db => db.OnlineRacesStarted.Count(match => match.StartedAt >= DateTime.UtcNow.AddDays(-7) && match.StartedAt <= DateTime.UtcNow)))

                .ForMember(dto => dto.OnlineWins, cfg => cfg.MapFrom(db => db.OnlineRacesFinished.Count(match => match.IsWinner)))
                .ForMember(dto => dto.OnlineWinsLastWeek, cfg => cfg.MapFrom(db => db.OnlineRacesFinished.Count(match => match.IsWinner && match.FinishedAt >= DateTime.UtcNow.AddDays(-14) && match.FinishedAt <= DateTime.UtcNow.AddDays(-7))))
                .ForMember(dto => dto.OnlineWinsThisWeek, cfg => cfg.MapFrom(db => db.OnlineRacesFinished.Count(match => match.IsWinner && match.FinishedAt >= DateTime.UtcNow.AddDays(-7) && match.FinishedAt <= DateTime.UtcNow)))

                .ForMember(dto => dto.PlayerCreationQuota, cfg => cfg.MapFrom(db => db.Quota))

                .ForMember(dto => dto.Quote, cfg => cfg.MapFrom(db => db.Quote != null ? db.Quote.Trim('\0') : ""))  // TODO: Is this EF translatable? (.Trim())

                .ForMember(dto => dto.UpdatedAt, cfg => cfg.MapFrom(db => db.UpdatedAt.ToString("yyyy-MM-ddThh:mm:sszzz")))
                //MNR
                .ForMember(dto => dto.TotalCharacters, cfg => cfg.MapFrom(db => db.PlayerCreations.Count(match => match.Type == PlayerCreationType.CHARACTER && match.Platform == session.Platform))) // TODO: Does this need the same checks as TotalPlayerCreations?
                .ForMember(dto => dto.TotalKarts, cfg => cfg.MapFrom(db => db.PlayerCreations.Count(match => match.Type == PlayerCreationType.KART && match.Platform == session.Platform))) // TODO: Does this need the same checks as TotalPlayerCreations?
                .ForMember(dto => dto.TotalPlayerCreations, cfg => cfg.MapFrom(db => db.PlayerCreations.Count(match => match.Type != PlayerCreationType.PHOTO && match.Type != PlayerCreationType.DELETED && match.IsMNR && match.Platform == session.Platform)))
                .ForMember(dto => dto.TotalTracks, cfg => cfg.MapFrom(db => db.PlayerCreations.Count(match => match.Type == PlayerCreationType.TRACK && (session.IsMNR ? match.IsMNR && match.Platform == session.Platform : !session.IsMNR))))
                
                .ForMember(dto => dto.SkillLevel, cfg => cfg.MapFrom(db => db.PlayerCreations.Count(match => match.Type == PlayerCreationType.TRACK && (session.IsMNR ? match.IsMNR && match.Platform == session.Platform : !session.IsMNR)))
            #endregion
        }
    }
}
