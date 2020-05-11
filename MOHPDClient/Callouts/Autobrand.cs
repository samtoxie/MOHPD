using System;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using MOHPDClient;

namespace MOHPDServer.Callouts
{
    public class Autobrand : Callout
    {
        private Vehicle vehicle;
        private Vector3 locatie;

        public Autobrand()
        {
            locatie = Common.Around(Game.Player.Character.Position, 100.0f);
            float heading = new float();
            API.GetClosestVehicleNodeWithHeading(locatie.X, locatie.Y, locatie.Z, ref locatie, ref heading, 2, 3.0f, 0);
            vehicle = World.CreateRandomVehicle(locatie, heading);
            API.StartEntityFire(vehicle.Handle);
            Common.SetGPS(locatie);
            
        }

        public Vector3 Locatie => locatie;

        public override void GetCallout()
        {
            throw new System.NotImplementedException();
        }

        public override string GetCalloutNotification()
        {
            throw new System.NotImplementedException();
        }
    }
}