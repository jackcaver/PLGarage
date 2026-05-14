using GameServer.Models;
using GameServer.Models.Config;
using GameServer.Models.Config.Storage;
using GameServer.Models.GameBrowser;
using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using GameServer.Utils;
using System;
using System.IO;

namespace GameServer.Implementation.Storage
{
    public class LocalStorage : IUGCStorage
    {
        private bool IsMigrationSource;
        private LocalStorageConfig Config => IsMigrationSource ? ServerConfig.Instance.Storage.MigrateFrom.Local : ServerConfig.Instance.Storage.Local;

        public void Initialize()
        {
            if (Config == null)
                throw new InvalidOperationException("Local storage config is not set");
        }

        public void SetAsMigrationSource()
        {
            IsMigrationSource = true;
        }

        public void SavePlayerAvatar(int userId, PlayerAvatarType avatarType, Stream avatar, bool isMNR)
        {
            var dir = $"{Config.StoragePath}/PlayerAvatars/{userId}/";
            var mnrDir = $"{Config.StoragePath}/PlayerAvatars/{userId}/MNR/";
            
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!Directory.Exists(mnrDir) && isMNR)
                Directory.CreateDirectory(mnrDir);

            if (!UserGeneratedContentUtils.CheckImage(avatar, 256, 256))
                return;

            FileStream file;

            if (!isMNR)
                file = File.Create($"{dir}{avatarType.ToString().ToLower()}.png");
            else
                file = File.Create($"{mnrDir}{avatarType.ToString().ToLower()}.png");
            
            avatar.CopyTo(file);
            file.Close();

            if (!isMNR)
                file = File.Create($"{dir}{avatarType.ToString().ToLower()}_128x128.png");
            else
                file = File.Create($"{mnrDir}{avatarType.ToString().ToLower()}_128x128.png");
            using (var rs = UserGeneratedContentUtils.Resize(avatar, 128, 128))
                rs.CopyTo(file);
            file.Close();

            avatar.Close();
        }
        
        public void SaveGriefReportData(int id, Stream data, Stream preview)
        {
            var dir = $"{Config.StoragePath}/GriefReports/{id}/";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            
            FileStream previewFile = File.Create($"{dir}preview.png");
            preview.CopyTo(previewFile);
            previewFile.Close();
            FileStream dataFile = File.Create($"{dir}data.xml");
            data.CopyTo(dataFile);
            dataFile.Close();
            data.Close();
            preview.Close();
        }
        
        public void SavePlayerCreationComplaintPreview(int id, Stream preview)
        {
            var dir = $"{Config.StoragePath}/PlayerCreationComplaints/{id}/";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            
            FileStream previewFile = File.Create($"{dir}preview.png");
            preview.CopyTo(previewFile);
            previewFile.Close();
            preview.Close();
        }
        
        public void SavePlayerCreation(int id, Stream data, Stream preview)
        {
            var dir = $"{Config.StoragePath}/PlayerCreations/{id}/";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using (var dataFile = File.Create($"{dir}data.bin"))
                data.CopyTo(dataFile);
            data.Close();

            using (var previewFile = File.Create($"{dir}preview_image.png"))
                preview.CopyTo(previewFile);
            using (var previewFile128 = File.Create($"{dir}preview_image_128x128.png"))
            {
                using (var rs = UserGeneratedContentUtils.Resize(preview, 128, 128))
                    rs.CopyTo(previewFile128);
            }
            using (var previewFile64 = File.Create($"{dir}preview_image_64x64.png"))
            {
                using (var rs = UserGeneratedContentUtils.Resize(preview, 64, 64))
                    rs.CopyTo(previewFile64);
            }
            preview.Close();
        }
        
        public void SavePlayerCreation(int id, Stream data)
        {
            var dir = $"{Config.StoragePath}/PlayerCreations/{id}/";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            
            FileStream dataFile = File.Create($"{dir}data.bin");
            data.CopyTo(dataFile);
            dataFile.Close();
            data.Close();
        }
        
        public void SavePlayerPhoto(int id, Stream data)
        {
            var dir = $"{Config.StoragePath}/PlayerCreations/{id}/";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            
            FileStream dataFile = File.Create($"{dir}data.jpg");
            data.CopyTo(dataFile);
            dataFile.Close();
            data.Close();
        }
        
        public void SaveGhostCarData(GameType gameType, Platform platform, int trackId, int playerId, Stream data)
        {
            var dir = $"{Config.StoragePath}/GhostCarData/{gameType}/{platform}/{trackId}/{playerId}/";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            
            FileStream dataFile = File.Create($"{dir}data.bin");
            data.CopyTo(dataFile);
            dataFile.Close();
            data.Close();
        }
        
