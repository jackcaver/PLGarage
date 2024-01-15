using Newtonsoft.Json;
using System;
using System.Net.WebSockets;

namespace GameServer.Models.ServerCommunication
{
    public class ServerInfo
    {
        [JsonIgnore]
        public Guid ServerId { get; set; }
        
        [JsonIgnore]
        public WebSocket Socket { get; set; }

        public ServerType Type { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public string ServerPrivateKey { get; set; }
        [JsonIgnore]
        public int PlayerCount { get; set; }
    }
}
