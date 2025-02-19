using System;
using System.Globalization;
using System.Linq;
using System.Xml;
using AutoMapper;
using GameServer.Models.Common;
using GameServer.Models.GameBrowser;
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

            #endregion

            #region Games

            CreateMap<GameData, GameListGame>()
                .ForMember(dto => dto.HostPlayerIpAddress, cfg => cfg.MapFrom(db => db.HostPlayerIP))
                .ForMember(dto => dto.CurPlayers, cfg => cfg.MapFrom(db => db.Players.Count))
                .ForMember(dto => dto.GameType, cfg => cfg.MapFrom(db => db.Type.ToString()))
                .ForMember(dto => dto.GameStateId, cfg => cfg.MapFrom(db => db.State));

            #endregion
        }
    }
}
