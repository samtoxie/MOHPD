using System;
using CitizenFX.Core;

namespace MOHPDServer.Peds
{
    public class PlayerPed
    {
        private Player player;
        private String playerName;

        public PlayerPed(Player player, string playerName)
        {
            this.player = player;
            this.playerName = playerName;
        }

        public PlayerPed(Player player) : this(player, player.Name)
        {
        }

        public Player Player => player;

        public string PlayerName => playerName;
    }
}