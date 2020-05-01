using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOHPDServer.Callouts;
using MOHPDServer.Data;

namespace MOHPDServer
{
    public class MOHPDServer : BaseScript
    {
        private static PoliceDAO policeDao = new PoliceDAO();
        public static int[] dispatchColors = new[] {52, 113, 235};
        private static string dispatchText = "[Meldkamer POL]";
        public MOHPDServer()
        {
            EventHandlers["SV:Inmelden"] += new Action<Player>(SvInmelden);
            EventHandlers["SV:Callout"] += new Action<Player>(SvCallout);
            EventHandlers["SV:VTB"] += new Action<Player>(SvVTB);
            EventHandlers["SV:FluitjePls"] += new Action<Player>(SvFluitjePls);
            EventHandlers["SV:Discord"] += new Action<Player>(SvDiscord);
            EventHandlers["playerDropped"] += new Action<Player, string>(OnPlayerDropped);
            Tick += OnTick;
        }

        private void SvFluitjePls([FromSource] Player player)
        {
            TriggerEvent("Server:SoundToRadius", player.Character.NetworkId , 50.0f, "fluit", 0.5f);
        }

        private void SvInmelden([FromSource] Player player)
        {
            if (policeDao.Contains(player))
            {
                player.TriggerEvent("chat:addMessage", new
                {
                    color = dispatchColors ,
                    args = new[] { dispatchText, "U bent reeds ingemeld!" }
                });
            }
            else
            {
                policeDao.Add(player);
                player.TriggerEvent("chat:addMessage", new
                {
                    color = dispatchColors,
                    args = new[] { dispatchText, "U bent succesvol ingemeld, fijne dienst!" }
                });
            }
        }
        
        private void SvCallout([FromSource] Player source)
        {
            foreach (var player in policeDao)
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
            foreach (var player in policeDao)
            {
                player.TriggerEvent("chat:addMessage", new
                {
                    color = dispatchColors,
                    args = new[] { dispatchText, newCallout.GetCalloutNotification() }
                });
            }
        }
        
        private void SvDiscord([FromSource] Player source)
        {
            source.TriggerEvent("chat:addMessage", new
            {
                color =dispatchColors ,
                args = new[] { "Discord: ", "https://discord.gg/WTBWBNv" }
            });
        }
        
        private void OnPlayerDropped([FromSource]Player player, string reason)
        {
            if(policeDao.Contains(player)) policeDao.Remove(player);
        }
    }
}
