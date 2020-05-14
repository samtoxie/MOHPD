using CitizenFX.Core;

namespace MOHPDClient.Callouts
{
    public abstract class CalloutCS : BaseScript
    {
        public CalloutCS()
        {
        }
        
        public abstract void GetCallout();
        
        public abstract string GetCalloutNotification();
    }
}