using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
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
        
        private static List<String> CarsToSelectFrom = new List<String> {"DUKES", "BALLER", "BALLER2", "BISON", "BISON2",
            "BJXL", "CAVALCADE", "CHEETAH", "COGCABRIO", "ASEA", "ADDER", "FELON", "FELON2", "ZENTORNO",
            "WARRENER", "RAPIDGT", "INTRUDER", "FELTZER2", "FQ2", "RANCHERXL", "REBEL", "SCHWARZER", "COQUETTE", "CARBONIZZARE",
            "EMPEROR", "SULTAN", "EXEMPLAR", "MASSACRO", "DOMINATOR", "ASTEROPE", "PRAIRIE", "NINEF", "WASHINGTON",
            "CHINO", "CASCO", "INFERNUS", "ZTYPE", "DILETTANTE", "VIRGO", "F620", "PRIMO", "SULTAN", "EXEMPLAR", "F620",
            "FELON2", "FELON", "SENTINEL", "WINDSOR", "DOMINATOR", "DUKES", "GAUNTLET", "VIRGO", "ADDER", "BUFFALO",
            "ZENTORNO", "MASSACRO" };
        
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
        
        public static async Task spawnVehicle(string model, bool deletePreviousVehicle)
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
                if(deletePreviousVehicle) Game.PlayerPed.CurrentVehicle.Delete();
            }
            catch (Exception e)
            {
            }

            var vehicle = await World.CreateVehicle(model, Game.PlayerPed.Position, Game.PlayerPed.Heading);

            // set the player ped into the vehicle and driver seat
            Game.PlayerPed.SetIntoVehicle(vehicle, VehicleSeat.Driver);
        }

        public static Vehicle spawnVehicleOnPosWithReturn(Vector3 pos, float heading)
        {
            Vehicle vehicle = World.CreateRandomVehicle(pos, heading);
            return vehicle;
        }
        
        public static async Task spawnVehicleOnPos(string model, Vector3 pos, float heading)
        {
            var hash = (uint) GetHashKey(model);
            if (!IsModelInCdimage(hash) || !IsModelAVehicle(hash))
            {
                Common.Notify("Oeps! Dit voertuig bestaat helaas niet!", TEAMHOOFDWEGENRP, GIERIGENARROGANT);
                return;
            }
            var vehicle = await World.CreateVehicle(model, pos, heading);
        }
        
        public static void playSound(string file, double vol)
        {
            TriggerEvent("Client:SoundToClient", file, vol);
        }
        
        public static Vector3 Around(Vector3 start, float radius)
        {
            // Random direction.
            Vector3 direction = Common.RandomXY();
            Vector3 around = start + (direction * radius);
            return around;
        }
 
        public static float DistanceTo(Vector3 start, Vector3 end)
        {
            return (end - start).Length();
        }
 
        public static Vector3 RandomXY()
        {
            Random random = new Random(Environment.TickCount);
 
            Vector3 vector3 = new Vector3();
            vector3.X = (float)(random.NextDouble() - 0.5);
            vector3.Y = (float)(random.NextDouble() - 0.5);
            vector3.Z = 0.0f;
            vector3.Normalize();
            return vector3;
        }

        public static string randomCar()
        {
            Random random = new Random();
            return CarsToSelectFrom[random.Next(CarsToSelectFrom.Count)];
        }
    }
}