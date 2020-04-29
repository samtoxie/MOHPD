using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOHPDServer.Callouts;

namespace MOHPDServer
{
    public class MOHPDServer : BaseScript
    {
        public MOHPDServer()
        {
            EventHandlers["SV:Callout"] += new Action<Player>(SvCallout);
            EventHandlers["SV:VTB"] += new Action<Player>(SvVTB);
            Tick += OnTick;
        }

        // Create a function to handle the event somewhere else in your code, or use a lambda.
        private void SvCallout([FromSource] Player source)
        {
            foreach (var player in Players)
            {
                player.TriggerEvent("chat:addMessage", new
                {
                    color = new[] { 255, 0, 0 },
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
            foreach (var player in Players)
            {
                player.TriggerEvent("chat:addMessage", new
                {
                    color = new[] { 52, 113, 235 },
                    args = new[] { "[Meldkamer POL]", newCallout.GetCalloutNotification() }
                });
            }
        }
    }
}
