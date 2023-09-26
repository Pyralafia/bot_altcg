using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALTCG_bot
{
    internal class ButtonManager
    {
        public async Task ButtonHandler(SocketMessageComponent component)
        {
            MessageManager messageManager = AltcgBot.altcgBot.MessageManager;
            Game game = null;

            string btnId;
            string tagggedUserId = "";
            char lastIdCharacter = component.Data.CustomId[component.Data.CustomId.Length - 1];
            Console.WriteLine(component.Data.CustomId.Substring(0, 5));

            if (component.Data.CustomId.Substring(0, 5) == "hand_")
            {
                btnId = "hand";
                int indexUnderscore = component.Data.CustomId.IndexOf('_');
                int indexBarre = component.Data.CustomId.IndexOf('|');
                game = AltcgBot.altcgBot.GameManager.GetGame(component.Data.CustomId.Substring(indexUnderscore + 1, indexBarre - indexUnderscore - 1));
            }
            else if (lastIdCharacter == 'B' || lastIdCharacter == 'R')
            {
                btnId = component.Data.CustomId.Substring(0, 15);
                game = AltcgBot.altcgBot.GameManager.GetGame(component.Data.CustomId.Substring(16, component.Data.CustomId.Length - 17));
            }
            else if (component.Data.CustomId.Length > 15)
            {
                btnId = component.Data.CustomId.Substring(0, 15);

                try
                {
                    game = AltcgBot.altcgBot.GameManager.GetGame(component.Data.CustomId.Substring(16));
                    tagggedUserId = game.RedPlayer.id.ToString();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"ERROR : {e.Message}");
                }
            }
            else
            {
                btnId = component.Data.CustomId;
            }

            switch (btnId)
            {
                case "altcgBotBtn0001": //challenge accepted
                    if (tagggedUserId == component.User.Id.ToString() && game != null)
                    {
                        Console.WriteLine("Accepted");
                        messageManager.UpdateChallengeMessage(component);
                        await messageManager.CreateGameThread(game);
                        await messageManager.SendGameStarterInThread(game);
                    }
                    else
                    {
                        await messageManager.WrongButton(component);
                    }
                    break;

                case "altcgBotBtn0002": // challenged refused
                    if (tagggedUserId == component.User.Id.ToString())
                    {
                        Console.WriteLine("Refused");
                        messageManager.UpdateChallengeMessage(component, false);
                    }
                    else
                    {
                        await messageManager.WrongButton(component);
                    }
                    break;

                case "altcgBotBtn0003":
                    if (component.User.Id == game.BluePlayer.id)
                    {
                        await messageManager.SendPlayerHand(component, game.BluePlayer, game.GetId());
                    }
                    else
                    {
                        await messageManager.WrongButton(component);
                    }
                    break;

                case "altcgBotBtn0004":
                    if (component.User.Id == game.RedPlayer.id)
                    {
                        await messageManager.SendPlayerHand(component, game.RedPlayer, game.GetId());
                    }
                    else
                    {
                        await messageManager.WrongButton(component);
                    }
                    break;

                case "hand":
                    GamePlayer player = component.Data.CustomId[component.Data.CustomId.Length - 1] == 'b' ? game.BluePlayer : game.RedPlayer;
                    int cardIndex = Utils.CharToInt(component.Data.CustomId[component.Data.CustomId.Length - 2]);
                    
                    if (game.Turn == GameTurn.Clutch
                        || (player.redOrBlue == "B" && game.Turn == GameTurn.BlueTurn) 
                        || (player.redOrBlue == "R" && game.Turn == GameTurn.RedTurn))
                    {
                        game.PlayCard(player, cardIndex);
                    }
                    else
                    {
                        await messageManager.CantPlayNow(component);
                    }
                    
                    break;

                default:
                    Console.WriteLine("Button error : id not found");
                    break;
            }
        }
    }
}
