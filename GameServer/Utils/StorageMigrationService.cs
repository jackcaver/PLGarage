using GameServer.Models;
using GameServer.Models.Config;
using GameServer.Models.GameBrowser;
using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer.Utils
{
    public class StorageMigrationService(IUGCStorage storage, ILogger<StorageMigrationService> logger) : IHostedService
    {
        private static MigratableStorageConfig Config => ServerConfig.Instance.Storage;

        private void Migrate()
        {
            IUGCStorage oldStorage = UserGeneratedContentUtils.GetStorage(Config.MigrateFrom.Type);
            
            oldStorage.SetAsMigrationSource();
            oldStorage.Initialize();

            Database database = new();
            
            logger.LogInformation($"migrating storage from {Config.MigrateFrom.Type} to {Config.Type}");
            
            var users = database.Users
                .AsNoTracking()
                .Select(u => new { u.UserId, u.PlayedMNR })
                .ToList();
            
            logger.LogInformation($"migrating avatars for {users.Count} users");
            
            foreach (var user in users)
            {
                foreach (var avatarType in Enum.GetValues<PlayerAvatarType>())
                {
                    using var avatar = oldStorage.LoadPlayerAvatar(user.UserId, $"{avatarType.ToString().ToLower()}.png");
                    if (avatar != null)
                        storage.SavePlayerAvatar(user.UserId, avatarType, avatar, false);
                }
                
                if (!user.PlayedMNR)
                    continue;
                
                foreach (var avatarType in Enum.GetValues<PlayerAvatarType>())
                {
                    using var avatar = oldStorage.LoadPlayerAvatar(user.UserId, $"{avatarType.ToString().ToLower()}.png", true);
                    if (avatar != null)
                        storage.SavePlayerAvatar(user.UserId, avatarType, avatar, true);
                }
            }

            var creations = database.PlayerCreations
                .AsNoTracking()
                .Where(match => match.Type != PlayerCreationType.DELETED && match.Type != PlayerCreationType.STORY)
                .Select(c => new { c.PlayerCreationId, c.Type, c.HasPreview })
                .ToList();

            logger.LogInformation($"migrating data for {creations.Count} creations");
            
            foreach (var creation in creations)
            {
                if (creation.Type == PlayerCreationType.PHOTO)
                {
                    using var data = oldStorage.LoadPlayerCreation(creation.PlayerCreationId, "data.jpg");
                    if (data != null)
                        storage.SavePlayerPhoto(creation.PlayerCreationId, data);
                }
                else if (creation.Type != PlayerCreationType.PLANET && creation.HasPreview)
                {
                    using var data = oldStorage.LoadPlayerCreation(creation.PlayerCreationId, "data.bin");
                    using var preview = oldStorage.LoadPlayerCreation(creation.PlayerCreationId, "preview_image.png");
                    if (data != null && preview != null)
                        storage.SavePlayerCreation(creation.PlayerCreationId, data, preview);
                }
                else
                {
                    using var data = oldStorage.LoadPlayerCreation(creation.PlayerCreationId, "data.bin");
                    if (data != null)
                        storage.SavePlayerCreation(creation.PlayerCreationId, data);
                }
            }
            
            var scores = database.Scores
                .AsNoTracking()
                .Where(match => match.IsMNR)
                .Select(s => new { s.SubGroupId, s.Platform, s.SubKeyId, s.PlayerId })
                .ToList();
            
            logger.LogInformation($"migrating ghost data for {scores.Count} scores");

            foreach (var score in scores)
            {
                var gameType = (GameType)score.SubGroupId + 10;
                using var data = oldStorage.LoadGhostCarData(gameType, score.Platform, score.SubKeyId, score.PlayerId);
                if (data != null)
                    storage.SaveGhostCarData(gameType, score.Platform, score.SubKeyId, score.PlayerId, data);
            }
            
            var griefReports = database.GriefReports
                .AsNoTracking()
                .Select(g => g.Id)
                .ToList();
            
            logger.LogInformation($"migrating data for {griefReports.Count} grief reports");

            foreach (var griefReport in griefReports)
            {
                using var data = oldStorage.LoadGriefReportData(griefReport, "data.bin");
                using var preview = oldStorage.LoadPlayerCreation(griefReport, "preview_image.png");
                if (data != null && preview != null)
                    storage.SaveGriefReportData(griefReport, data, preview);
            }
            
            var playerCreationComplaints = database.PlayerCreationComplaints
                .AsNoTracking()
                .Select(p => p.Id)
                .ToList();

            logger.LogInformation($"migrating previews for {playerCreationComplaints.Count} player creation complaints");
            
            foreach (var playerCreationComplaint in playerCreationComplaints)
            {
                using var preview = oldStorage.LoadPlayerCreationComplaintPreview(playerCreationComplaint);
                if (preview != null)
                    storage.SavePlayerCreationComplaintPreview(playerCreationComplaint, preview);
            }
            
            logger.LogInformation($"storage migration finished");
            
            oldStorage.Dispose();
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (!Config.Migrate || Config.MigrateFrom == null)
                return Task.CompletedTask;

            Migrate();
            
            return Task.CompletedTask;
        }
        
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
