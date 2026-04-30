using System;

namespace GameServer.Models
{
    public record SessionInfo(int UserId, Guid SessionId);
}
