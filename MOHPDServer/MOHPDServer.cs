using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOHPDServer.Callouts;
using MOHPDServer.Data;

namespace MOHPDServer
{
    public class MOHPDServer : BaseScript
    {
        private readonly string TEAMHOOFDWEGENRP = "TeamHoofdWegenRP";
        private readonly string MELDING = "Melding";
        private readonly string GIERIGENARROGANT = "Gul en Vredelievend";
        private readonly string MELDING_VTB = "Melding: VTB";


        private static PoliceDAO policeDao = new PoliceDAO();
        public static int[] dispatchColors = new[] {52, 113, 235};
        private static string dispatchText = "[Meldkamer POL]";
        private Vector3 mostRecentNoodknop;
        private DateTime mostRecentNoodknopTime;

        public MOHPDServer()
        {
            EventHandlers["SV:Inmelden"] += new Action<Player>(SvInmelden);
            EventHandlers["SV:Callout"] += new Action<Player>(SvCallout);
            EventHandlers["SV:VTB"] += new Action<Player>(SvVTB);
            EventHandlers["SV:FluitjePls"] += new Action<Player>(SvFluitjePls);
            EventHandlers["SV:Discord"] += new Action<Player>(SvDiscord);
            EventHandlers["SV:NoodknopIngedrukt"] += new Action<Player, Vector3>(SvNoodknopIngedrukt);
            EventHandlers["SV:GetNoodknopIngedrukt"] += new Action<Player, string>(SvGetNoodknopIngedrukt);
            EventHandlers["playerDropped"] += new Action<Player, string>(OnPlayerDropped);
            Tick += OnTick;
        }

        private void SvFluitjePls([FromSource] Player player)
        {
            TriggerEvent("Server:SoundToRadius", player.Character.NetworkId, 50.0f, "fluit", 0.5f);
        }

        private void SvInmelden([FromSource] Player player)
        {
            if (policeDao.Contains(player))
            {
                player.TriggerEvent("CL:Notify", "U bent reeds ingemeld!", $"~r~{TEAMHOOFDWEGENRP}", GIERIGENARROGANT);
            }
            else
            {
                policeDao.Add(player);
                player.TriggerEvent("CL:Notify", "U bent succesvol ingemeld, fijne dienst!", $"~b~{TEAMHOOFDWEGENRP}",
                    GIERIGENARROGANT);
            }
        }

        private void SvCallout([FromSource] Player source)
        {
            foreach (var player in policeDao)
            {
                player.TriggerEvent("CL:Notify", "je kanker moeder", $"~r~{TEAMHOOFDWEGENRP}", GIERIGENARROGANT);
            }
        }

        private async Task OnTick()
        {
            await Delay(100);
        }

        private void SvVTB([FromSource] Player source)
        {
            Callout newCallout = new VTB();
            foreach (var player in policeDao)
            {
                player.TriggerEvent("CL:Notify", newCallout.GetCalloutNotification(), MELDING_VTB, TEAMHOOFDWEGENRP);
                player.TriggerEvent("CL:PlaySound", "meldingVTB", 1.0f);
            }
        }

        private void SvDiscord([FromSource] Player source)
        {
            source.TriggerEvent("CL:Notify", "~b~Discord: ~w~https://discord.gg/WTBWBNv", $"~b~{TEAMHOOFDWEGENRP}",
                GIERIGENARROGANT);
        }

        private void SvNoodknopIngedrukt([FromSource] Player source, Vector3 pos)
        {
            mostRecentNoodknop = pos;
            mostRecentNoodknopTime = DateTime.UtcNow;
            foreach (var player in policeDao)
            {
                if (!player.Equals(source))
                {
                    player.TriggerEvent("CL:PlaySound", "panicButton", 1.0f);
                    player.TriggerEvent("CL:Notify",
                        "~r~Noodknop ingedrukt~n~~w~Gebruik /noodknop om de gps in te stellen!",
                        "Assistentie Collega", TEAMHOOFDWEGENRP);
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
                    source.TriggerEvent("CL:NoodknopGPS", mostRecentNoodknop);
                    return;
                }
            }
            source.TriggerEvent("CL:Notify", "Er is momenteel geen noodknop actief!", "Assistentie Collega",
                    TEAMHOOFDWEGENRP);
        }

        private void OnPlayerDropped([FromSource] Player player, string reason)
        {
            if (policeDao.Contains(player)) policeDao.Remove(player);
        }
    }
}