using GameServer.Models.Response;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace GameServer.Models.Config
{
    public class SkillConfig
    {
        private static SkillConfig instance = null;

        public static SkillConfig Instance
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
        ];

        private static SkillConfig GetFromFile()
        {
            SkillConfig config = new();
            if (File.Exists("./skill_levels.json"))
            {
                string file = File.ReadAllText("./skill_levels.json");
                config.skillLevels = JsonConvert.DeserializeObject<List<SkillLevel>>(file);
            }
            else
                File.WriteAllText("./skill_levels.json", JsonConvert.SerializeObject(config.skillLevels, Formatting.Indented));
            
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
            skillLevels.Sort((curr, prev) => curr.Points.CompareTo(prev.Points));
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
            skillLevels.Sort((curr, prev) => curr.Points.CompareTo(prev.Points));
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
