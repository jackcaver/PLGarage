using Amazon.S3;
using Amazon.S3.Model;
using GameServer.Models;
using GameServer.Models.Config;
using GameServer.Models.Config.Storage;
using GameServer.Models.GameBrowser;
using GameServer.Models.PlayerData;
using GameServer.Models.Request;
using GameServer.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace GameServer.Implementation.Storage
{
    public class S3Storage : IUGCStorage
    {
        private bool IsMigrationSource;
        private S3StorageConfig Config => IsMigrationSource ? ServerConfig.Instance.Storage.MigrateFrom.S3 : ServerConfig.Instance.Storage.S3;
        private AmazonS3Client S3Client;

        private void StoreRescaled(string key, Stream image, int width, int height)
        {
            using var rs = UserGeneratedContentUtils.Resize(image, width, height);
            
            try
            {
                var request = new PutObjectRequest()
                {
                    BucketName = Config.BucketName,
                    Key = key,
                    InputStream = rs
                };
                S3Client.PutObjectAsync(request).Wait();
            }
            catch (AmazonS3Exception) { }
        }

        private Stream GetMemoryStream(Stream source)
        {
            using var reader = new BinaryReader(source);
            return new MemoryStream(reader.ReadBytes((int)source.Length));
        }
        
        public void Initialize()
        {
            if (Config == null)
                throw new InvalidOperationException("S3 storage config is not set");
            
            S3Client = new(Config.AccessKeyId, Config.SecretAccessKey, new AmazonS3Config()
            {
                ServiceURL = Config.ServiceURL
            });
        }

        public void SetAsMigrationSource()
        {
            IsMigrationSource = true;
        }

        public void SavePlayerAvatar(int userId, PlayerAvatarType avatarType, Stream avatar, bool isMNR)
        {
            string path;

            if (isMNR)
                path = "player_avatars/MNR";
            else
                path = "player_avatars";
            
            var key = $"{path}/{userId}/{avatarType.ToString().ToLower()}.png";
            var rsKey = $"{path}/{userId}/{avatarType.ToString().ToLower()}_128x128.png";
            
            try
            {
                var request = new PutObjectRequest()
                {
                    BucketName = Config.BucketName,
                    Key = key,
                    InputStream = avatar
                };
                StoreRescaled(rsKey, avatar, 128, 128);
                S3Client.PutObjectAsync(request).Wait();
            }
            catch (AmazonS3Exception) { }
        }
        
        public void SaveGriefReportData(int id, Stream data, Stream preview)
        {
            var dataKey = $"player_creations/{id}/data.xml";
            var previewKey = $"player_creations/{id}/preview.png";

            try
            {
                var dataRequest = new PutObjectRequest()
                {
                    BucketName = Config.BucketName,
                    Key = dataKey,
                    InputStream = data
                };
                var previewRequest = new PutObjectRequest()
                {
                    BucketName = Config.BucketName,
                    Key = previewKey,
                    InputStream = preview
                };
                S3Client.PutObjectAsync(dataRequest).Wait();
                S3Client.PutObjectAsync(previewRequest).Wait();
            }
            catch (AmazonS3Exception) { }
        }
        
        public void SavePlayerCreationComplaintPreview(int id, Stream preview)
        {
            var key = $"player_creations_complaints/{id}/preview.png";

            try
            {
                var request = new PutObjectRequest()
                {
                    BucketName = Config.BucketName,
                    Key = key,
                    InputStream = preview
                };
                S3Client.PutObjectAsync(request).Wait();
            }
            catch (AmazonS3Exception) { }
        }
        
        public void SavePlayerCreation(int id, Stream data, Stream preview)
        {
            var dataKey = $"player_creations/{id}/data.bin";
            var previewKey = $"player_creations/{id}/preview_image.png";
            var previewRsKey = $"player_creations/{id}/preview_image_128x128.png";
            var previewRs2Key = $"player_creations/{id}/preview_image_64x64.png";
            
            try
            {
                var dataRequest = new PutObjectRequest()
                {
                    BucketName = Config.BucketName,
                    Key = dataKey,
                    InputStream = data
                };
                var previewRequest = new PutObjectRequest()
                {
                    BucketName = Config.BucketName,
                    Key = previewKey,
                    InputStream = preview
                };
                S3Client.PutObjectAsync(dataRequest).Wait();
                StoreRescaled(previewRsKey, preview, 128, 128);
                StoreRescaled(previewRs2Key, preview, 64, 64);
                S3Client.PutObjectAsync(previewRequest).Wait();
            }
            catch (AmazonS3Exception) { }
        }
        
        public void SavePlayerCreation(int id, Stream data)
        {
            var key = $"player_creations/{id}/data.bin";

            try
            {
                var request = new PutObjectRequest()
                {
                    BucketName = Config.BucketName,
                    Key = key,
                    InputStream = data
                };
                S3Client.PutObjectAsync(request).Wait();
            }
            catch (AmazonS3Exception) { }
        }
        
        public void SavePlayerPhoto(int id, Stream data)
        {
            var key = $"player_creations/{id}/data.jpg";

            try
            {
                var request = new PutObjectRequest()
                {
                    BucketName = Config.BucketName,
                    Key = key,
                    InputStream = data
                };
                S3Client.PutObjectAsync(request).Wait();
            }
            catch (AmazonS3Exception) { }
        }
        
        public void SaveGhostCarData(GameType gameType, Platform platform, int trackId, int playerId, Stream data)
        {
            string key;
            if (platform == Platform.PS3)
                key = $"ghost_car_data/{gameType}/{trackId}/{playerId}/data.bin";
            else
                key = $"ghost_car_data/{gameType}/{platform}/{trackId}/{playerId}/data.bin";

            try
            {
                var request = new PutObjectRequest()
                {
                    BucketName = Config.BucketName,
                    Key = key,
                    InputStream = data
                };
                S3Client.PutObjectAsync(request).Wait();
            }
            catch (AmazonS3Exception) { }
        }
        
        public Stream LoadPlayerAvatar(int id, string file, bool isMNR = false)
        {
            string path;

            if (isMNR)
                path = "player_avatars/MNR";
            else
                path = "player_avatars";
            
            var key = $"{path}/{id}/{file}";

            try
            {
                var response = S3Client.GetObjectAsync(Config.BucketName, key).GetAwaiter().GetResult();

                return GetMemoryStream(response.ResponseStream);
            }
            catch (AmazonS3Exception e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound && file.Contains("_128x128"))
                {
                    try
                    {
                        var response = S3Client.GetObjectAsync(Config.BucketName, key.Replace("_128x128", "")).GetAwaiter().GetResult();

                        StoreRescaled(key, response.ResponseStream, 128, 128);

                        return LoadPlayerCreation(id, file);
                    }
                    catch (AmazonS3Exception)
                    {
                        return null;
                    }
                }
                else
                    return null;
            }
        }
        
        public Stream LoadGriefReportData(int id, string file)
        {
            var key = $"grief_reports/{id}/{file}";
            
            try
            {
                var response = S3Client.GetObjectAsync(Config.BucketName, key).GetAwaiter().GetResult();

                return response.ResponseStream;
            }
            catch (AmazonS3Exception)
            {
                return null;
            }
        }
        
        public Stream LoadPlayerCreationComplaintPreview(int id)
        {
            var key = $"player_creations_complaints/{id}/preview.png";
            
            try
            {
                var response = S3Client.GetObjectAsync(Config.BucketName, key).GetAwaiter().GetResult();

                return response.ResponseStream;
            }
            catch (AmazonS3Exception)
            {
                return null;
            }
        }
        
        public Stream LoadPlayerCreation(int id, string file)
        {
            var key = $"player_creations/{id}/{file}";
            
            try
            {
                var response = S3Client.GetObjectAsync(Config.BucketName, key).GetAwaiter().GetResult();

                return GetMemoryStream(response.ResponseStream);
            }
            catch (AmazonS3Exception e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound && file.Contains("_128x128"))
                {
                    try
                    {
                        var response = S3Client.GetObjectAsync(Config.BucketName, key.Replace("_128x128", "")).GetAwaiter().GetResult();

                        StoreRescaled(key, response.ResponseStream, 128, 128);

                        return LoadPlayerCreation(id, file);
                    }
                    catch (AmazonS3Exception)
                    {
                        return null;
                    }
                }
                else if (e.StatusCode == HttpStatusCode.NotFound && file.Contains("_64x64"))
                {
                    try
                    {
                        var response = S3Client.GetObjectAsync(Config.BucketName, key.Replace("_64x64", "")).GetAwaiter().GetResult();

                        StoreRescaled(key, response.ResponseStream, 64, 64);

                        return LoadPlayerCreation(id, file);
                    }
                    catch (AmazonS3Exception)
                    {
                        return null;
                    }
                }
                else
                    return null;
            }
        }
        
        public Stream LoadGhostCarData(GameType gameType, Platform platform, int trackId, int playerId)
        {
            string key;
            if (platform == Platform.PS3)
                key = $"ghost_car_data/{gameType}/{trackId}/{playerId}/data.bin";
            else
                key = $"ghost_car_data/{gameType}/{platform}/{trackId}/{playerId}/data.bin";
            
            try
            {
                var response = S3Client.GetObjectAsync(Config.BucketName, key).GetAwaiter().GetResult();

                return GetMemoryStream(response.ResponseStream);
            }
            catch (AmazonS3Exception)
            {
                return null;
            }
        }

        public Stream LoadAnnouncementImage(string file)
        {
            var key = $"announcements/{file}";
            
            try
            {
                var response = S3Client.GetObjectAsync(Config.BucketName, key).GetAwaiter().GetResult();

                return response.ResponseStream;
            }
            catch (AmazonS3Exception)
            {
                return null;
            }
        }

        public string CalculateMD5(int id, string file)
        {
            var key = $"player_creations/{id}/{file}";
            
            try
            {
                var response = S3Client.GetObjectAsync(Config.BucketName, key).GetAwaiter().GetResult();
                using var stream = GetMemoryStream(response.ResponseStream);
                
                return UserGeneratedContentUtils.CalculateMD5(stream) ?? "";
            }
            catch (AmazonS3Exception)
            {
                return "";
            }
        }

        public long CalculateSize(int id, string file)
        {
            var key = $"player_creations/{id}/{file}";
            
            try
            {
                var response = S3Client.GetObjectAsync(Config.BucketName, key).GetAwaiter().GetResult();

                return response.ContentLength;
            }
            catch (AmazonS3Exception)
            {
                return 0;
            }
        }

        public void RemovePlayerCreation(int id)
        {
            List<string> keys = [
                $"player_creations/{id}/data.bin",
                $"player_creations/{id}/data.jpg",
                $"player_creations/{id}/preview_image.png",
                $"player_creations/{id}/preview_image_128x128.png",
                $"player_creations/{id}/preview_image_64x64.png",
            ];

            foreach (var key in keys)
            {
                try
                {
                    S3Client.DeleteObjectAsync(Config.BucketName, key).Wait();
                }
                catch (AmazonS3Exception) { }
            }
        }
        
        public void RemoveGhostCarData(GameType gameType, Platform platform, int trackId, int playerId)
        {
            string key;
            if (platform == Platform.PS3)
                key = $"ghost_car_data/{gameType}/{trackId}/{playerId}/data.bin";
            else
                key = $"ghost_car_data/{gameType}/{platform}/{trackId}/{playerId}/data.bin";
            
            try
            {
                S3Client.DeleteObjectAsync(Config.BucketName, key).Wait();
            }
            catch (AmazonS3Exception) { }
        }
        
        public void Dispose()
        {
            S3Client.Dispose();
        }
    }
}
