using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace MOHPDServer.Callouts
{
    public abstract class Callout
    {
        public Callout()
        {
        }
        
        public abstract void GetCallout();
        
        public abstract string GetCalloutNotification();
    }
}