using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using AdvancedTeamCreationReborn.Teams;

namespace AdvancedTeamCreationReborn
{
    public class Events
    {
        private readonly Plugin<Config> plugin;
        public Events(Plugin<Config> plugin)
        {
            this.plugin = plugin;
        }

        public void RoleChange(ChangedRoleEventArgs ev)
        {
            ev.Player.SetAdvancedTeamTitle(new AdvancedTeam(ev.Player.Team));
        }

        public void SpawnTeam(RespawningTeamEventArgs ev)
        {
            AdvancedTeam teamRef = ev.AttemptedAdvancedSpawn();
            Log.Debug($"Captured team from spawning team event {teamRef.name}");
            int spawn = 0;
            foreach (Player p in ev.Players)
            {
                if (teamRef.IsCustomTeam)
                {
                    if (teamRef.SpawnBank.Length <= spawn)
                    {
                        p.SetRole(RoleType.Spectator);
                        p.ShowHint($"Could not spawn in due to this spawn being {teamRef.name}");
                        return;
                    }
                    if (teamRef.SpawnBank[spawn].ToLower() == "loop") spawn--;
                    
                    p.SetAdvancedTeam(teamRef, AdvancedTeam.AdvancedTeamSubclass.GetAdvancedTeamSubclass(teamRef.SpawnBank[spawn]));
                }
                spawn++;
            }
        }
    }
}
