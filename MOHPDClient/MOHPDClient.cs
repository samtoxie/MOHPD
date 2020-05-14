using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using MOHPDClient.Commons;
using MOHPDServer;
using static CitizenFX.Core.Native.API;

namespace MOHPDClient
{
    public class MOHPDClient : BaseScript
    {
        private readonly string TEAMHOOFDWEGENRP = "TeamHoofdWegenRP";
        private readonly string MELDING = "Melding";
        private readonly string GIERIGENARROGANT = "Gul en Vredelievend";

        private F5Menu f5Menu;
        private Vector3 latestGPS;
        private DateTime latestGPSTime;
        public MOHPDClient()
        {
            f5Menu = new F5Menu();
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
            EventHandlers["CL:Notify"] += new Action<string, string, string>(Common.Notify);
            EventHandlers["CL:PlaySound"] += new Action<string, double>(Common.playSound);
            EventHandlers["CL:SetGPS"] += new Action<Vector3>(Common.SetGPS);
            EventHandlers["CL:UpdateGPS"] += new Action<Vector3,long>(UpdateGPS);
            EventHandlers["CL:SpawnVehicle"] += new Action<string,bool>(doSpawnVehicle);
            
            RegisterCommand("setGPS",
                new Action<int, List<object>, string>((source, args, raw) =>
                {
                    if (latestGPS != null && latestGPSTime != null)
                    {
                        TimeSpan diff = DateTime.Now.Subtract(latestGPSTime);
                        if (diff.TotalSeconds <= 120)
                        {
                            Common.SetGPS(latestGPS);
                        }
                    }
                    else Common.Notify("Er is momenteel geen melding actief!", "Meldkamer",
                        TEAMHOOFDWEGENRP);
                }), false);
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

        private void doSpawnVehicle(string model, bool deletePreviousVehicle)
        {
            Common.spawnVehicle(model, deletePreviousVehicle);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vec">Vector3 location</param>
        /// <param name="time">DateTime in binary</param>
        private void UpdateGPS(Vector3 vec, long time)
        {
            latestGPS = vec; 
            latestGPSTime = DateTime.FromBinary(time);
        }
    }
}