namespace GameServer.Models.Config.ServerList
{
    public class Server
    {
        public string Address { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 10501;
        public string ServerPrivateKey { get; set; } = "MIGrAgEAAiEAq0cOe8L1tOpnc7e+ouVD";
    }
}