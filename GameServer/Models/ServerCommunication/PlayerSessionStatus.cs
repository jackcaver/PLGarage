using System;

namespace GameServer.Models.ServerCommunication
{
    public class PlayerSessionStatus
    {
        public Guid SessionID { get; set; }
        public bool IsAuthorized { get; set; }
    }
}
