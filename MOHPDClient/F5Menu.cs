using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using MenuAPI;
using static CitizenFX.Core.Native.API;

namespace MOHPDClient
{
    public class F5Menu : BaseScript
    {
        private static readonly string TEAMHOOFDWEGENRP = "TeamHoofdWegenRP";
        private readonly string MELDING = "Melding";
        private static readonly string GIERIGENARROGANT = "Gul en Vredelievend";
        
        private String config = Function.Call<String>(Hash.LOAD_RESOURCE_FILE, "MOHPD", "config.ini");
        private ArrayList vehicleModels;
        private ArrayList vehicles;

        private Menu menu;
        private MenuItem inmelden;
        private MenuItem discord;
        private MenuItem calloutsMenuButton;
        private MenuItem voertuigenMenuButton;
        private MenuItem noodknop;

        private Menu calloutsMenu;
        private MenuItem startVTB;
        private Menu voertuigenMenu;

        public F5Menu()
        {
            menu = new Menu(TEAMHOOFDWEGENRP, GIERIGENARROGANT);
            inmelden = new MenuItem("Inmelden");
            discord = new MenuItem("Discord", "Link naar de TeamHoofdwegenRP Discord.");
            calloutsMenuButton = new MenuItem("Meldingen", "Hier kunnen meldingen worden gegenereerd.")
                {Label = "→→→"};
            voertuigenMenuButton = new MenuItem("Voertuigen", "Hier kan je een voertuig inspawnen.")
                {Label = "→→→"};
            noodknop = new MenuItem("",
                    "Alleen gebruiken in geval van nood, stuurt een signaal naar alle collega's!")
                {LeftIcon = MenuItem.Icon.WARNING, Label = "~r~Noodknop"};
            
            calloutsMenu = new Menu("Meldingen", GIERIGENARROGANT);
            startVTB = new MenuItem("VTB");
            voertuigenMenu = new Menu("Voertuigen", GIERIGENARROGANT);

            vehicleModels = new ArrayList();
            vehicles = new ArrayList();
            LoadConfig();
            
            MenuController.MenuToggleKey = Control.SelectCharacterMichael;
            MenuController.AddMenu(menu);
            MenuController.AddSubmenu(menu, calloutsMenu);
            MenuController.AddSubmenu(menu, voertuigenMenu);

            menu.AddMenuItem(inmelden);
            menu.AddMenuItem(discord);
            
            calloutsMenu.AddMenuItem(startVTB);
            
            foreach (MenuItem car in vehicles)
            {
                voertuigenMenu.AddMenuItem(car);
            }

            menu.OnItemSelect += (menu, item, index) => ItemSelected(menu, item, index);
            calloutsMenu.OnItemSelect += (menu, item, index) => ItemSelected(menu, item, index);
            voertuigenMenu.OnItemSelect += (menu, item, index) => ItemSelected(menu, item, index);

            EventHandlers["CL:Inmelden"] += new Action<string>(Inmelden);

            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;
        }

        private void ItemSelected(Menu menu, MenuItem item, int index)
        {
            if (menu.Equals(this.menu))
            {
                if (item == inmelden)
                {
                    TriggerServerEvent("SV:Inmelden");
                    TriggerEvent("CL:Inmelden", GetPlayerPed(-1));
                } 
                else if (item == discord)
                {
                    TriggerServerEvent("SV:Discord");
                } 
                else if (item == noodknop)
                {
                    TriggerServerEvent("SV:NoodknopIngedrukt", Game.Player.Character.Position);
                } else if (item == startVTB)
                {
                    TriggerServerEvent("SV:VTB");
                    TriggerServerEvent("SV:Discord");
                }
            }
            else if (menu.Equals(calloutsMenu))
            {
                if (item == startVTB)
                {
                    TriggerServerEvent("SV:VTB");
                }
            }
            else if (menu.Equals(voertuigenMenu))
            {
                foreach (MenuItem m in vehicles)
                {
                    if (item == m)
                    {
                        Common.spawnVehicle(vehicleModels[m.Index] as string);
                    }
                }
            }
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
                }), false);
        }
        
    }
}