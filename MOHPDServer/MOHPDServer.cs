using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOHPDServer.Callouts;
using MOHPDServer.Data;
using MOHPDServer.Peds;

namespace MOHPDServer
{
    public class MOHPDServer : BaseScript
    {
        private static PlayerPedDAO playerPedDao = new PlayerPedDAO();
        private static int[] dispatchColors = new[] {52, 113, 235};
        private static string dispatchText = "[Meldkamer POL]";
        public MOHPDServer()
        {
            EventHandlers["SV:Inmelden"] += new Action<Player>(SvInmelden);
            EventHandlers["SV:Callout"] += new Action<Player>(SvCallout);
            EventHandlers["SV:VTB"] += new Action<Player>(SvVTB);
            EventHandlers["playerDropped"] += new Action<Player, string>(OnPlayerDropped);
            EventHandlers["playerConnecting"] += new Action<Player, string>(OnPlayerConnecting);
            Tick += OnTick;
        }

        private void SvInmelden([FromSource] Player player)
        {
            if (playerPedDao.Contains(player))
            {
                player.TriggerEvent("chat:addMessage", new
                {
                    color = dispatchColors ,
                    args = new[] { dispatchText, "U bent reeds ingemeld!" }
                });
            }
            else
            {
                playerPedDao.Add(player);
                player.TriggerEvent("chat:addMessage", new
                {
                    color = dispatchColors,
                    args = new[] { dispatchText, "U bent succesvol ingemeld, fijne dienst!" }
                });
            }
        }
        
        private void SvCallout([FromSource] Player source)
        {
            foreach (var player in playerPedDao)
            {
                player.TriggerEvent("chat:addMessage", new
                {
                    color = dispatchColors,
                    args = new[] { "[StCallouts]", "je kanker moeder" }
                });
            }
        }
        private async Task OnTick()
        {
            await Delay(100);
        }

        private void SvVTB([FromSource] Player source)
        {
            Callout newCallout = new VTB();
            foreach (var player in playerPedDao)
            {
                player.TriggerEvent("chat:addMessage", new
                {
                    color = dispatchColors,
                    args = new[] { dispatchText, newCallout.GetCalloutNotification() }
                });
            }
        }
        
        private void OnPlayerDropped([FromSource]Player player, string reason)
        {
            if(playerPedDao.ContainsPlayer(playerPedDao.GetForPlayer(player))) playerPedDao.RemovePlayer(playerPedDao.GetForPlayer(player));
        }
        
        private void OnPlayerConnecting([FromSource]Player player, string playerName)
        {
            if (playerPedDao.ContainsPlayer(playerPedDao.GetForPlayer(player))) playerPedDao.RemovePlayer(playerPedDao.GetForPlayer(player));
            playerPedDao.AddPlayer(new PlayerPed(player,playerName));
        }
    }
}
