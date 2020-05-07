using System;
using System.Collections;
using System.IO;
using CitizenFX.Core.Native;

namespace MOHPDServer.Callouts
{
    public class VTB : Callout
    {
        private String config = Function.Call<String>(Hash.LOAD_RESOURCE_FILE, "MOHPD", "config.ini");
        private string name;
        private ArrayList locations;
        private int postalFrom;
        private int postalTo;

        public VTB() : base()
        {
            this.name = "VTB";
            this.locations = new ArrayList();
            LoadConfig();
            postalFrom = randomPostal();
            postalTo = randomPostal();
        }

        /// <summary>
        /// used to generate a random postal, it also checks if the postal isn't already used. If it is it will generate
        /// a new random postal.
        /// </summary>
        /// <returns>returns an index in validPostals</returns>
        private int randomPostal()
        {
            Random rnd = new Random();
            int postal = rnd.Next(0, locations.Count);
            try
            {
                postalFrom.Equals(2);
            }
            catch (Exception e)
            {
                return postal;
            }

            if (postalFrom.Equals(postal))
            {
                return randomPostal();
            }

            return postal;
        }

        public override void GetCallout()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gives back the callout information, kinda like a ToString
        /// </summary>
        /// <returns>string calloutNotification</returns>
        public override string GetCalloutNotification()
        {
            return String.Format("~b~Van~w~:  {0}~n~~n~~b~Naar~w~: {1}",locations[postalFrom],
                locations[postalTo]);
        }

        private void LoadConfig()
        {
            StringReader strReader = new StringReader(config);
            String configLine = strReader.ReadLine();
            if (configLine != null)
            {
                while (!configLine.Equals("<hospitals>"))
                {
                    configLine = strReader.ReadLine();
                }
                configLine = strReader.ReadLine();
                while (!configLine.Equals("</hospitals>"))
                {
                    locations.Add(configLine);
                    configLine = strReader.ReadLine();
                }
            }
        }
    }
}