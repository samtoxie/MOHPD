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

        private bool blipEnabled = false;
        private int blip;

        private String config = Function.Call<String>(Hash.LOAD_RESOURCE_FILE, "MOHPD", "config.ini");
        private ArrayList vehicleModels;
        private ArrayList vehicles;

        Menu menu = new Menu("TeamHoofdwegenRP", "Gul en Vredelievend");
        MenuItem inmelden = new MenuItem("Inmelden");
        MenuItem discord = new MenuItem("Discord", "Link naar de TeamHoofdwegenRP Discord.");

        MenuItem calloutsMenuButton = new MenuItem("Meldingen", "Hier kunnen meldingen worden gegenereerd.")
            {Label = "→→→"};

        MenuItem voertuigenMenuButton = new MenuItem("Voertuigen", "Hier kan je een voertuig inspawnen.")
            {Label = "→→→"};

        MenuItem noodknop = new MenuItem("",
                "Alleen gebruiken in geval van nood, stuurt een signaal naar alle collega's!")
            {LeftIcon = MenuItem.Icon.WARNING, Label = "~r~Noodknop"};


        Menu calloutsMenu = new Menu("Meldingen", "Gul en Vredelievend");
        MenuItem startVTB = new MenuItem("VTB");

        Menu voertuigenMenu = new Menu("Voertuigen", "Gul en Vredelievend");


        public MOHPDClient()
        {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
            EventHandlers["CL:Inmelden"] += new Action<string>(Inmelden);
            EventHandlers["CL:Notify"] += new Action<string, string, string>(Notify);
            EventHandlers["CL:PlaySound"] += new Action<string, double>(playSound);
            EventHandlers["CL:NoodknopGPS"] += new Action<Vector3>(NoodknopGPS);


            vehicleModels = new ArrayList();
            vehicles = new ArrayList();
            LoadConfig();

            MenuController.MenuToggleKey = Control.SelectCharacterMichael;
            MenuController.AddMenu(menu);
            MenuController.AddSubmenu(menu, calloutsMenu);
            MenuController.AddSubmenu(menu, voertuigenMenu);

            menu.AddMenuItem(inmelden);
            menu.AddMenuItem(discord);

            menu.OnItemSelect += (menu, item, index) =>
            {
                if (item == inmelden)
                {
                    TriggerServerEvent("SV:Inmelden");
                    TriggerEvent("CL:Inmelden", GetPlayerPed(-1));
                }

                if (item == discord)
                {
                    TriggerServerEvent("SV:Discord");
                }

                if (item == noodknop)
                {
                    TriggerServerEvent("SV:NoodknopIngedrukt", Game.Player.Character.Position);
                }
            };

            calloutsMenu.AddMenuItem(startVTB);

            calloutsMenu.OnItemSelect += (menu, item, index) =>
            {
                if (item == startVTB)
                {
                    TriggerServerEvent("SV:VTB");
                }
            };

            foreach (MenuItem car in vehicles)
            {
                voertuigenMenu.AddMenuItem(car);
            }

            voertuigenMenu.OnItemSelect += (menu, item, index) =>
            {
                foreach (MenuItem m in vehicles)
                {
                    if (item == m)
                    {
                        spawnVehicle(vehicleModels[m.Index] as string);
                    }
                }
            };

            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;
            Tick += OnTick;
        }

        private void LoadConfig()
        {
            StringReader strReader = new StringReader(config);
            String configLine = strReader.ReadLine();
            if (configLine != null && configLine == "<vehicles>")
            {
                while (true)
                {
                    configLine = strReader.ReadLine();
                    if (configLine == "</vehicles>")
                    {
                        return;
                    }

                    String[] strlist = configLine.Split(':');
                    vehicleModels.Add(strlist[1]);
                    vehicles.Add(new MenuItem(strlist[0]));
                }
            }
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

            if (blipEnabled)
            {
                if (Game.Player.Character.Position.DistanceToSquared(GetBlipCoords(blip)) < 3000)
                {
                    SetBlipRoute(blip, false);
                    RemoveBlip(ref blip);
                }
            }

            await Delay(100);
        }

        private async Task spawnVehicle(String model)
        {
            var hash = (uint) GetHashKey(model);
            if (!IsModelInCdimage(hash) || !IsModelAVehicle(hash))
            {
                Notify("Oeps! Dit voertuig bestaat helaas niet!", TEAMHOOFDWEGENRP, GIERIGENARROGANT);
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

        private void Inmelden(string resourceName)
        {
            menu.RemoveMenuItem(inmelden);
            menu.RemoveMenuItem(discord);
            menu.AddMenuItem(calloutsMenuButton);
            MenuController.BindMenuItem(menu, calloutsMenu, calloutsMenuButton);

            menu.AddMenuItem(voertuigenMenuButton);
            MenuController.BindMenuItem(menu, voertuigenMenu, voertuigenMenuButton);

            menu.AddMenuItem(discord);
            menu.AddMenuItem(noodknop);

            RegisterCommand("stcallout",
                new Action<int, List<object>, string>((source, args, raw) =>
                {
                    TriggerServerEvent("SV:Callout", GetPlayerFromServerId(source));
                }), false);

            RegisterCommand("noodknop",
                new Action<int, List<object>, string>((source, args, raw) =>
                {
                    TriggerServerEvent("SV:GetNoodknopIngedrukt", DateTime.UtcNow.ToString());
                    Notify("De route is toegevoegd aan je GPS!", "Assistentie Collega",TEAMHOOFDWEGENRP);
                }), false);
        }

        private void Notify(string message, string title, string subtitle)
        {
            SetNotificationTextEntry("STRING");
            AddTextComponentString(message);
            SetNotificationMessage("CHAR_CALL911", "CHAR_CALL911", false, 0, title, subtitle);
            DrawNotification(true, false);
        }

        private void playSound(string file, double vol)
        {
            TriggerEvent("Client:SoundToClient", file, vol);
        }

        private void NoodknopGPS(Vector3 v)
        {
            blip = AddBlipForCoord(v.X, v.Y, v.Z);
            SetBlipColour(blip, 58);
            SetBlipRouteColour(blip, 58);
            SetBlipRoute(blip, true);
            blipEnabled = true;
        }
    }
}