        public Stream LoadPlayerAvatar(int id, string file, bool isMNR = false)
        {
            var dir = $"{Config.StoragePath}/PlayerAvatars/{id}/";
            var mnrDir = $"{Config.StoragePath}/PlayerAvatars/{id}/MNR/";
            if (File.Exists($"{dir}{file}") && !isMNR)
                return File.OpenRead($"{dir}{file}");
            else if (file.Contains("_128x128") && !File.Exists($"{dir}{file}")
                                               && File.Exists($"{dir}{file.Replace("_128x128", "")}") && !isMNR)
            {
                var newFile = File.Create($"{dir}{file}");
                var original = File.OpenRead($"{dir}{file.Replace("_128x128", "")}");
                var rs = UserGeneratedContentUtils.Resize(original, 128, 128);
                rs.CopyTo(newFile);
                rs.Position = 0;
                newFile.Close();
                original.Close();
                return rs;
            }
            else if (File.Exists($"{mnrDir}{file}") && isMNR)
                return File.OpenRead($"{mnrDir}{file}");
            else if (file.Contains("_128x128") && !File.Exists($"{mnrDir}{file}")
                                               && File.Exists($"{mnrDir}{file.Replace("_128x128", "")}") && isMNR)
            {
                var newFile = File.Create($"{mnrDir}{file}");
                var original = File.OpenRead($"{mnrDir}{file.Replace("_128x128", "")}");
                var rs = UserGeneratedContentUtils.Resize(original, 128, 128);
                rs.CopyTo(newFile);
                rs.Position = 0;
                newFile.Close();
                original.Close();
                return rs;
            }
            else
                return null;
        }
        
        public Stream LoadGriefReportData(int id, string file)
        {
            var dir = $"{Config.StoragePath}/GriefReports/{id}/";
            if (File.Exists($"{dir}{file}"))
                return File.OpenRead($"{dir}{file}");
            else
                return null;
        }
        
        public Stream LoadPlayerCreationComplaintPreview(int id)
        {
            var file = $"{Config.StoragePath}/PlayerCreationComplaints/{id}/preview.png";
            if (File.Exists(file))
                return File.OpenRead(file);
            else
                return null;
        }
        
        public Stream LoadPlayerCreation(int id, string file)
        {
            var dir = $"{Config.StoragePath}/PlayerCreations/{id}/";
            if (File.Exists($"{dir}{file}"))
                return File.OpenRead($"{dir}{file}");
            else if (file.Contains("_128x128") && !File.Exists($"{dir}{file}")
                                               && File.Exists($"{dir}{file.Replace("_128x128", "")}"))
            {
                var newFile = File.Create($"{dir}{file}");
                var original = File.OpenRead($"{dir}{file.Replace("_128x128", "")}");
                var rs = UserGeneratedContentUtils.Resize(original, 128, 128);
                rs.CopyTo(newFile);
                rs.Position = 0;
                newFile.Close();
                original.Close();
                return rs;
            }
            else if (file.Contains("_64x64") && !File.Exists($"{dir}{file}")
                                             && File.Exists($"{dir}{file.Replace("_64x64", "")}"))
            {
                var newFile = File.Create($"{dir}{file}");
                var original = File.OpenRead($"{dir}{file.Replace("_64x64", "")}");
                var rs = UserGeneratedContentUtils.Resize(original, 64, 64);
                rs.CopyTo(newFile);
                rs.Position = 0;
                newFile.Close();
                original.Close();
                return rs;
            }
            else
                return null;
        }
        
        public Stream LoadGhostCarData(GameType gameType, Platform platform, int trackId, int playerId)
        {
            var file = $"{Config.StoragePath}/GhostCarData/{gameType}/{platform}/{trackId}/{playerId}/data.bin";
            if (File.Exists(file))
                return File.OpenRead(file);
            else
                return null;
        }

        public Stream LoadAnnouncementImage(string file)
        {
            var dir = $"{Config.StoragePath}/Announcements/";
            if (File.Exists($"{dir}{file}"))
                return File.OpenRead($"{dir}{file}");
            else
                return null;
        }

        public string CalculateMD5(int id, string file)
        {
            using var data = LoadPlayerCreation(id, file);
            if (data != null)
                return UserGeneratedContentUtils.CalculateMD5(data);
            else
                return "";
        }

        public long CalculateSize(int id, string file)
        {
            using var data = LoadPlayerCreation(id, file);
            if (data != null)
                return data.Length;
            else
                return 0;
        }

        public void RemovePlayerCreation(int id)
        {
            var dir = $"{Config.StoragePath}/PlayerCreations/{id}/";
            if (Directory.Exists(dir))
                Directory.Delete($"{Config.StoragePath}/PlayerCreations/{id}/", true);
        }

        public void RemoveGhostCarData(GameType gameType, Platform platform, int trackId, int playerId)
        {
            string directory = $"{Config.StoragePath}/GhostCarData/{gameType}/{platform}/{trackId}/{playerId}/";
            
            if (Directory.Exists(directory))
                Directory.Delete(directory, true);
        }
        
        public void Dispose()
        {
            
        }
    }
}
