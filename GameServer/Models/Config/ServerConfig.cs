using System;
using System.Collections.Generic;
using System.IO;
using GameServer.Models.Config.ServerList;
using Newtonsoft.Json;
using Serilog;

namespace GameServer.Models.Config
{
    public class ServerConfig 
    {
        private static ServerConfig instance = null;

        public static ServerConfig Instance
        {
			get
			{
				if (instance == null)
				{
					instance = GetFromFile();
				}

				return instance;
			}
		}

        public string MysqlConnectionString { get; set; } = "server=127.0.0.1;uid=root;pwd=password;database=PLGarage";
        public Dictionary<ServerType, Server> ServerList { get; set; } = new Dictionary<ServerType, Server> { { ServerType.DIRECTORY, new Server() } };

        private static ServerConfig GetFromFile()
        {
            if (File.Exists("./config.json")) 
            {
                string file = File.ReadAllText("./config.json");
                return JsonConvert.DeserializeObject<ServerConfig>(file);
            }
            else
            {
                Log.Error("No configuration found");
                new ServerConfig().GenerateExample();
                Environment.Exit(1);
                return null;
            }
        }

        private void GenerateExample() 
        {
            if (File.Exists("./ExampleConfig.json"))
            {
                Log.Information($"Example Configuration already exists at {Path.Combine(Environment.CurrentDirectory, "ExampleConfig.json")}");
            }
            else
            {
                File.WriteAllText("./ExampleConfig.json", JsonConvert.SerializeObject(this));
                Log.Information($"Generated example configuration at {Path.Combine(Environment.CurrentDirectory, "ExampleConfig.json")}");
            }
        }
    }
}