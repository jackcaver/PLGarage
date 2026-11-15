using GameServer.Models.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GameServer.Models.Config
{
    public class SkillConfig
    {
        private static readonly Lazy<SkillConfig> instance = new(GetFromFile);

        public static SkillConfig Instance => instance.Value;

        private List<SkillLevel> skillLevels = [
            new SkillLevel
            {
                Id = 1,
                Name = "Newcomer I",
                Points = 0
            },
            new SkillLevel
            {
                Id = 2,
                Name = "Newcomer II",
                Points = 25
            },
            new SkillLevel
            {
                Id = 3,
                Name = "Newcomer III",
                Points = 50
            },
            new SkillLevel
            {
                Id = 4,
                Name = "Rookie I",
                Points = 100
            },
            new SkillLevel
            {
                Id = 5,
                Name = "Rookie II",
                Points = 200
            },
            new SkillLevel
            {
                Id = 6,
                Name = "Rookie III",
                Points = 350
            },
            new SkillLevel
            {
                Id = 7,
                Name = "Prospect I",
                Points = 500
            },
            new SkillLevel
            {
                Id = 8,
                Name = "Prospect II",
                Points = 650
            },
            new SkillLevel
            {
                Id = 9,
                Name = "Prospect III",
                Points = 800
            },
            new SkillLevel
            {
                Id = 10,
                Name = "Prodigy I",
                Points = 1000
            },
            new SkillLevel
            {
                Id = 11,
                Name = "Prodigy II",
                Points = 1250
            },
            new SkillLevel
            {
                Id = 12,
                Name = "Prodigy III",
                Points = 1500
            },
            new SkillLevel
            {
                Id = 13,
                Name = "Phenom I",
                Points = 2000
            },
            new SkillLevel
            {
                Id = 14,
                Name = "Phenom II",
                Points = 2500
            },
            new SkillLevel
            {
                Id = 15,
                Name = "Phenom III",
                Points = 3500
            },
            new SkillLevel
            {
                Id = 16,
                Name = "Hotshot I",
                Points = 5000
            },
            new SkillLevel
            {
                Id = 17,
                Name = "Hotshot II",
                Points = 6500
            },
            new SkillLevel
            {
                Id = 18,
                Name = "Hotshot III",
                Points = 8000
            },
            new SkillLevel
            {
                Id = 19,
                Name = "Celebrity I",
                Points = 10000
            },
            new SkillLevel
            {
                Id = 20,
                Name = "Celebrity II",
                Points = 11500
            },
            new SkillLevel
            {
                Id = 21,
                Name = "Celebrity III",
                Points = 13000
            },
            new SkillLevel
            {
                Id = 22,
                Name = "Guru I",
                Points = 15000
            },
            new SkillLevel
            {
                Id = 23,
                Name = "Guru II",
                Points = 25000
            },
            new SkillLevel
            {
                Id = 24,
                Name = "Guru III",
                Points = 35000
            },
            new SkillLevel
            {
                Id = 25,
                Name = "Star I",
                Points = 50000
            },
            new SkillLevel
            {
                Id = 26,
                Name = "Star II",
                Points = 60000
            },
            new SkillLevel
            {
                Id = 27,
                Name = "Star III",
                Points = 75000
            },
            new SkillLevel
            {
                Id = 28,
                Name = "Elite I",
                Points = 100000
            },
            new SkillLevel
            {
                Id = 29,
                Name = "Elite II",
                Points = 250000
            },
            new SkillLevel
            {
                Id = 30,
                Name = "Elite III",
                Points = 500000
            },
            new SkillLevel
            {
                Id = 31,
                Name = "Master I",
                Points = 750000
            },
            new SkillLevel
            {
                Id = 32,
                Name = "Master II",
                Points = 830000
            },
            new SkillLevel
            {
                Id = 33,
                Name = "Master III",
                Points = 910000
            },
            new SkillLevel
            {
                Id = 34,
                Name = "Grandmaster I",
                Points = 1000000
            },
            new SkillLevel
            {
                Id = 35,
                Name = "Grandmaster II",
                Points = 1100000
            },
            new SkillLevel
            {
                Id = 36,
                Name = "Grandmaster III",
                Points = 1200000
            },
            new SkillLevel
            {
                Id = 37,
                Name = "Grandmaster IV",
                Points = 1300000
            },
            new SkillLevel
            {
                Id = 38,
                Name = "Grandmaster V",
                Points = 1500000
            },
            new SkillLevel
            {
                Id = 39,
                Name = "Ultimate I",
                Points = 1600000
            },
            new SkillLevel
            {
                Id = 40,
                Name = "Ultimate II",
                Points = 1800000
            },
            new SkillLevel
            {
                Id = 41,
                Name = "Ultimate III",
                Points = 1900000
            },
            new SkillLevel
            {
                Id = 42,
                Name = "Ultimate IV",
                Points = 2100000
            },
            new SkillLevel
            {
                Id = 43,
                Name = "Champion I",
                Points = 2400000
            },
            new SkillLevel
            {
                Id = 44,
                Name = "Champion II",
                Points = 2600000
            },
            new SkillLevel
            {
                Id = 45,
                Name = "Champion III",
                Points = 2800000
            },
        ];

        private static SkillConfig GetFromFile()
        {
            SkillConfig config = new();
            if (File.Exists("./skill_levels.json"))
            {
                string file = File.ReadAllText("./skill_levels.json");
                var loaded = JsonConvert.DeserializeObject<List<SkillLevel>>(file);
                
                if (loaded != null && loaded.Count != 0)
                {
                    var newLevels = config.skillLevels
                        .Where(defaultLevel => defaultLevel.Id > 30 && loaded.All(level => level.Id != defaultLevel.Id))
                        .ToList();
                    loaded.AddRange(newLevels);

                    foreach (var defaultLevel in config.skillLevels.Where(level => level.Id > 30))
                    {
                        foreach (var loadedLevel in loaded.Where(level => level.Id == defaultLevel.Id))
                            loadedLevel.Points = defaultLevel.Points;
                    }

                    config.skillLevels = loaded;
                }
            }
            else
                File.WriteAllText("./skill_levels.json", JsonConvert.SerializeObject(config.skillLevels, Formatting.Indented));
            
            config.skillLevels = config.skillLevels.OrderBy(s => s.Points).ToList();
            
            return config;
        }

        public SkillLevel GetSkillLevel(int Points)
        {
            if (skillLevels.Count == 0)
            {
                return new SkillLevel
                {
                    Id = 1,
                    Name = "Newcomer I",
                    Points = 0
                };
            }
            
            int index = 0;
            for (int i = 0; i < skillLevels.Count; i++)
            {
                if (skillLevels[i].Points <= Points)
                    index = i;
            }
            return skillLevels[index];
        }

        public string GetSkillLevelList()
        {
            var skillLevelList = new List<skill_level>();

            foreach (SkillLevel skillLevel in skillLevels) 
            {
                skillLevelList.Add(new skill_level
                {
                    id = skillLevel.Id,
                    name = skillLevel.Name,
                    points = skillLevel.Points
                });
            }

            var resp = new Response<List<skill_levels>>
            {
                status = new ResponseStatus { id = 0, message = "Successful completion" },
                response =
                [
                    new skill_levels
                    {
                        total = skillLevels.Count,
                        skillLevelList = skillLevelList
                    }
                ]
            };
            return resp.Serialize();
        }
    }
}