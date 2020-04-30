using System.Collections;
using System.Collections.Generic;
using CitizenFX.Core;
using MOHPDServer.Peds;

namespace MOHPDServer.Data
{
    public class PlayerPedDAO
    {
        private ArrayList players;
        public PlayerPedDAO()
        {
            players = new ArrayList();
        }

        public PlayerPed GetForPlayer(Player player)
        {
            foreach (var player2 in players)
            {
                if (player2.Equals(player)) return (PlayerPed) player2;
            }
            return null;
        }

        public bool ContainsPlayer(PlayerPed player)
        {
            return players.Contains(player);
        }

        public void AddPlayer(PlayerPed player)
        {
            players.Add(player);
        }
        
        public void RemovePlayer(PlayerPed player)
        {
            if(players.Contains(player)) players.Remove(player);
        }
    }
}