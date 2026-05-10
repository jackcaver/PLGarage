namespace GameServer.Models.Moderation
{
    public class ModeratorPermissions
    {
        public bool ManageModerators { get; set; }
        public bool BanUsers { get; set; }
        public bool ChangeUserSettings { get; set; }
        public bool ChangeUserQuota { get; set; }
        public bool ResetUserStats { get; set; }
        public bool ManageUserSessions { get; set; }
        public bool RemoveUsers { get; set; }
        public bool ChangeCreationStatus { get; set; }
        public bool ResetCreationStats { get; set; }
        public bool RemovePlayerCreations { get; set; }
        public bool RemovePlayerCreationComments { get; set; }
        public bool RemoveProfileComments { get; set; }
        public bool ManageAnnouncements { get; set; }
        public bool ManageHotlap { get; set; }
        public bool RemoveScores { get; set; }
        public bool ManageSystemEvents { get; set; }
        public bool ManageWhitelist { get; set; }
        public bool ViewGriefReports { get; set; }
        public bool ViewPlayerComplaints { get; set; }
        public bool ViewPlayerCreationComplaints { get; set; }
    }
}
