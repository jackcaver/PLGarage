using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace GameServer.Models.PlayerData
{
    public class Moderator
    {
        [Key]
        public int ID { get; set; }
        public string Username { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        public bool ManageModerators { get; set; }
        public bool BanUsers { get; set; }
        public bool ChangeUserSettings { get; set; }
        public bool ChangeUserQuota { get; set; }
        public bool ChangeCreationStatus { get; set; }
        public bool ManageAnnouncements { get; set; }
        public bool ManageHotlap { get; set; }
        public bool ManageSystemEvents { get; set; }
        public bool ViewGriefReports { get; set; }
        public bool ViewPlayerComplaints { get; set; }
        public bool ViewPlayerCreationComplaints { get; set; }
    }
}
