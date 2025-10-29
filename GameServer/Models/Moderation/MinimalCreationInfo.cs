using GameServer.Models.PlayerData.PlayerCreations;

namespace GameServer.Models.Moderation
{
    public class MinimalCreationInfo
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public PlayerCreationType Type { get; set; }
        public int PlayerID { get; set; }
        public int ParentPlayerID { get; set; }
        public int OriginalPlayerID { get; set; }
        public int ParentCreationID { get; set; }
        public ModerationStatus ModerationStatus { get; set; }
        public bool IsMNR { get; set; }
    }
}
