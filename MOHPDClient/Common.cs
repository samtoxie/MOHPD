using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace MOHPDClient
{
    public class Common : BaseScript
    {
        public static readonly string TEAMHOOFDWEGENRP = "TeamHoofdWegenRP";
        public static readonly string MELDING = "Melding";
        public static readonly string GIERIGENARROGANT = "Gul en Vredelievend";
        
        public static bool blipEnabled = false;
        public static int blip = 0;
        
        public static  void SetGPS(Vector3 v)
        {
            blip = AddBlipForCoord(v.X, v.Y, v.Z);
            SetBlipColour(blip, 58);
            SetBlipRouteColour(blip, 58);
            SetBlipRoute(blip, true);
            blipEnabled = true;
            Notify("De route is toegevoegd aan je GPS!", "Assistentie Collega",TEAMHOOFDWEGENRP);
        }
        
        public static void Notify(string message, string title, string subtitle)
        {
            SetNotificationTextEntry("STRING");
            AddTextComponentString(message);
            SetNotificationMessage("CHAR_CALL911", "CHAR_CALL911", false, 0, title, subtitle);
            DrawNotification(true, false);
        }
        
        public static async Task spawnVehicle(string model)
        {
            var hash = (uint) GetHashKey(model);
            if (!IsModelInCdimage(hash) || !IsModelAVehicle(hash))
            {
                Common.Notify("Oeps! Dit voertuig bestaat helaas niet!", TEAMHOOFDWEGENRP, GIERIGENARROGANT);
                return;
            }

            // create the vehicle
            try
            {
                Game.PlayerPed.CurrentVehicle.Delete();
            }
            catch (Exception e)
            {
            }

            var vehicle = await World.CreateVehicle(model, Game.PlayerPed.Position, Game.PlayerPed.Heading);

            // set the player ped into the vehicle and driver seat
            Game.PlayerPed.SetIntoVehicle(vehicle, VehicleSeat.Driver);
        }
        
        public static void playSound(string file, double vol)
        {
            TriggerEvent("Client:SoundToClient", file, vol);
        }
    }
}