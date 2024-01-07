using Microsoft.AspNetCore.Http;

namespace GameServer.Models.Request
{
    public class BadRectangleData 
    {
        public string top { get; set; }
        public string bottom { get; set; }
    }

    public class GriefReport 
    {
        public string context { get; set; }
        public string reason { get; set; }
        public string comments { get; set; }
        public IFormFile preview { get; set; }
        public BadRectangleData bad_rect_data { get; set; }
        public IFormFile data { get; set; }
    }
}