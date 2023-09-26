using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALTCG_bot
{
    public class XmlData
    {
        public ulong challengeChannelId;
        public ulong matchChannelId;
        public ulong imageSwapChannelId;
    }

    public class XmlCard
    {

    }

    static class FileManager
    {
        public static XmlData storedData = new XmlData();

        public static void LogError(string errorMessage)
        {
            string path = Directory.GetCurrentDirectory() + "/log.txt";

            using (StreamWriter stream = File.CreateText(path))
            {
                stream.WriteLine($"[{DateTime.Now}] : {errorMessage}");
            }
        }

        public static void StoreData()
        {
            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(XmlData));
            string path = Directory.GetCurrentDirectory() + "/data/channels.xml";

            FileStream file = File.OpenWrite(path);

            writer.Serialize(file, storedData);
            file.Close();
        }

        public static void LoadData()
        {
            string path = Directory.GetCurrentDirectory() + "/data/channels.xml";

            if (File.Exists(path))
            {
                using (FileStream stream = File.OpenRead(path))
                {
                    System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(XmlData));

                    storedData = (XmlData)reader.Deserialize(stream);
                }
            }
            else
            {
                storedData = new XmlData();
                Console.WriteLine("ERROR : data not loaded");
            }
        }

        public static void StorePlayer(List<Player> playerList)
        {
            List<ulong> playerId = new List<ulong>();
            string path = Directory.GetCurrentDirectory() + "/data/players.xml";
            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<ulong>));
            FileStream file = File.OpenWrite(path);

            foreach (Player player in playerList)
            {
                playerId.Add(player.DiscordUser.Id);
            }

            writer.Serialize(file, playerId);
            file.Close();
        }

        public static List<ulong> LoadPlayerList()
        {
            List<ulong> playerList;

            string path = Directory.GetCurrentDirectory() + "/data/players.xml";

            if (File.Exists(path))
            {
                using (FileStream stream = File.OpenRead(path))
                {
                    System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<ulong>));

                    playerList = (List<ulong>)reader.Deserialize(stream);
                }
            }
            else
            {
                playerList = new List<ulong>();
            }

            return playerList;
        }
    }
}
