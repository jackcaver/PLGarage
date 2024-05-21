using GameServer.Models.PlayerData;
using System.Collections.Generic;

namespace GameServer.Models.GameBrowser
{
    public class GameData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int HostPlayerId { get; set; }
        public string HostPlayerIP { get; set; }
        public int MaxPlayers { get; set; }
        public int MinPlayers { get; set; }
        public GameType Type { get; set; }
        public GameState State { get; set; }
        public Platform Platform { get; set; }
        public string SpeedClass { get; set; }
        public bool IsRanked { get; set; }
        public int Track { get; set; }
        public string TrackGroup { get; set; }
        public string Privacy { get; set; }
        public string Password { get; set; }
        public int LobbyChannelId { get; set; }
        public int NumberLaps { get; set; }
        public List<GamePlayerData> Players { get; set; }
    }
}
