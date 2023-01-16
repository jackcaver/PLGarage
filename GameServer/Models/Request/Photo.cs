using GameServer.Models.PlayerData;
using Microsoft.AspNetCore.Http;

namespace GameServer.Models.Request
{
    public class Photo
    {
        public string name { get; set; }
        public string description { get; set; }
        public Platform platform { get; set; }
        public string associated_usernames { get; set; }
        public string associated_coordinates { get; set; }
        public int track_id { get; set; }
        //public IFormFile data { get; set; }
    }
}
