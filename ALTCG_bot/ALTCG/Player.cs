using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALTCG_bot
{
    internal class Player
    {
        IUser discordUser;
        bool isChallengeable;

        public Player(IUser user)
        {
            discordUser = user;
            isChallengeable = true;
        }

        public IUser DiscordUser
        {
            get { return discordUser; }
            set { discordUser = value; }
        }

        public string Name { get { return discordUser.Username; } }
    }
}
