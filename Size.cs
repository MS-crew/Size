using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Size
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Size : ICommand, IUsageProvider
    {
        public string Command { get; } = "size";
        public string[] Aliases { get; } = Array.Empty<string>();
        public string Description { get; } = "Sets the size of all users or a user";
        public string[] Usage { get; } = new string[] { "%player%", "x", "y", "z" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("size"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 2)
            {
                response = "Usage:\nsize (player id / name) (x value) (y value) (z value)" +
                           "\nsize (all / *) (x value) (y value) (z value)" +
                           "\nsize (Team name) (x value) (y value) (z value)" +
                           "\nsize (player id / name) reset" +
                           "\nsize (all / *) reset";
                return false;
            }

            if (IsGroupCommand(arguments.At(0)))
            {
                if (arguments.At(1).ToLower() == "reset")
                {
                    var players = GetPlayersByGroup(arguments.At(0));
                    if (players == null || !players.Any())
                    {
                        response = $"No players found for group: {arguments.At(0)}";
                        return false;
                    }

                    ResetPlayerSize(players);
                    response = $"All {arguments.At(0)} players' size has been reset to default (1, 1, 1)";
                    return true;
                }

                if (arguments.Count < 4 || !TryGetVector3(arguments, out Vector3 size))
                {
                    response = "Usage: size (all / *) (x) (y) (z)";
                    return false;
                }

                var playersToResize = GetPlayersByGroup(arguments.At(0));
                if (playersToResize == null || !playersToResize.Any())
                {
                    response = $"No players found for group: {arguments.At(0)}";
                    return false;
                }

                SetPlayerSize(playersToResize, size);

                if (arguments.At(0).ToLower() == "all" || arguments.At(0) == "*")
                {
                    response = $"All players' size has been set to {size}";
                }
                else
                {
                    response = $"All {arguments.At(0)} players' size has been set to {size}";
                }

                return true;
            }

            var targetPlayers = Player.GetProcessedData(arguments);
            if (targetPlayers == null || !targetPlayers.Any())
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }

            if (arguments.At(1).ToLower() == "reset")
            {
                ResetPlayerSize(targetPlayers);
                response = "The specified player's size has been reset";
                return true;
            }

            if (arguments.Count != 4 || !TryGetVector3(arguments, out Vector3 newSize))
            {
                response = "Usage: size (player id / name) (x) (y) (z)";
                return false;
            }

            SetPlayerSize(targetPlayers, newSize);
            response = $"The specified player's size has been set to {newSize}";
            return true;
        }
        private bool IsGroupCommand(string argument) =>
            argument == "all" || argument == "*" ||
            argument == "SCPs" || argument == "FoundationForces" ||
            argument == "ChaosInsurgency" || argument == "Scientists" ||
            argument == "ClassD" || argument == "OtherAlive";

        private IEnumerable<Player> GetPlayersByGroup(string group)
        {
            if (group == "all" || group == "*")
            {
                return Player.List;
            }
            return Player.List.Where(player => player.Role.Team.ToString() == group);
        }

        private void ResetPlayerSize(IEnumerable<Player> players)
        {
            foreach (var player in players)
            {
                player.Scale = new Vector3(1, 1, 1);
            }
        }

        private void SetPlayerSize(IEnumerable<Player> players, Vector3 size)
        {
            foreach (var player in players)
            {
                player.Scale = size;
            }
        }
        private bool TryGetVector3(ArraySegment<string> arguments, out Vector3 size)
        {
            size = Vector3.zero;
            if (!float.TryParse(arguments.At(1), out float x)) return false;
            if (!float.TryParse(arguments.At(2), out float y)) return false;
            if (!float.TryParse(arguments.At(3), out float z)) return false;

            size = new Vector3(x, y, z);
            return true;
        }
    }
}
