using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;
using Microsoft.AspNetCore.Http;

namespace GameServer.Models.Request
{
    public class PlayerCreation
    {
        public string name { get; set; }
        public PlayerCreationType player_creation_type { get; set; }
        public IFormFile data { get; set; }
        public Platform platform { get; set; }
        public string description { get; set; }
        public string tags { get; set; }
        public string auto_tags { get; set; }
        public string user_tags { get; set; }
        public IFormFile preview { get; set; }
        public bool requires_dlc { get; set; }
        public string dlc_keys { get; set; }
        public bool is_remixable { get; set; }
        public float longest_hang_time { get; set; }
        public float longest_drift { get; set; }
        public int races_started { get; set; }
        public int races_won { get; set; }
        public int votes { get; set; }
        public int races_started_this_week { get; set; }
        public int races_started_this_month { get; set; }
        public int races_finished { get; set; }
        public int track_theme { get; set; }
        public bool auto_reset { get; set; }
        public bool ai { get; set; }
        public int num_laps { get; set; }
        public PlayerCreationSpeed speed { get; set; }
        public RaceType race_type { get; set; }
        public string weapon_set { get; set; }
        public PlayerCreationDifficulty difficulty { get; set; }
        public int battle_kill_count { get; set; }
        public int battle_time_limit { get; set; }
        public bool battle_friendly_fire { get; set; }
        public int num_racers { get; set; }
        public int max_humans { get; set; }
        public int unique_racer_count { get; set; }
        public string associated_item_ids { get; set; }
        public bool is_team_pick { get; set; }
        public int level_mode { get; set; }
        public int scoreboard_mode { get; set; }
        public string associated_usernames { get; set; }
        public string associated_coordinates { get; set; }
        public int track_id { get; set; }
        //MNR
        public int parent_creation_id { get; set; }
        public int parent_player_id { get; set; }
        public int original_player_id { get; set; }
        public float best_lap_time { get; set; }
    }
}
