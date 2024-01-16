using GameServer.Models.Config;
using GameServer.Models.ServerCommunication;
using GameServer.Utils;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer.Implementation.Common
{
    public class ServerCommunication
    {
        private static List<ServerInfo> Servers = new List<ServerInfo>();

        public static async Task HandleConnection(Database database, WebSocket webSocket, Guid ServerID)
        {
            var receiveResult = new WebSocketReceiveResult(0, WebSocketMessageType.Text, false);

            while (!receiveResult.CloseStatus.HasValue && webSocket.State == WebSocketState.Open)
            {
                var buffer = new byte[4096];
                try
                {
                    receiveResult = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                }
                catch (Exception e)
                {
                    Log.Debug($"There was an error receiving message: {e}");
                }

                string message = Encoding.UTF8.GetString(buffer).Trim('\0');

                if (!string.IsNullOrEmpty(ServerConfig.Instance.ServerCommunicationKey))
                    message = Decrypt(message);

                try
                {
                    var clientMessage = JsonConvert.DeserializeObject<Message>(message);
                    if (clientMessage != null)
                    {
                        clientMessage.From = ServerID.ToString();
                        ProcessMessage(database, webSocket, clientMessage);
                    }
                }
                catch (Exception e)
                {
                    Log.Debug($"Failed to process message: {e}");
                }
            }

            Servers.RemoveAll(match => match.ServerId == ServerID);

            try
            {
                await webSocket.CloseAsync(receiveResult.CloseStatus != null ? receiveResult.CloseStatus.Value : WebSocketCloseStatus.NormalClosure,
                    receiveResult.CloseStatusDescription, CancellationToken.None);
            }
            catch (Exception e)
            {
                Log.Debug($"There was an error while closing connection: {e}");
            }
        }

        private static void ProcessMessage(Database database, WebSocket webSocket, Message message)
        {
            var Response = new Message
            {
                From = "API",
                To = message.From,
                Type = $"{message.Type}Error"
            };

            if (message.To == "API")
            {
                var Server = Servers.FirstOrDefault(match => match.ServerId.ToString() == message.From);
                switch (message.Type) 
                {
                    case "ServerInfo":
                        Response.Content = "Cannot parse server info";
                        var info = JsonConvert.DeserializeObject<ServerInfo>(message.Content);
                        if (info != null)
                        {
                            info.ServerId = Guid.Parse(message.From);
                            info.Socket = webSocket;
                            Servers.Add(info);
                        }
                        else Send(webSocket, JsonConvert.SerializeObject(Response)).Wait();
                        break;

                    case "PlayerCountUpdate":
                        if (Server == null)
                        {
                            Response.Content = $"Unknown sender {message.From}";
                            Send(webSocket, JsonConvert.SerializeObject(Response)).Wait();
                            break;
                        }
                        int playerCount = 0;
                        if (int.TryParse(message.Content, out playerCount))
                            Server.PlayerCount = playerCount;
                        else
                        {
                            Response.Content = "Cannot parse player count";
                            Send(webSocket, JsonConvert.SerializeObject(Response)).Wait();
                        }
                        break;

                    default:
                        Response.Content = $"Unknown message type {message.Type}";
                        Send(webSocket, JsonConvert.SerializeObject(Response)).Wait();
                        break;
                }
            }
            else if (message.To == "Broadcast")
            {
                foreach (var server in Servers)
                {
                    Send(server.Socket, JsonConvert.SerializeObject(message)).Wait();
                }
            }
            else if (Servers.FirstOrDefault(match => match.ServerId.ToString() == message.From) != null)
            {
                Response.Content = $"Cannot find receiver {message.To}";
                var receiver = Servers.FirstOrDefault(match => match.ServerId == Guid.Parse(message.To));
                if (receiver != null)
                    Send(receiver.Socket, JsonConvert.SerializeObject(message)).Wait();
                else
                    Send(webSocket, JsonConvert.SerializeObject(Response)).Wait();
            }
            else
            {
                Response.Content = $"Unknown sender {message.From}";
                Send(webSocket, JsonConvert.SerializeObject(Response)).Wait();
            }
        }

        private static async Task Send(WebSocket webSocket, string message)
        {
            byte[] bytes;

            if (!string.IsNullOrEmpty(ServerConfig.Instance.ServerCommunicationKey))
                bytes = Encoding.UTF8.GetBytes(Encrypt(message));
            else
                bytes = Encoding.UTF8.GetBytes(message);

            if (webSocket.State == WebSocketState.Open)
                await webSocket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public static ServerInfo GetServer(ServerType Type)
        {
            return Servers.Where(match => match.Type == Type).MinBy(s => s.PlayerCount);
        }

        public static ServerInfo GetServer(Guid ServerID)
        {
            return Servers.FirstOrDefault(match => match.ServerId == ServerID);
        }

        private static string Encrypt(string message)
        {
            var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(ServerConfig.Instance.ServerCommunicationKey);
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;

            var stream = new MemoryStream();
            var cryptoTransform = aes.CreateEncryptor(aes.Key, null);
            var cryptoStream = new CryptoStream(stream, cryptoTransform, CryptoStreamMode.Write);

            cryptoStream.Write(Encoding.UTF8.GetBytes(message));

            return Convert.ToBase64String(stream.ToArray());
        }

        private static string Decrypt(string message)
        {
            var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(ServerConfig.Instance.ServerCommunicationKey);
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;

            var stream = new MemoryStream(Convert.FromBase64String(message));
            var cryptoTransform = aes.CreateDecryptor(aes.Key, null);
            var cryptoStream = new CryptoStream(stream, cryptoTransform, CryptoStreamMode.Read);
            var streamReader = new StreamReader(cryptoStream, Encoding.UTF8);

            return streamReader.ReadToEnd();
        }
    }
}
