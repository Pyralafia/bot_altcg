using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALTCG_bot
{
    internal class AltcgBot
    {
        public static AltcgBot altcgBot;

        private DiscordSocketClient _client;
        private GameManager _gameManager;
        private MessageManager _messageManager;
        private CommandManager _commandManager;
        private ButtonManager _buttonManager;
        private PlayerManager _playerManager;

        public DiscordSocketClient Client { get { return _client; } }
        public GameManager GameManager { get { return _gameManager; } }
        public MessageManager MessageManager {  get { return _messageManager; } }

        public static Task Main(string[] args) => new AltcgBot().MainAsync();

        public async Task MainAsync()
        {
            string token = File.ReadAllText("token.txt");

            altcgBot = this;

            FileManager.LoadData();

            _gameManager = new GameManager();
            _messageManager = new MessageManager();
            _commandManager = new CommandManager();
            _buttonManager = new ButtonManager();
            _playerManager = new PlayerManager();
            _client = new DiscordSocketClient();


            _client.Log += Log;
            _client.Ready += ClientReady;
            _client.MessageReceived += _messageManager.DeleteUnwantedMessage;
            _client.SlashCommandExecuted += _commandManager.SlashCommandHandler;
            _client.ButtonExecuted += _buttonManager.ButtonHandler;

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        public Task ClientReady()
        {
            _commandManager.SetGuild(_client.GetGuild(1076931331419807744)); // Dev ALTCGBot
            //_commandManager.SetGuild(_client.GetGuild(672514170641580061)); // Amongs the Legends
            _commandManager.SetupCommand();

            _messageManager.SetGameChannel(FileManager.storedData.matchChannelId);
            _messageManager.SetChallengeChannel(FileManager.storedData.challengeChannelId);
            _messageManager.LoadGameThreads();

            _playerManager.LoadPlayerList(); 

            return Task.CompletedTask;
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
