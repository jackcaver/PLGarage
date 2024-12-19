using Microsoft.AspNetCore.Http;

namespace GameServer.Models.Request
{
    public class PlayerAvatar
    {
        public IFormFile avatar { get; set; }
        public PlayerAvatarType player_avatar_type { get; set; }
    }
}
