using CitizenFX.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CitizenFX.Core.Native;
using MenuAPI;
using static CitizenFX.Core.Native.API;

namespace MOHPDClient
{
    public class MOHPDClient : BaseScript
    {
        private String config = Function.Call<String>(Hash.LOAD_RESOURCE_FILE, "MOHPD", "config.ini");
        private String configLine;
        private ArrayList vehicleNames;
        private ArrayList vehicleModels;
        private ArrayList vehicles;
        private StringReader strReader;


        Menu menu = new Menu("TeamHoofdwegenRP","Gierig en Arrogant");
        MenuItem inmelden = new MenuItem("Inmelden","Gierig en Arrogant");
        MenuItem discord = new MenuItem("Discord", "Link naar de TeamHoofdwegenRP Discord.");
        MenuItem calloutsMenuButton = new MenuItem("Meldingen", "Hier kunnen meldingen worden gegenereerd.") {Label = "→→→"};
        MenuItem voertuigenMenuButton = new MenuItem("Voertuigen", "Hier kan je een voertuig inspawnen.") {Label = "→→→"};

        Menu calloutsMenu = new Menu("Meldingen", "Gierig en Arrogant");
        MenuItem startVTB = new MenuItem("VTB");
        
        Menu voertuigenMenu = new Menu("Voertuigen");


        public MOHPDClient()
        {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
            EventHandlers["CL:Inmelden"] += new Action<string>(Inmelden);
            strReader = new StringReader(config);
            vehicleNames = new ArrayList();
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
                    TriggerServerEvent("SV:Inmelden", GetPlayerPed(-1));
                    TriggerEvent("CL:Inmelden", GetPlayerPed(-1));
                }
                if (item == discord)
                {
                    TriggerServerEvent("SV:Discord", GetPlayerPed(-1));
                }
            };
            
            calloutsMenu.AddMenuItem(startVTB);
            
            calloutsMenu.OnItemSelect += (menu, item, index) =>
            {
                if (item == startVTB)
                {
                    TriggerServerEvent("SV:VTB", GetPlayerPed(-1));
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
            configLine = strReader.ReadLine();
            if(configLine != null && configLine == "<vehicles>")
            {
                while(true)
                {
                    configLine = strReader.ReadLine();
                    if(configLine == "</vehicles>")
                    {
                        return;
                    }
                    String[] strlist = configLine.Split(':');
                    vehicleNames.Add(strlist[0]);
                    vehicleModels.Add(strlist[1]);
                    vehicles.Add(new MenuItem(strlist[0]));
                }
            }
        }
        
        private void OnClientResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;
            
            RegisterCommand("inmelden", new Action<int, List<object>, string>((source, args, raw) =>
            {
                TriggerServerEvent("SV:Inmelden", GetPlayerFromServerId(source));
                TriggerEvent("CL:Inmelden", GetPlayerFromServerId(source));
            }), false);
            
            RegisterCommand("discord", new Action<int, List<object>, string>((source, args, raw) =>
            {
                TriggerServerEvent("SV:Discord", GetPlayerFromServerId(source));
            }), false);
        }

        private async Task OnTick()
        {
            if (IsControlPressed(1, 48))
            {
                TriggerServerEvent("SV:FluitjePls");
            }
            await Delay(100);
        }

        private async Task spawnVehicle(String model)
        {
            var hash = (uint) GetHashKey(model);
            if (!IsModelInCdimage(hash) || !IsModelAVehicle(hash))
            {
                TriggerEvent("chat:addMessage", new 
                {
                    color = new[] { 255, 0, 0 },
                    args = new[] { "[TeamHoofdwegenRP]", $"It might have been a good thing that you tried to spawn a {model}. Who even wants their spawning to actually ^*succeed?" }
                });
                return;
            }

            // create the vehicle
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
            
            RegisterCommand("stcallout", new Action<int, List<object>, string>((source, args, raw) =>
            {
                TriggerServerEvent("SV:Callout", GetPlayerFromServerId(source));
            }), false);
            
            RegisterCommand("VTB", new Action<int, List<object>, string>((source, args, raw) =>
            {
                TriggerServerEvent("SV:VTB", GetPlayerFromServerId(source));
            }), false);
        }
    }
}
