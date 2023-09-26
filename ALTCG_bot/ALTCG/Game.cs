using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALTCG_bot
{


    internal class GamePlayer
    {
        public ulong id;
        public Player player;
        public List<Card> deck;
        public List<Card> hand;
        public List<Card> board;
        public Card effectCard;
        public SocketMessageComponent handMessage; //je stock le bouton vu que c'est lui qui me donne accès à la réponse ephemeral
        public RestUserMessage boardMessage;

        public bool havePlayClutch;
        public Color color;
        public string redOrBlue; //B or R only

        public GamePlayer(IUser user, Color color)
        {
            player = new Player(user);
            deck = new List<Card>();
            hand = new List<Card>();
            id = user.Id;
            havePlayClutch = false;

            GenerateTempDeck();
            this.color = color;

            redOrBlue = color == Color.Blue ? "B" : "R";
        }

        public void DrawCard()
        {
            Random random = new Random();
            int index = random.Next(deck.Count);
            hand.Add(deck[index]);
            deck.RemoveAt(index);
        }

        private void GenerateTempDeck()
        {
            for (int i = 0; i < 30; i++)
            {
                if (i%2 == 0)
                {
                    deck.Add(new MemberCard(3, "001"));
                }
                else
                {
                    deck.Add(new MemberCard(4, "002"));
                }
            }
        }
    }

    public enum GameTurn
    {
        BlueTurn, RedTurn, Clutch
    }

    internal class Game
    {
        private string _gameId;
        private ulong _threadId;
        private RestUserMessage _starterMessage;
        private RestUserMessage _statusMessage;
        private GamePlayer _bluePlayer;
        private GamePlayer _redPlayer;
        private GameTurn _turn;
        private int _numberTurn;

        public string GetId() { return _gameId; }
        public GameTurn Turn { get { return _turn; } }

        public ulong ThreadId 
        { 
            get { return _threadId; } 
            set { _threadId = value; } 
        }
        public RestUserMessage StartMessage 
        { 
            get { return _starterMessage; } 
            set { _starterMessage = value; } 
        }

        public GamePlayer BluePlayer { get { return _bluePlayer; } }
        public GamePlayer RedPlayer { get { return _redPlayer; } }
        public RestUserMessage StatusMessage
        {
            get { return _statusMessage; }
            set { _statusMessage = value; }
        }

        public Game(IUser blueUser, IUser redUser)
        {
            _gameId = Utils.GetRandomID();
            _bluePlayer = new GamePlayer(blueUser, Color.Blue);
            _redPlayer = new GamePlayer(redUser, Color.Red);
            _numberTurn = 1;

            if (new Random().Next(1, 2) == 1)
            {
                _turn = GameTurn.BlueTurn;
            }
            else
            {
                _turn = GameTurn.RedTurn;
            }


            for (int i = 0; i < 6; i++)
            {
                _bluePlayer.DrawCard();
                _redPlayer.DrawCard();
            }
        }

        public async Task PlayCard(GamePlayer player, int cardIndex)
        {
            player.board.Add(player.hand[cardIndex]);
            await AltcgBot.altcgBot.MessageManager.UpdateBoard(player.boardMessage);

            NextTurn();
        }

        private void NextTurn()
        {
            if (_numberTurn >= 7)
            {
                _turn = GameTurn.Clutch;
                CheckEndGame();
            }
            else
            {
                if (_turn == GameTurn.BlueTurn)
                {
                    _turn = GameTurn.RedTurn;
                }
                else
                {
                    _turn = GameTurn.BlueTurn;
                }
            }
            _numberTurn++;
        }

        private void CheckEndGame()
        {
            if (_bluePlayer.havePlayClutch && _redPlayer.havePlayClutch)
            {
                int blueScore = CalculatePoint(_bluePlayer);
                int redScore = CalculatePoint(_redPlayer);

                Console.WriteLine($"BlueScore {blueScore}");
                Console.WriteLine($"RedScore  {redScore}");


                if (blueScore > redScore)
                {
                    
                }
                else if (redScore > blueScore)
                {

                }
                else
                {

                }
            }
        }

        private int CalculatePoint(GamePlayer player)
        {
            GamePlayer opponent = player.redOrBlue == "B" ? _redPlayer : _bluePlayer;
            int res = 0;


            foreach (MemberCard card in player.board)
            {
                int modifier = 0;

                foreach (MemberCard opponentCard in opponent.board)
                {
                    bool breaker = false;

                    //check advantage
                    foreach (MemberType type in card.AdvantageType)
                    {
                        if (type == opponentCard.MemberType)
                        {
                            modifier = 2;
                            breaker = true;
                            break;
                        }
                    }

                    //check disadvantage
                    foreach (MemberType type in card.DisadvantageType)
                    {
                        if (type == opponentCard.MemberType)
                        {
                            modifier = -2;
                            breaker = true;
                            break;
                        }
                    }

                    if (breaker)
                    {
                        break;
                    }
                }

                res += card.Force + modifier;
            }

            return res;
        }
    }
}
