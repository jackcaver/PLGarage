using System;
using System.IO;
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
				instance ??= GetFromFile();

				return instance;
			}
		}

        public string NotWhitelistedText = "User \"%username\" is not whitelisted on this instance";
        [JsonIgnore]
        public string EulaText 
        { 
            get
            {
                if (!File.Exists("./eula.txt"))
                {
                    string defaultText = "Welcome %username! You have successfully logged in from %platform";
                    File.WriteAllText("./eula.txt", defaultText);
                    return defaultText;
                }
                return File.ReadAllText("./eula.txt");
            }
            set
            {
                File.WriteAllText("./eula.txt", value);
            }
        }
        public string ExternalURL = "auto:10050";
        public string MysqlConnectionString = "server=127.0.0.1;uid=root;pwd=password;database=PLGarage";
        public bool Whitelist = false;
        public bool BlockMNR = false;
        public bool BlockLBPK = false;
        public bool EnableRequestLogging = false;
        public string InstanceName = "PLGarage";
        public string ServerCommunicationKey = "";
        public string ModerationKey = "";

        private static ServerConfig GetFromFile()
        {
            if (File.Exists("./config.json")) 
            {
                string file = File.ReadAllText("./config.json");
                ServerConfig config = JsonConvert.DeserializeObject<ServerConfig>(file);
                File.WriteAllText("./config.json", JsonConvert.SerializeObject(config, Formatting.Indented));
                return config;
            }
            else
            {
                Log.Error("No configuration found");
                GenerateExample();
                Environment.Exit(1);
                return null;
            }
        }

        private static void GenerateExample() 
        {
            if (File.Exists("./ExampleConfig.json"))
            {
                Log.Information($"Example Configuration already exists at {Path.Combine(Environment.CurrentDirectory, "ExampleConfig.json")}");
            }
            else
            {
                File.WriteAllText("./ExampleConfig.json", JsonConvert.SerializeObject(new ServerConfig(), Formatting.Indented));
                Log.Information($"Generated example configuration at {Path.Combine(Environment.CurrentDirectory, "ExampleConfig.json")}");
            }
        }
    }
}