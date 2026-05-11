using GameServer.Models.Moderation;
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
        public bool ManageTeamPicks { get; set; }
        public bool ViewGriefReports { get; set; }
        public bool ViewPlayerComplaints { get; set; }
        public bool ViewPlayerCreationComplaints { get; set; }

        public bool HasPermissions(ModeratorPermissions permissions)
        {
            return (!permissions.BanUsers || BanUsers)
                   && (!permissions.ChangeCreationStatus || ChangeCreationStatus)
                   && (!permissions.ChangeUserSettings || ChangeUserSettings)
                   && (!permissions.ChangeUserQuota || ChangeUserQuota)
                   && (!permissions.ViewGriefReports || ViewGriefReports)
                   && (!permissions.ViewPlayerComplaints || ViewPlayerComplaints)
                   && (!permissions.ViewPlayerCreationComplaints || ViewPlayerCreationComplaints)
                   && (!permissions.ManageHotlap || ManageHotlap)
                   && (!permissions.RemoveScores || RemoveScores)
                   && (!permissions.ManageAnnouncements || ManageAnnouncements)
                   && (!permissions.ManageWhitelist || ManageWhitelist)
                   && (!permissions.ManageTeamPicks || ManageTeamPicks)
                   && (!permissions.RemovePlayerCreations || RemovePlayerCreations)
                   && (!permissions.RemovePlayerCreationComments || RemovePlayerCreationComments)
                   && (!permissions.RemoveProfileComments || RemoveProfileComments)
                   && (!permissions.ResetCreationStats || ResetCreationStats)
                   && (!permissions.ResetUserStats || ResetUserStats)
                   && (!permissions.RemoveUsers || RemoveUsers)
                   && (!permissions.ManageSystemEvents || ManageSystemEvents)
                   && (!permissions.ManageUserSessions || ManageUserSessions);
        }

        public ModeratorPermissions GetPermissions()
        {
            return new ModeratorPermissions
            {
                BanUsers = BanUsers,
                ChangeCreationStatus = ChangeCreationStatus,
                ChangeUserSettings = ChangeUserSettings,
                ChangeUserQuota = ChangeUserQuota,
                ManageModerators = ManageModerators,
                ViewGriefReports = ViewGriefReports,
                ViewPlayerComplaints = ViewPlayerComplaints,
                ViewPlayerCreationComplaints = ViewPlayerCreationComplaints,
                ManageAnnouncements = ManageAnnouncements,
                ManageHotlap = ManageHotlap,
                RemoveScores = RemoveScores,
                ManageSystemEvents = ManageSystemEvents,
                ManageWhitelist = ManageWhitelist,
                ManageTeamPicks = ManageTeamPicks,
                RemovePlayerCreations = RemovePlayerCreations,
                RemovePlayerCreationComments = RemovePlayerCreationComments,
                RemoveProfileComments = RemoveProfileComments,
                ResetCreationStats = ResetCreationStats,
                ResetUserStats = ResetUserStats,
                RemoveUsers = RemoveUsers,
                ManageUserSessions = ManageUserSessions
            };
        }
        
        public bool CanDelete(Moderator moderator)
        {
            return HasPermissions(moderator.GetPermissions());
        }
    }
}
