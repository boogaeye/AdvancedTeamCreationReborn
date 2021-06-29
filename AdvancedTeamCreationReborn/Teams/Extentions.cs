using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.API.Enums;
using Exiled.Events.EventArgs;
using Exiled.API.Interfaces;
using Subclass;
using MEC;

namespace AdvancedTeamCreationReborn.Teams
{
    public static class Extentions
    {
        public static AdvancedTeam AdvancedTeam(this Player player)
        {
            return MainTeamPlugin.initalizedTeams[player].AdvancedTeam;
        }

        public static void SetAdvancedTeam(this Player player, AdvancedTeam team, AdvancedTeam.AdvancedTeamSubclass teamSubclass)
        {
            var teamref = team;
            if (MainTeamPlugin.inst.Config.Teams.Contains(team))
            {
                teamref = MainTeamPlugin.inst.Config.Teams.FirstOrDefault(x => x.name == team.name);
            }
            
            Timing.CallDelayed(0.2f, () =>
            {

                if (teamSubclass != null)
                {
                    var subTeamref = teamref.AdvancedTeamSubclassNames.FirstOrDefault(x => x == teamSubclass.name);
                    player.SetRole(teamSubclass.roleModel, true);
                    player.MaxHealth = (int)teamSubclass.HP;
                    player.Health = (int)teamSubclass.HP;
                    player.ResetInventory(teamSubclass.inventory);
                    Log.Debug("Setting Player Team", MainTeamPlugin.inst.Config.Debug);

                }
                //get rid of the subclass because they dont need it 0_0 <--- MTF classes "They be missing out bruh..."
                if (MainTeamPlugin.SubclassEnabled && team.IsCustomTeam)
                {
                    Log.Debug($"Removing Player Subclass {API.GetPlayersSubclass(player)}", MainTeamPlugin.inst.Config.Debug);
                    Subclass.API.RemoveClass(player);
                }
                
                MainTeamPlugin.initalizedTeams[player] = new AdvancedTeam.AdvancedTeamPair(teamref, teamSubclass);
                Log.Debug($"Setting {player.Nickname}'s team to {MainTeamPlugin.initalizedTeams[player]}", MainTeamPlugin.inst.Config.Debug);
            });
        }
        public static void SetAdvancedTeam(this Player player, AdvancedTeam team)
        {
            player.SetAdvancedTeam(team, null);
        }

        public static void SetAdvancedTeamTitle(this Player player, AdvancedTeam team)
        {
            MainTeamPlugin.initalizedTeams[player] = new AdvancedTeam.AdvancedTeamPair(team, new Teams.AdvancedTeam.AdvancedTeamSubclass());
            Log.Debug($"Setting {player.Nickname}'s team to {MainTeamPlugin.initalizedTeams[player]}", MainTeamPlugin.inst.Config.Debug);
        }

        public static AdvancedTeam AttemptedAdvancedSpawn(this RespawningTeamEventArgs ev)
        {
            if (ev.IsAllowed)
            {
                List<AdvancedTeam> roles = new List<AdvancedTeam>() { };
                roles.Add(new AdvancedTeam((Team)ev.NextKnownTeam));
                Log.Debug("Adding Original Team", MainTeamPlugin.inst.Config.Debug);
                foreach (AdvancedTeam a in MainTeamPlugin.inst.Config.Teams)
                {
                    if (a.SpawnableTeams.Contains(ev.NextKnownTeam) && !a.IsOverride())
                    {
                        roles.Add(a);
                        Log.Debug($"Adding {a.name}", MainTeamPlugin.inst.Config.Debug);
                    }
                }
                AdvancedTeam teamRef = roles[new Random().Next(0, roles.Count)];
                if (new Random().Next(0, 99) <= teamRef.chance)
                {
                    return teamRef;
                }
                else
                {
                    return roles[0];
                }
            }
            return null;
        }
        
        
    }
    public class AdvancedTeam
    {
        public string name { get; set; } = "NoTeam";
        public string[] AdvancedTeamSubclassNames { get; set; } = new string[] { };
        public uint chance { get; set; } = 0;
        public Respawning.SpawnableTeamType[] SpawnableTeams { get; set; } = new Respawning.SpawnableTeamType[] { };
        public string[] SpawnBank = new string[] { };
        public bool IsCustomTeam = true;
        public bool IsOverrideTeam = false;
        public AdvancedTeam()
        {
            if (MainTeamPlugin.inst != null)
                Log.Debug($"Formatting Custom Team", MainTeamPlugin.inst.Config.Debug);
        }
        public AdvancedTeam(Team team)
        {
            if (MainTeamPlugin.inst != null)
            {
                Log.Debug($"Formatting Team {team} to be a custom team", MainTeamPlugin.inst.Config.Debug);
                foreach (AdvancedTeam a in MainTeamPlugin.inst.Config.Teams)
                {
                    if (a.name == team.ToString())
                    {
                        IsOverrideTeam = true;
                        IsCustomTeam = false;
                        Log.Debug($"Cancelling format for team {team} as it has override", MainTeamPlugin.inst.Config.Debug);
                        name = team.ToString();
                        AdvancedTeamSubclassNames = a.AdvancedTeamSubclassNames;
                        chance = 100;
                        return;
                    }
                }
                name = team.ToString();
                IsCustomTeam = false;
            }
        }
        public class AdvancedTeamSubclass
        {
            public string name { get; set; } = "Subcommander";
            public uint HP { get; set; } = 100;
            public List<ItemType> inventory { get; set; } = new List<ItemType> { };
            public RoleType roleModel { get; set; } = RoleType.ClassD;
            public static AdvancedTeamSubclass GetAdvancedTeamSubclass(string name)
            {
                var bruh = MainTeamPlugin.inst.Config.Subteams.FirstOrDefault(x => x.name == name);
                if (bruh != null)
                {
                    return bruh;
                }
                return new AdvancedTeamSubclass();
            }
        }
        public bool IsOverride()
        {
            foreach (AdvancedTeam a in MainTeamPlugin.inst.Config.advancedTeams)
            {
                foreach (Team team in Enum.GetValues(typeof(Team)))
                {
                    if (a.name == team.ToString())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public class AdvancedTeamPair
        {
            public AdvancedTeamPair(AdvancedTeam AT, AdvancedTeamSubclass ATS)
            {
                AdvancedTeam = AT;
                AdvancedSubteam = ATS;
            }
            public AdvancedTeam AdvancedTeam { get; }
            public AdvancedTeamSubclass AdvancedSubteam { get; }
            public override string ToString()
            {
                return $"AdvancedTeam: {AdvancedTeam.name} AdvancedSubteam: {AdvancedSubteam.name}";
            }
        }
        public static bool TryGetTeam(object team, out AdvancedTeam advancedTeam)
        {
            advancedTeam = null;
            if (team.GetType() == typeof(AdvancedTeam))
            {
                advancedTeam = (AdvancedTeam)team;
                return true;
            }
            else if (team.GetType() == typeof(Team))
            {
                advancedTeam = new AdvancedTeam { name = ((Team)team).ToString() };
                return true;
            }
            return false;
        }
    }
}
