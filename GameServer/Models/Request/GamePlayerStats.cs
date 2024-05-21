using GameServer.Models.PlayerData;
using Microsoft.AspNetCore.Http;

namespace GameServer.Models.Request
{
    public class GamePlayerStats 
    {
        public bool is_complete { get; set; }
        public int stat_1 { get; set; }
        public int stat_2 { get; set; }
        public float score { get; set; }
        public int is_winner { get; set; }
        public int finish_place { get; set; }
        public float finish_time { get; set; }
        public int laps_completed { get; set; }
        public float points { get; set; }
        public float volatility { get; set; }
        public float deviation { get; set; }
        public int playgroup_size { get; set; }
        public int num_kills { get; set; }
        //MNR
        public int track_idx { get; set; }
        public int kart_idx { get; set; }
        public int character_idx { get; set; }
        public float best_lap_time { get; set; }
        public IFormFile ghost_car_data { get; set; }
        public int music_idx { get; set; }
        public int bank { get; set; }
        public float longest_drift { get; set; }
        public float longest_hang_time { get; set; }
        //MNR: Road Trip
        public float latitude { get; set; }
        public float longitude { get; set; }
        public string location_tag { get; set; }
        public Platform track_platform { get; set; }
        //MNR PSP
        public float percent_complete { get; set; }
    }
}