namespace GameServer.Models.Request
{
    public class ClientPreferences 
    {
        public string domain { get; set; }
        public string language_code { get; set; }
        public string region_code { get; set; }
        public string timezone { get; set; }
    }
}