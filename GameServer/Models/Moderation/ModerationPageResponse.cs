using System.Collections.Generic;

namespace GameServer.Models.Moderation
{
    public class ModerationPageResponse<T> where T : class
    {
        public int Total { get; set; }
        public List<T> Page { get; set; }
    }
}
