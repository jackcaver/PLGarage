using System;

namespace GameServer.Models.PlayerData
{
    public class ModerationSessionInfo
    {
        public int ModeratorID { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
