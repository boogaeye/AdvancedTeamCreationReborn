using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using CommandSystem;
using Exiled.Permissions.Extensions;
using AdvancedTeamCreationReborn.Teams;

namespace AdvancedTeamCreationReborn.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class TeamsAlive : ICommand
    {
        public string Command { get; } = "teamsalive";
        public string[] Aliases { get; } = { "ta", "tma", "talive" };
        public string Description { get; } = "Tells you all teams alive";
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player ply = Player.Get(sender as CommandSender);
            if (ply.CheckPermission("ATCR.teamsalive") || ply.CheckPermission("'*'"))
            {
                response = string.Empty;
                foreach (KeyValuePair<Player, AdvancedTeam.AdvancedTeamPair> t in MainTeamPlugin.initalizedTeams)
                {
                    response += $"\nPlayer: {t.Key.Nickname}, Team: {t.Value.AdvancedTeam.name}";
                }
                return true;
            }
            else
            {
                response = "You dont have permission";
                return false;
            }
        }
    }
}
