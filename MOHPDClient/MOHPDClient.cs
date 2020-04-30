using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using MOHPDServer.Callouts;
using static CitizenFX.Core.Native.API;

namespace MOHPDClient
{
    public class MOHPDClient : BaseScript
    {
        public MOHPDClient()
        {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
            EventHandlers["CL:Inmelden"] += new Action<string>(Inmelden);
            Tick += OnTick;
        }
        
        private void OnClientResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;
            
            RegisterCommand("onduty", new Action<int, List<object>, string>((source, args, raw) =>
            {
                TriggerServerEvent("SV:Inmelden", GetPlayerFromServerId(source));
                TriggerEvent("CL:Inmelden", GetPlayerFromServerId(source));
            }), false);
        }

        private async Task OnTick()
        {
            await Delay(100);
        }
        
        private void Inmelden(string resourceName)
        {
            RegisterCommand("stcallout", new Action<int, List<object>, string>((source, args, raw) =>
            {
                TriggerServerEvent("SV:Callout", GetPlayerFromServerId(source));
            }), false);
            
            RegisterCommand("transport", new Action<int, List<object>, string>((source, args, raw) =>
            {
                TriggerServerEvent("SV:VTB", GetPlayerFromServerId(source));
            }), false);
        }
        
    }
}
