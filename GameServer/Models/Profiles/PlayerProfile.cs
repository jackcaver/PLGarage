using System;
using System.Globalization;
using System.Linq;
using System.Xml;
using AutoMapper;
using GameServer.Models.Common;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Request;
using GameServer.Models.Response;

namespace GameServer.Models.Profiles
{
    public class PlayerProfile : Profile
    {
        public PlayerProfile()
        {
            #region Variables

            User requestedBy = null;

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
        }
    }
}
