namespace GameServer.Models.Config.Storage
{
    public class S3StorageConfig
    {
        public string BucketName { get; set; }
        public string ServiceURL { get; set; }
        public string AccessKeyId { get; set; }
        public string SecretAccessKey { get; set; }
    }
}
