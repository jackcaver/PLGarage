using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Utils;
using System;
using System.Linq;
using GameServer.Implementation.Common;

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
        public int TotalTracks => this.database.PlayerCreations.Count(match => match.PlayerId == this.UserId && match.Type == PlayerCreationType.TRACK);
        public int Rank { get; set; }
        public int Points { get; set; }
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

        public bool IsHeartedByMe(int id) 
        {
            var entry = this.database.HeartedProfiles.FirstOrDefault(match => match.HeartedUserId == this.UserId && match.UserId == id);
            return entry != null;
        }
    }
}
