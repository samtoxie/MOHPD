using CitizenFX.Core;

namespace MOHPDServer.Players
{
    public class Hulpverlener
    {
        private Player player;
        private bool status;

        public Hulpverlener(Player player)
        {
            this.player = player;
            this.status = false;
        }

        public bool Status
        {
            get => status;
            set => status = value;
        }

        public Player Player => player;
    }
}