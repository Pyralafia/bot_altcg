using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ALTCG_bot
{
    static class Utils
    {
        public static string GetRandomID(int nbChar = 10)
        {
            string[] ingredients = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,1,2,3,4,5,6,7,8,9,0".Split(',');
            string result = "";

            Random rnd = new Random();
            for (int i = 0; i < nbChar; i++)
            {
                result += ingredients[rnd.Next(ingredients.Length)];
            }

            return result;
        }

        public static async Task<IUser> GetUserFromId(ulong id)
        {
            IUser res = await AltcgBot.altcgBot.Client.GetUserAsync(id);

            return res;
        }

        public static string ImagePath(string name)
        {
            return Directory.GetCurrentDirectory() + "/cards/" + name + ".jpg";
        }

        public static int CharToInt(char value)
        {
            return int.Parse(value.ToString());
        }
    }
}
