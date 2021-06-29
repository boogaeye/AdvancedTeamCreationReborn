using System.ComponentModel;
using Exiled.API.Interfaces;
using System.IO;
using Exiled.API.Features;
using Exiled.Loader;
using System.Collections.Generic;
using System.Linq;
using AdvancedTeamCreationReborn.Teams;

namespace AdvancedTeamCreationReborn
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        public string ConfigsFolder { get; set; } = Path.Combine(Paths.Configs, "AdvancedTeamCreation");
        public AdvancedTeam[] advancedTeams = new AdvancedTeam[] {
            new AdvancedTeam
            {
                name = "TTA",
                chance = 35,
                AdvancedTeamSubclassNames = new string[]{ 
                    "TTARookie"
                },
                SpawnBank = new string[]{ 
                    "TTARookie",
                    "Loop"
                },
                SpawnableTeams = new Respawning.SpawnableTeamType[] { Respawning.SpawnableTeamType.ChaosInsurgency }
            },
            new AdvancedTeam
            {
                name = "BruhMoungUs2Test",
                chance = 100,
                AdvancedTeamSubclassNames = new string[]{ 
                    "Bruh2Commander",
                    "SadBruh2Commander"
                },
                SpawnableTeams = new Respawning.SpawnableTeamType[] { Respawning.SpawnableTeamType.ChaosInsurgency, Respawning.SpawnableTeamType.NineTailedFox }
            }
        };
        public AdvancedTeam.AdvancedTeamSubclass[] AdvancedTeamSubclasses = new AdvancedTeam.AdvancedTeamSubclass[]{
                    new AdvancedTeam.AdvancedTeamSubclass
                    {
                        name = "TTARookie",
                        HP = 130,
                        inventory = new List<ItemType>(){ ItemType.GunUSP, ItemType.Adrenaline, ItemType.KeycardFacilityManager },
                        roleModel = RoleType.ClassD
                    },
                    new AdvancedTeam.AdvancedTeamSubclass
                    {
                        name = "Bruh2Commander",
                        HP = 100,
                        inventory = new List<ItemType>(){ ItemType.Adrenaline, ItemType.Coin },
                        roleModel = RoleType.ClassD
                    },
                    new AdvancedTeam.AdvancedTeamSubclass
                    {
                        name = "SadBruh2Commander",
                        HP = 100,
                        inventory = new List<ItemType>(){ ItemType.Coin },
                        roleModel = RoleType.Scientist
                    }
                };
        public List<AdvancedTeam> Teams = new List<AdvancedTeam>();
        public List<AdvancedTeam.AdvancedTeamSubclass> Subteams = new List<AdvancedTeam.AdvancedTeamSubclass>();
        public void LoadConfigs()
        {
            Teams.Clear();

            if (Directory.Exists(ConfigsFolder) == false)
            {
                Directory.CreateDirectory(ConfigsFolder);
            }

            var teamsdir = Path.Combine(ConfigsFolder, "Teams");
            if (Directory.Exists(teamsdir) == false)
            {
                Directory.CreateDirectory(teamsdir);
                foreach (AdvancedTeam tm in advancedTeams)
                {
                    File.WriteAllText(Path.Combine(teamsdir, $"{tm.name}.yml"), Loader.Serializer.Serialize(tm));
                    var subteamsdir = Path.Combine(teamsdir, tm.name + "Subteams");
                    Directory.CreateDirectory(subteamsdir);
                    foreach (string Subteam in tm.AdvancedTeamSubclassNames)
                    {
                        var subteamRef = AdvancedTeamSubclasses.FirstOrDefault(x => x.name == Subteam);
                        File.WriteAllText(Path.Combine(subteamsdir, $"{Subteam}.yml"), Loader.Serializer.Serialize(subteamRef));
                    }
                }
            }

            var tfiles = Directory.GetFiles(teamsdir);
            foreach (var file in tfiles.Where(x => x.EndsWith("yml")))
            {
                Teams.Add(Loader.Deserializer.Deserialize<AdvancedTeam>(File.ReadAllText(file)));
                var stfiles = Directory.GetFiles(Path.Combine(teamsdir, file.Replace(".yml", "") + "Subteams"));
                foreach (var stfile in stfiles.Where(x => x.EndsWith("yml")))
                {
                    Subteams.Add(Loader.Deserializer.Deserialize<AdvancedTeam.AdvancedTeamSubclass>(File.ReadAllText(stfile)));
                }
                
            }
            //Commented since this doesnt exist anymore
            // This let the people change the normal requeriments and friendly teams of the base game teams
            //string npath = Path.Combine(ConfigsFolder, "NormalTeams.yml");
            //if (File.Exists(npath) == false)
            //{
            //    NormalConfigs = new NormalTeams();
            //    File.WriteAllText(npath, Loader.Serializer.Serialize(NormalConfigs));
            //}
            //else
            //{
            //    NormalConfigs = Loader.Deserializer.Deserialize<NormalTeams>(File.ReadAllText(npath));
            //}

            //foreach (var ntm in NormalConfigs.NTeams)
            //{
            //    Teams.Add(new Teams
            //    {
            //        Active = false,
            //        Name = ntm.Name,
            //        Requirements = ntm.Requirements,
            //        Friendlys = ntm.FriendlyTeams
            //    });
            //}
        }

    }
}
