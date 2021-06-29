using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Loader;
using AdvancedTeamCreationReborn.Teams;
using Exiled.API.Enums;

namespace AdvancedTeamCreationReborn
{
    public class MainTeamPlugin : Plugin<Config>
    {
        public override string Name { get; } = "Advanced Team Creation Reborn";
        public override PluginPriority Priority { get; } = PluginPriority.Lowest;
        public override Version RequiredExiledVersion { get; } = new Version("2.10.0");
        public override string Author { get; } = "BoogaEye && Raul125";
        public static Dictionary<Player, AdvancedTeam.AdvancedTeamPair> initalizedTeams = new Dictionary<Player, AdvancedTeam.AdvancedTeamPair>();
        public static MainTeamPlugin inst = null;
        public static bool SubclassEnabled = false;
        public static Events events = null;
        public override void OnEnabled()
        {
            inst = this;
            events = new Events(this);
            Exiled.Events.Handlers.Server.RespawningTeam += events.SpawnTeam;
            Exiled.Events.Handlers.Player.ChangedRole += events.RoleChange;
            foreach (IPlugin<IConfig> plugin in Loader.Plugins)
            {
                if (plugin.Name == "Subclass" && plugin.Config.IsEnabled)
                {
                    SubclassEnabled = true;
                    Log.Debug("Found Advanced Subclassing", Config.Debug);
                }
            }
            Log.Debug("Creating Configs", Config.Debug);
            Config.LoadConfigs();
            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.ChangedRole -= events.RoleChange;
            Exiled.Events.Handlers.Server.RespawningTeam -= events.SpawnTeam;
            base.OnDisabled();
        }
    }
}
