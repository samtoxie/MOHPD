using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOHPDServer.Callouts;

namespace MOHPDServer
{
    public class MOHPDServer : BaseScript
    {
        private readonly string TEAMHOOFDWEGENRP = "TeamHoofdWegenRP";
        private readonly string MELDING = "Melding";
        private readonly string GIERIGENARROGANT = "Gul en Vredelievend";
        private readonly string MELDING_VTB = "Melding: VTB";
        private readonly List<string>DISCIPLINES = new List<string>() {"Politie", "Marechaussee", "Brandweer", "Ambulance"};

        
        private static ArrayList polRepository = new ArrayList();
        private static ArrayList kmarRepository = new ArrayList();
        private static ArrayList brwRepository = new ArrayList();
        private static ArrayList ambuRepository = new ArrayList();
        private static ArrayList beschikbaar = new ArrayList();
        

        public static int[] dispatchColors = new[] {52, 113, 235};
        private static string dispatchText = "[Meldkamer POL]";
        private Vector3 mostRecentNoodknop;
        private DateTime mostRecentNoodknopTime;

        public MOHPDServer()
        {
            EventHandlers["SV:Inmelden"] += new Action<Player, int>(SvInmelden);
            EventHandlers["SV:VTB"] += new Action<Player>(SvVTB);
            EventHandlers["SV:FluitjePls"] += new Action<Player>(SvFluitjePls);
            EventHandlers["SV:Discord"] += new Action<Player>(SvDiscord);
            EventHandlers["SV:NoodknopIngedrukt"] += new Action<Player, Vector3>(SvNoodknopIngedrukt);
            EventHandlers["SV:GetNoodknopIngedrukt"] += new Action<Player, string>(SvGetNoodknopIngedrukt);
            EventHandlers["SV:Backup"] += new Action<Player,int,int>(SvBackup);
            EventHandlers["SV:UpdateBeschikbaar"] += new Action<Player>(SvUpdateBeschikbaar);
            EventHandlers["playerDropped"] += new Action<Player, string>(OnPlayerDropped);
            Tick += OnTick;
        }

        private void SvFluitjePls([FromSource] Player player)
        {
            TriggerEvent("Server:SoundToRadius", player.Character.NetworkId, 50.0f, "fluit", 0.5f);
        }

        private void SvInmelden([FromSource] Player player, int service)
        {
            switch (service)
            {
                case 0:
                    if (polRepository.Contains(player))
                    {
                        player.TriggerEvent("CL:Notify", "U bent reeds ingemeld!", $"~r~{TEAMHOOFDWEGENRP}", GIERIGENARROGANT);
                    }
                    else
                    {
                        polRepository.Add(player);
                        player.TriggerEvent("CL:Notify", "U bent succesvol ingemeld, fijne dienst!", $"~b~{TEAMHOOFDWEGENRP}",
                            GIERIGENARROGANT);
                    }
                    break;
                case 1:
                    if (kmarRepository.Contains(player))
                    {
                        player.TriggerEvent("CL:Notify", "U bent reeds ingemeld!", $"~r~{TEAMHOOFDWEGENRP}", GIERIGENARROGANT);
                    }
                    else
                    {
                        kmarRepository.Add(player);
                        player.TriggerEvent("CL:Notify", "U bent succesvol ingemeld, fijne dienst!", $"~b~{TEAMHOOFDWEGENRP}",
                            GIERIGENARROGANT);
                    }             
                    break;
                case 2:
                    if (brwRepository.Contains(player))
                    {
                        player.TriggerEvent("CL:Notify", "U bent reeds ingemeld!", $"~r~{TEAMHOOFDWEGENRP}", GIERIGENARROGANT);
                    }
                    else
                    {
                        brwRepository.Add(player);
                        player.TriggerEvent("CL:Notify", "U bent succesvol ingemeld, fijne dienst!", $"~b~{TEAMHOOFDWEGENRP}",
                            GIERIGENARROGANT);
                    }         
                    break;
                case 3:   
                    if (ambuRepository.Contains(player))
                    {
                        player.TriggerEvent("CL:Notify", "U bent reeds ingemeld!", $"~r~{TEAMHOOFDWEGENRP}", GIERIGENARROGANT);
                    }
                    else
                    {
                        ambuRepository.Add(player);
                        player.TriggerEvent("CL:Notify", "U bent succesvol ingemeld, fijne dienst!", $"~b~{TEAMHOOFDWEGENRP}",
                            GIERIGENARROGANT);
                    }
                    break;
            }
        }

        private async Task OnTick()
        {
            await Delay(100);
        }
        
        private void SvUpdateBeschikbaar([FromSource] Player player)
        {
            if (beschikbaar.Contains(player)) beschikbaar.Remove(player);
            else beschikbaar.Add(player);
            string status = beschikbaar.Contains(player) ? "beschikbaar" : "niet beschikbaar";
            player.TriggerEvent("CL:Notify", $"Je bent nu {status} voor meldingen.", TEAMHOOFDWEGENRP,
                GIERIGENARROGANT);
        }

        private void SvVTB([FromSource] Player source)
        {
            Callout newCallout = new VTB();
            foreach (Player player in polRepository)
            {
                if (!beschikbaar.Contains(player)) continue;
                player.TriggerEvent("CL:Notify", newCallout.GetCalloutNotification(), MELDING_VTB, TEAMHOOFDWEGENRP);
                player.TriggerEvent("CL:PlaySound", "meldingVTB", 1.0f);
            }
        }

