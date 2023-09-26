using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALTCG_bot
{
    internal class GameManager
    {
        private List<Game> _gameList;

        public List<Game> GetAllGames() { return _gameList;  }

        public Game GetGame(string id)
        {
            Game res = null;
            foreach (Game game in _gameList)
            {
                if (id == game.GetId())
                {
                    res = game;
                    break;
                }
            }
            return res;
        }

        public bool GameAlreadyExist(ulong userChallengingId, ulong userChallengedId)
        {
            bool res = true;

            return res;
        }

        public Game GetGameFromThreadId(ulong threadId)
        {
            Game res = null;

            foreach (Game game in _gameList)
            {
                if (threadId == game.ThreadId)
                {
                    res = game;
                    break;
                }
            }

            return res;
        }


        public GameManager()
        {
            _gameList = new List<Game>();
        }

        public string CreateGame(IUser userBlue, IUser userRed)
        {
            Game newGame = new Game(userBlue, userRed);

            _gameList.Add(newGame);
            return newGame.GetId();
        }

        public void ArchiveGame()
        {

        }
    }
}
