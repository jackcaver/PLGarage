namespace GameServer.Models.Moderation
{
    public class ModeratorPermissions
    {
        public bool ManageModerators { get; set; }
        public bool BanUsers { get; set; }
        public bool ChangeUserSettings { get; set; }
        public bool ChangeCreationStatus { get; set; }
        public bool ViewGriefReports { get; set; }
        public bool ViewPlayerComplaints { get; set; }
        public bool ViewPlayerCreationComplaints { get; set; }
    }
}
