﻿using System;

namespace MOHPDServer.Callouts
{
    public class VTB : Callout
    {
        private string name;
        private int[] validPostals;
        private string[] validPostalsName;
        private int postalFrom;
        private int postalTo;
        
        public VTB() : base()
        {
            this.name = "Ambulance Transport";
            this.validPostals = new int[]
                {5145, 7039, 4023, 3224, 5008, 6020, 5153, 628, 7061, 913, 816, 702, 7054, 2010, 3706, 802, 0};
            this.validPostalsName = new string[]
            {
                "Our Lovely Women Guesthouse", "University Hospital", "LS Medical Center", "Academic Hospital",
                "Ditchspeed MC", "Our Lovely Women Guesthome", "LSMC Policlinic", "Nothern Hospital", "Autismhelpdesk", "Pallet Town Hospital",
                "Lakeshore Hospital", "Fort Knox", "LSMC North", "Schiphol Airport",
                "LS Harbour", "Lakeshore Airfield", "Military Airfield"
            };
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
            int postal = rnd.Next(0, validPostals.Length);
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
            return String.Format("Callout: {0} | From: {1} {2} | Towards: {3} {4}", name, validPostals[postalFrom],
                validPostalsName[postalFrom], validPostals[postalTo], validPostalsName[postalTo]);
        }
    }
}