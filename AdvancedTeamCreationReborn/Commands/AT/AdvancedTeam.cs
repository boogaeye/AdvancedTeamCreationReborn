using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using CommandSystem;

namespace AdvancedTeamCreationReborn.Commands.AT
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class AdvancedTeam : ParentCommand
    {
        public AdvancedTeam() => LoadGeneratedCommands();

        public override string Command { get; } = "advancedteam";

        public override string[] Aliases { get; } = { "adtm", "adt", "atc" };

        public override string Description { get; } = "AdvancedTeam main command";
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new TeamsAlive());
        }
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player ply = Player.Get(((CommandSender)sender).Nickname);
            if (arguments.Count == 0)
            {
                response = "You need to enter an argument\n<b>forcenextteam</b>\n<b>forceteam</b>\n<b>teamsalive</b>\n<b>reload</b>";
                return true;
            }
            else if (arguments.Contains("reload"))
            {
                if (ply.CheckPermission("ATC.reload"))
                {
                    MainTeamPlugin.inst.Config.LoadConfigs();
                    response = "Done!";
                    return true;
                }
                else
                {
                    response = "You dont have permission to use this command";
                    return false;
                }
            }
            else
            {
                response = "Invalid Argument";
                return false;
            }
        }
    }
}
