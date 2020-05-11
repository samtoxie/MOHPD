using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using MenuAPI;
using MOHPDServer.Callouts;
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
        private List<string> disciplines;
        
        private Menu menu;
        private MenuListItem inmelden;
        private MenuItem discord;
        private MenuItem calloutsMenuButton;
        private MenuItem voertuigenMenuButton;
        private MenuItem noodknop;
        private MenuCheckboxItem beschikbaar;

        private Menu calloutsMenu;
        private MenuItem startVTB;
        private MenuItem startAutobrand;
        
        private Menu voertuigenMenu;
        
        private Menu spoedBackupMenu;
        private MenuItem spoedBackupMenuItem;
        private MenuItem spoedPol;
        private MenuItem spoedKmar;
        private MenuItem spoedBrw;
        private MenuItem spoedAmbu;

        private Menu backupMenu;
        private MenuItem backupMenuItem;
        private MenuItem regPol;
        private MenuItem regKmar;
        private MenuItem regBrw;
        private MenuItem regAmbu;

        public F5Menu()
        {
            
            menu = new Menu(TEAMHOOFDWEGENRP, GIERIGENARROGANT);
            disciplines = new List<string>() {"Politie", "Marechaussee", "Brandweer", "Ambulance"};
            inmelden = new MenuListItem("Inmelden", disciplines, 0);
            discord = new MenuItem("Discord", "Link naar de TeamHoofdwegenRP Discord.");
            calloutsMenuButton = new MenuItem("Meldingen", "Hier kunnen meldingen worden gegenereerd.")
                {Label = "→→→"};
            voertuigenMenuButton = new MenuItem("Voertuigen", "Hier kan je een voertuig inspawnen.")
                {Label = "→→→"};
            noodknop = new MenuItem("",
                    "Alleen gebruiken in geval van nood, stuurt een signaal naar alle collega's!")
                {LeftIcon = MenuItem.Icon.WARNING, Label = "~r~Noodknop"};
            beschikbaar = new MenuCheckboxItem("Beschikbaar", "Vink dit aan als je beschikbaar bent voor meldingen.", false);
            
            calloutsMenu = new Menu("Meldingen", GIERIGENARROGANT);
            startVTB = new MenuItem("VTB");
            startAutobrand = new MenuItem("Autobrand");

            voertuigenMenu = new Menu("Voertuigen", GIERIGENARROGANT);
            vehicleModels = new ArrayList();
            vehicles = new ArrayList();
            LoadConfig();
            
            spoedBackupMenu = new Menu("P1 Assistentie", "Assistentie met spoed ");
            spoedBackupMenuItem = new MenuItem("P1 Assistentie", "Hier kun je assistentie met spoed aanvragen.");
            spoedPol = new MenuItem("P1 Politie");
            spoedKmar = new MenuItem("P1 Marechaussee");
            spoedBrw = new MenuItem("P1 Brandweer");
            spoedAmbu = new MenuItem("P1 Ambulance");

            backupMenu = new Menu("P2 Assistentie", "Assistentie zonder spoed ");
            backupMenuItem = new MenuItem("P2 Assistentie", "Hier kun je assistentie zonder spoed aanvragen.");
            regPol = new MenuItem("P2 Politie");
            regKmar = new MenuItem("P2 Marechaussee");
            regBrw = new MenuItem("P2 Brandweer");
            regAmbu = new MenuItem("P2 Ambulance");
            
            MenuController.MenuToggleKey = Control.SelectCharacterMichael;
            MenuController.AddMenu(menu);
            MenuController.AddSubmenu(menu, calloutsMenu);
            MenuController.AddSubmenu(menu, voertuigenMenu);
            MenuController.AddSubmenu(menu, backupMenu);
            MenuController.AddSubmenu(menu, spoedBackupMenu);
            spoedBackupMenu.AddMenuItem(spoedPol);
            spoedBackupMenu.AddMenuItem(spoedKmar);
            spoedBackupMenu.AddMenuItem(spoedBrw);
            spoedBackupMenu.AddMenuItem(spoedAmbu);

            backupMenu.AddMenuItem(regPol);
            backupMenu.AddMenuItem(regKmar);
            backupMenu.AddMenuItem(regBrw);
            backupMenu.AddMenuItem(regAmbu);

            menu.AddMenuItem(inmelden);
            menu.AddMenuItem(discord);
            
            calloutsMenu.AddMenuItem(startVTB);
            calloutsMenu.AddMenuItem(startAutobrand);
            
            foreach (MenuItem car in vehicles)
            {
                voertuigenMenu.AddMenuItem(car);
            }

            menu.OnItemSelect += (menu, item, index) => ItemSelected(menu, item, index);
            calloutsMenu.OnItemSelect += (menu, item, index) => ItemSelected(menu, item, index);
            voertuigenMenu.OnItemSelect += (menu, item, index) => ItemSelected(menu, item, index);
            backupMenu.OnItemSelect += (menu, item, index) => ItemSelected(menu, item, index);
            spoedBackupMenu.OnItemSelect += (menu, item, index) => ItemSelected(menu, item, index);
            menu.OnListItemSelect += (menu, listItem, listIndex, itemIndex) =>
                ItemSelected(menu, listItem, listIndex, itemIndex);
            menu.OnCheckboxChange += (menu, item, index, _checked) => checkboxChanged(menu, item, index, _checked);
            
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;
        }

        private void checkboxChanged(Menu menu1, MenuCheckboxItem menuItem, int itemIndex, bool newCheckedState)
        {
            if (menuItem == beschikbaar)
            {
                TriggerServerEvent("SV:UpdateBeschikbaar");
            }
        }

        private void ItemSelected(Menu menu, MenuItem listItem, int listIndex, int itemIndex)
        {
            if (menu.Equals(this.menu))
            {
                if (listItem == inmelden)
                {
                    if (listIndex == 0)
                    {
                        TriggerServerEvent("SV:Inmelden", listIndex);
                        Inmelden(listIndex);
                    }
                    if (listIndex == 1)
                    {
                        TriggerServerEvent("SV:Inmelden", listIndex);
                        Inmelden(listIndex);
                    }
                    if (listIndex == 2)
                    {
                        TriggerServerEvent("SV:Inmelden", listIndex);
                        Inmelden(listIndex);
                    }
                    if (listIndex == 3)
                    {
                        TriggerServerEvent("SV:Inmelden", listIndex);
                        Inmelden(listIndex);
                    }
                }
            }
        }

        private void ItemSelected(Menu menu, MenuItem item, int index)
        {
            if (menu.Equals(this.menu))
            {
                if (item == discord)
                {
                    TriggerServerEvent("SV:Discord");
                } 
                else if (item == noodknop)
                {
                    TriggerServerEvent("SV:NoodknopIngedrukt", Game.Player.Character.Position);
                }
            }
            else if (menu.Equals(calloutsMenu))
            {
                if (item == startVTB)
                {
                    TriggerServerEvent("SV:VTB");
                }
                else if (item == startAutobrand)
                {
                    Autobrand melding = new Autobrand();
                    Common.SetGPS(melding.Locatie);
                    Common.Notify("Autobrand", TEAMHOOFDWEGENRP, GIERIGENARROGANT);
                }
            }
            else if (menu.Equals(voertuigenMenu))
            {
                foreach (MenuItem m in vehicles)
                {
                    if (item == m)
                    {
                        Common.spawnVehicle(vehicleModels[m.Index] as string, true);
                    }
                }
            }
            else if (menu.Equals(spoedBackupMenu))
            {
                int dienst = 4;
                if (item == spoedPol)
                {
                    dienst = 0;
                }
                else if (item == spoedKmar)
                {
                    dienst = 1;
                }
                else if (item == spoedBrw)
                {
                    dienst = 2;
                }
                else if (item == spoedAmbu)
                {
                    dienst = 3;
                }
                TriggerServerEvent("SV:Backup", dienst, 1);
            }
            else if (menu.Equals(backupMenu))
            {
                int dienst = 4;
                if (item == regPol)
                {
                    dienst = 0;
                }
                else if (item == regKmar)
                {
                    dienst = 1;
                }
                else if (item == regBrw)
                {
                    dienst = 2;
                }
                else if (item == regAmbu)
                {
                    dienst = 3;
                }
                TriggerServerEvent("SV:Backup", dienst, 2);
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

        private void Inmelden(int service)
        {
            menu.RemoveMenuItem(inmelden);
            switch (service)
            {
                case 0:
                    menu.AddMenuItem(calloutsMenuButton);
                    menu.AddMenuItem(voertuigenMenuButton);
                    menu.AddMenuItem(beschikbaar);
                    menu.AddMenuItem(backupMenuItem);
                    menu.AddMenuItem(spoedBackupMenuItem);
                    menu.AddMenuItem(noodknop);

                    MenuController.BindMenuItem(menu, calloutsMenu, calloutsMenuButton);
                    MenuController.BindMenuItem(menu, voertuigenMenu, voertuigenMenuButton);
                    MenuController.BindMenuItem(menu, spoedBackupMenu, spoedBackupMenuItem);
                    MenuController.BindMenuItem(menu, backupMenu, backupMenuItem);
                    RegisterCommand("noodknop",
                        new Action<int, List<object>, string>((source, args, raw) =>
                        {
                            TriggerServerEvent("SV:GetNoodknopIngedrukt", DateTime.UtcNow.ToString());
                        }), false);
                    break;
                case 1:
                    menu.AddMenuItem(voertuigenMenuButton);
                    menu.AddMenuItem(beschikbaar);
                    menu.AddMenuItem(backupMenuItem);
                    menu.AddMenuItem(spoedBackupMenuItem);
                    menu.AddMenuItem(noodknop);

                    MenuController.BindMenuItem(menu, calloutsMenu, calloutsMenuButton);
                    MenuController.BindMenuItem(menu, voertuigenMenu, voertuigenMenuButton);
                    MenuController.BindMenuItem(menu, spoedBackupMenu, spoedBackupMenuItem);
                    MenuController.BindMenuItem(menu, backupMenu, backupMenuItem);
                    RegisterCommand("noodknop",
                        new Action<int, List<object>, string>((source, args, raw) =>
                        {
                            TriggerServerEvent("SV:GetNoodknopIngedrukt", DateTime.UtcNow.ToString());
                        }), false);
                    break;
                case 2:
                    menu.AddMenuItem(voertuigenMenuButton);
                    menu.AddMenuItem(backupMenuItem);
                    menu.AddMenuItem(spoedBackupMenuItem);
                    MenuController.BindMenuItem(menu, voertuigenMenu, voertuigenMenuButton);
                    MenuController.BindMenuItem(menu, spoedBackupMenu, spoedBackupMenuItem);
                    MenuController.BindMenuItem(menu, backupMenu, backupMenuItem);
                    break;
                case 3:
                    menu.AddMenuItem(voertuigenMenuButton);
                    menu.AddMenuItem(backupMenuItem);
                    menu.AddMenuItem(spoedBackupMenuItem);
                    MenuController.BindMenuItem(menu, voertuigenMenu, voertuigenMenuButton);
                    MenuController.BindMenuItem(menu, spoedBackupMenu, spoedBackupMenuItem);
                    MenuController.BindMenuItem(menu, backupMenu, backupMenuItem);
                    break;
            }
        }
        
    }
}