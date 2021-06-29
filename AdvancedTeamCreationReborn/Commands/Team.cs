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
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Team : ICommand
    {
        public string Command { get; } = "team";
        public string[] Aliases { get; } = { "tm" };
        public string Description { get; } = "Tells you your team";
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player ply = Player.Get(sender as CommandSender);
            if (ply.CheckPermission("ATCR.team") || ply.CheckPermission("'*'"))
            {
                response = ply.AdvancedTeam().name;
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
