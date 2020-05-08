using CitizenFX.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using MenuAPI;
using static CitizenFX.Core.Native.API;

namespace MOHPDClient
{
    public class MOHPDClient : BaseScript
    {
        private readonly string TEAMHOOFDWEGENRP = "TeamHoofdWegenRP";
        private readonly string MELDING = "Melding";
        private readonly string GIERIGENARROGANT = "Gul en Vredelievend";

        private F5Menu f5Menu ;
        public MOHPDClient()
        {
            f5Menu = new F5Menu();
            
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
            EventHandlers["CL:Notify"] += new Action<string, string, string>(Common.Notify);
            EventHandlers["CL:PlaySound"] += new Action<string, double>(Common.playSound);
            EventHandlers["CL:SetGPS"] += new Action<Vector3>(Common.SetGPS);
            EventHandlers["CL:SpawmVehicle"] += new Action<string>(doSpawnVehicle);
            Tick += OnTick;
        }
        
        private void OnClientResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;
        }

        private async Task OnTick()
        {
            if (IsControlPressed(1, 48))
            {
                TriggerServerEvent("SV:FluitjePls");
            }

            if (Common.blipEnabled)
            {
                if (Game.Player.Character.Position.DistanceToSquared(GetBlipCoords(Common.blip)) < 3000)
                {
                    SetBlipRoute(Common.blip, false);
                    RemoveBlip(ref Common.blip);
                }
            }

            await Delay(100);
        }

        private void doSpawnVehicle(string model)
        {
            Common.spawnVehicle(model);
        }
    }
}