        private void SvDiscord([FromSource] Player source)
        {
            source.TriggerEvent("CL:Notify", "~b~Discord: ~w~https://discord.gg/WTBWBNv", $"~b~{TEAMHOOFDWEGENRP}",
                GIERIGENARROGANT);
        }

        private void SvBackup([FromSource] Player source, int gevraagdeDienst, int prioriteit)
        {
            int vragendeDienst = 4;
            if (polRepository.Contains(source)) vragendeDienst = 0;
            else if (kmarRepository.Contains(source)) vragendeDienst = 1;
            else if (brwRepository.Contains(source)) vragendeDienst = 2;
            else if (ambuRepository.Contains(source)) vragendeDienst = 3;

            bool zelfdeDienst = gevraagdeDienst == vragendeDienst;
            string message;
            string meldkamer = $"MK {DISCIPLINES[gevraagdeDienst]}";
            string prio;

            if (zelfdeDienst)
            {
                prio = $"P{prioriteit} Assistentie Collega";
                message = $"Collega '{source.Name}' vraagt om assistentie van een collega met prioriteit {prioriteit}";
            }
            else
            {
                prio = $"P{prioriteit} Assistentie {DISCIPLINES[vragendeDienst]}";
                message = $"De {DISCIPLINES[vragendeDienst]} vraagt om assistentie van de {DISCIPLINES[gevraagdeDienst]}, graag P{prioriteit} aanrijden naar {source.Name}.";
            }
            switch (gevraagdeDienst)
            {
                case 0:
                    foreach (Player player in polRepository)
                    {
                        if (!player.Equals(source) && beschikbaar.Contains(player))
                        {
                            player.TriggerEvent("CL:Notify", message, meldkamer, prio);
                        }
                    }
                    break;
                case 1:
                    foreach (Player player in kmarRepository)
                    {
                        if (!player.Equals(source) && beschikbaar.Contains(player))
                        {
                            player.TriggerEvent("CL:Notify", message, meldkamer, prio);
                        }
                    }
                    break;
                case 2:
                    foreach (Player player in brwRepository)
                    {
                        if (!player.Equals(source) && beschikbaar.Contains(player))
                        {
                            player.TriggerEvent("CL:Notify", message, meldkamer, prio);
                        }
                    }
                    break;
                case 3:
                    foreach (Player player in ambuRepository)
                    {
                        if (!player.Equals(source) && beschikbaar.Contains(player))
                        {
                            player.TriggerEvent("CL:Notify", message, meldkamer, prio);
                        }
                    }
                    break;
            }
            string temp = zelfdeDienst ? "collega's" : "eenheden";
            source.TriggerEvent("CL:Notify",
                $"De {temp} zijn gealarmeerd naar jouw locatie!",
                meldkamer, prio);
        }
        
        private void SvNoodknopIngedrukt([FromSource] Player source, Vector3 pos)
        {
            mostRecentNoodknop = pos;
            mostRecentNoodknopTime = DateTime.UtcNow;
            string message = " ";
            string meldkamer = " ";

            if (polRepository.Contains(source))
            {
                message = "~r~Noodknop POL ingedrukt~n~~s~Gebruik /noodknop om de gps in te stellen!";
                meldkamer = "Meldkamer POL";
            }
            else if (kmarRepository.Contains(source))
            {
                message = "~r~Noodknop KMAR ingedrukt~n~~s~Gebruik /noodknop om de gps in te stellen!"; 
                meldkamer = "Meldkamer KMAR";
            }

            foreach (Player player in polRepository)
            {
                if (!player.Equals(source))
                {
                    player.TriggerEvent("CL:PlaySound", "panicButton", 1.0f);
                    player.TriggerEvent("CL:Notify", message, "Assistentie Collega", meldkamer);
                }
            }
            foreach (Player player in kmarRepository)
            {
                if (!player.Equals(source))
                {
                    player.TriggerEvent("CL:PlaySound", "panicButton", 1.0f);
                    player.TriggerEvent("CL:Notify", message, "Assistentie Collega", meldkamer);

                }
            }

            source.TriggerEvent("CL:PlaySound", "panicButton", 1.0f);
            source.TriggerEvent("CL:Notify",
                "~r~Noodknop ingedrukt~n~~w~De collega's zijn gealarmeerd naar jouw locatie!",
                "Assistentie Collega", TEAMHOOFDWEGENRP);
        }

        private void SvGetNoodknopIngedrukt([FromSource] Player source, string d)
        {
            if (mostRecentNoodknop != null && mostRecentNoodknopTime != null)
            {
                TimeSpan diff = Convert.ToDateTime(d).Subtract(mostRecentNoodknopTime);
                if (diff.TotalSeconds <= 90)
                {
                    source.TriggerEvent("CL:SetGPS", mostRecentNoodknop);
                    return;
                }
            }

            source.TriggerEvent("CL:Notify", "Er is momenteel geen noodknop actief!", "Assistentie Collega",
                TEAMHOOFDWEGENRP);
        }

        private void OnPlayerDropped([FromSource] Player player, string reason)
        {
            if (polRepository.Contains(player)) polRepository.Remove(player);
            if (kmarRepository.Contains(player)) polRepository.Remove(player);
            if (brwRepository.Contains(player)) polRepository.Remove(player);
            if (ambuRepository.Contains(player)) polRepository.Remove(player);
        }
    }
}