using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALTCG_bot
{
    internal class PlayerManager
    {
        private List<Player> _players;

        public PlayerManager()
        {
            _players = new List<Player>();
        }

        public async void LoadPlayerList()
        {
            List<ulong> playerId = FileManager.LoadPlayerList();
            foreach (ulong id in playerId)
            {
                _players.Add(new Player(await Utils.GetUserFromId(id)));
            }
        }

        public void CreatePlayer()
        {

        }

        public bool CheckPlayer(ulong id)
        {
            return _players.Find(p => p.DiscordUser.Id == id) != null;
        }
    }
}
