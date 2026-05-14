using GameServer.Models.GameBrowser;
using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using System;
using System.IO;

namespace GameServer.Models
{
    public interface IUGCStorage : IDisposable
    {
        public void Initialize();
        public void SetAsMigrationSource();
        public void SavePlayerAvatar(int userId, PlayerAvatarType avatarType, Stream avatar, bool isMNR);
        public void SavePlayerAvatar(int userId, PlayerAvatar avatar, bool isMNR) => SavePlayerAvatar(userId, avatar.player_avatar_type, avatar.avatar.OpenReadStream(), isMNR);
        public void SaveGriefReportData(int id, Stream data, Stream preview);
        public void SavePlayerCreationComplaintPreview(int id, Stream preview);
        public void SavePlayerCreation(int id, Stream data, Stream preview);
        public void SavePlayerCreation(int id, Stream data);
        public void SavePlayerPhoto(int id, Stream data);
        public void SaveGhostCarData(GameType gameType, Platform platform, int trackId, int playerId, Stream data);
        public Stream LoadPlayerAvatar(int id, string file, bool isMNR = false);
        public Stream LoadGriefReportData(int id, string file);
        public Stream LoadPlayerCreationComplaintPreview(int id);
        public Stream LoadPlayerCreation(int id, string file);
        public Stream LoadGhostCarData(GameType gameType, Platform platform, int trackId, int playerId);
        public Stream LoadAnnouncementImage(string file);
        public string CalculateMD5(int id, string file);
        public long CalculateSize(int id, string file);
        public void RemovePlayerCreation(int id);
        public void RemoveGhostCarData(GameType gameType, Platform platform, int trackId, int playerId);
    }
}
