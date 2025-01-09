using System;
using System.Globalization;
using System.Linq;
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

            #endregion

            #region PlayerCreations

            CreateMap<PlayerCreationData, PlayerCreation>();

            #endregion
        }
    }
}
