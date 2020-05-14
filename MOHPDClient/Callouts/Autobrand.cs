using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using MOHPDClient.Commons;

namespace MOHPDClient.Callouts
{
    public class Autobrand : CalloutCS
    {
        private TupleList<Vector3, float> ValidTrafficStopSpawnPointsWithHeadings = new TupleList<Vector3, float>();
        private Tuple<Vector3, float> ChosenSpawnData;
        public static Random rnd = new Random();

        private int vehicle;
        private Vector3 locatie;
        private float heading;
        private string adres;
        private string car;

        public Autobrand()
        {
            
            foreach (Tuple<Vector3, float> tuple in RoadsideSpawns.TrafficStopSpawnPointsWithHeadings)
            {
                if ((Vector3.Distance(tuple.Item1, Game.Player.Character.Position) < 750f) && (Vector3.Distance(tuple.Item1, Game.Player.Character.Position) > 280f))
                {
                    ValidTrafficStopSpawnPointsWithHeadings.Add(tuple);
                }
            }

            if (ValidTrafficStopSpawnPointsWithHeadings.Count == 0)
            {
                 locatie = Common.Around(Game.Player.Character.Position, 1000.0f);
                 float heading = 3.0f;
                 API.GetClosestVehicleNodeWithHeading(locatie.X, locatie.Y, locatie.Z, ref locatie, ref heading, 0,
                     3.0f, 0);
                 //API.GetClosestMajorVehicleNode(locatie.X, locatie.Y, locatie.Z, ref locatie, 3.0f, 0);
            }
            else
            {
                ChosenSpawnData = ValidTrafficStopSpawnPointsWithHeadings[rnd.Next(ValidTrafficStopSpawnPointsWithHeadings.Count)];
                locatie = ChosenSpawnData.Item1;
                heading = ChosenSpawnData.Item2;
            }
            uint streetname = new uint();
            uint crossing = new uint();
            API.GetStreetNameAtCoord(locatie.X,locatie.Y,locatie.Z,ref streetname,ref crossing);
            adres = API.GetStreetNameFromHashKey(streetname);
            car = Common.randomCar();
        }

        public Vector3 Locatie => locatie;
        public string Adres => adres;

        public override async void GetCallout()
        {
            vehicle = await Common.spawnVehicleOnPos(car, false, locatie, heading) /*.Result*/;
            API.SetVehicleEngineHealth(vehicle,-50);
        }
        public override string GetCalloutNotification()
        {
            uint streetname = new uint();
            uint crossing = new uint();
            API.GetStreetNameAtCoord(locatie.X,locatie.Y,locatie.Z,ref streetname,ref crossing);
            return $"~r~Melding:~s~ Autobrand~n~~b~Locatie:~s~ {API.GetStreetNameFromHashKey(streetname)}";
        }
    }
}