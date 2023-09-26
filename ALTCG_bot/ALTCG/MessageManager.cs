using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ALTCG_bot
{
    internal class MessageManager
    {
        private SocketTextChannel _challengeChannel;
        private SocketTextChannel _gameChannel;
        private List<IThreadChannel> _threadList;

        public MessageManager()
        {
            _threadList = new List<IThreadChannel>();
        }

        public void SetGameChannel(ulong id)
        {
            SocketChannel channel = AltcgBot.altcgBot.Client.GetChannel(id);
            _gameChannel =  (SocketTextChannel)channel;
        }
        
        public void SetChallengeChannel(ulong id)
        {
            SocketChannel channel = AltcgBot.altcgBot.Client.GetChannel(id);
            _challengeChannel =  (SocketTextChannel)channel;
        }

        public async void LoadGameThreads()
        {
            IReadOnlyCollection<RestThreadChannel> restThreadList = await _gameChannel.GetActiveThreadsAsync();

            foreach (RestThreadChannel threadChannel in restThreadList)
            {
                _threadList.Add(threadChannel);
            }
        }

        public async Task DeleteUnwantedMessage(SocketMessage message)
        {
            if (message.Author.Id != 1076931081183428609)
            {
                if (message.Channel.Id == _gameChannel.Id || message.Channel.Id == _challengeChannel.Id)
                {
                    await message.DeleteAsync();
                    return;
                }

                foreach (IThreadChannel thread in _threadList)
                {
                    if (message.Channel.Id == thread.Id)
                    {
                        await message.DeleteAsync();
                        break;
                    }
                }
            }
        }

        public async void ChallengeHandler(SocketSlashCommand command)
        {
            SocketGuildUser taggedUser = (SocketGuildUser)command.Data.Options.First().Value;

            string gameId = AltcgBot.altcgBot.GameManager.CreateGame(command.User, taggedUser);

            EmbedBuilder embed = new EmbedBuilder()
                .WithTitle("Nouveau défi")
                .WithDescription($"Vous avez été défié par {command.User.Username}")
                .WithColor(Color.Purple)
                .WithCurrentTimestamp();

            ComponentBuilder component = new ComponentBuilder()
                .WithButton("Accepter", $"altcgBotBtn0001_{gameId}", style:ButtonStyle.Success)
                .WithButton("Refuser", $"altcgBotBtn0002_{gameId}", style:ButtonStyle.Danger);

            await command.RespondAsync(text: taggedUser.Mention,embed:embed.Build(), components:component.Build());
        }

        public async void UpdateChallengeMessage(SocketMessageComponent component, bool isAccepted = true)
        {
            EmbedBuilder embed = new EmbedBuilder()
                .WithTitle($"Le défi a été {(isAccepted ? "accepté" : "refusé")}.")
                .WithColor(Color.Red);

            ComponentBuilder comp = new ComponentBuilder();

            await component.UpdateAsync(x =>
            {
                x.Embed = embed.Build();
                x.Components = comp.Build();
            });
        }

        public async Task CreateGameThread(Game game)
        {
            IMessage message = await _gameChannel.SendMessageAsync($"Match opposant {game.BluePlayer.player.DiscordUser.Mention} et {game.RedPlayer.player.DiscordUser.Mention}");

            SocketThreadChannel thread = await _gameChannel.CreateThreadAsync(
                name: $"Match opposant {game.BluePlayer.player.DiscordUser.Username} et {game.RedPlayer.player.DiscordUser.Username}",
                autoArchiveDuration: ThreadArchiveDuration.OneDay,
                type: ThreadType.PublicThread,
                invitable: false,
                message: message
            );

            game.ThreadId = thread.Id;

            _threadList.Add(thread);
        }

        public async Task SendGameStarterInThread(Game game)
        {
            SocketThreadChannel thread = null;
            IUser userBlue = await Utils.GetUserFromId(game.BluePlayer.id);
            IUser userRed = await Utils.GetUserFromId(game.RedPlayer.id);

            foreach (SocketThreadChannel threadItem in _gameChannel.Threads)
            {
                if (threadItem.Id == game.ThreadId)
                {
                    thread = threadItem;
                    break;
                }
            }

            EmbedBuilder embed = new EmbedBuilder()
                .WithTitle("**RAPPEL : LE BOT DELETE TOUS LES MESSAGES ENVOYEZ DANS LES THREADS**")
                .WithColor(Color.DarkPurple);
            await thread.SendMessageAsync(embed: embed.Build());

            await SendBoard(thread, game.BluePlayer);

            EmbedBuilder status = new EmbedBuilder()
                .WithTitle(game.Turn == GameTurn.BlueTurn ? "Tour du joueur bleu" : "Tour du joueur rouge")
                .WithColor(Color.Gold);
            ComponentBuilder component = new ComponentBuilder()
                .WithButton($"{userBlue.Username} : Voir main", $"altcgBotBtn0003_{game.GetId()}B", style: ButtonStyle.Primary)
                .WithButton($"{userRed.Username} : Voir main", $"altcgBotBtn0004_{game.GetId()}R", style: ButtonStyle.Danger);
            game.StatusMessage = await thread.SendMessageAsync(String.Empty, embed: status.Build(), components: component.Build());

            await SendBoard(thread, game.RedPlayer);
        }

        private async Task SendBoard(SocketThreadChannel thread, GamePlayer player)
        {
            EmbedBuilder embed = new EmbedBuilder()
                .WithTitle($"Plateau {(player.redOrBlue == "B" ? "Bleu" : "Rouge")}")
                .WithColor(player.color)
                .WithImageUrl($"attachment://mBackside.jpg")
                .WithImageUrl($"attachment://mBackside.jpg")
                .WithImageUrl($"attachment://mBackside.jpg")
                .WithThumbnailUrl($"attachment://eBackside.jpg");
            List<FileAttachment> fileList = new List<FileAttachment>();
            fileList.Add(new FileAttachment(Utils.ImagePath("mBackside")));
            fileList.Add(new FileAttachment(Utils.ImagePath("mBackside")));
            fileList.Add(new FileAttachment(Utils.ImagePath("mBackside")));
            fileList.Add(new FileAttachment(Utils.ImagePath("eBackside")));
            player.boardMessage = await thread.SendFilesAsync(fileList, String.Empty, embed: embed.Build());
        }

        public async Task UpdateBoard(RestUserMessage boardMessage)
        {
            Console.WriteLine(boardMessage.Attachments.Count);
        }

        public async Task SendPlayerHand(SocketMessageComponent button, GamePlayer player, string gameId)
        {
            List<FileAttachment> fileList = new List<FileAttachment>();
            ComponentBuilder component = new ComponentBuilder();
            int rowSwitch = player.hand.Count / 2;

            if (player.handMessage == null)
            {
                player.handMessage = button;
            }

            for (int i = 0; i < player.hand.Count; i++)
            {
                fileList.Add(new FileAttachment(Utils.ImagePath(player.hand[i].Path)));
                component.WithButton($"Jouer {player.hand[i].Name}", 
                    $"hand_{gameId}|{i}{player.redOrBlue}", 
                    row: i < rowSwitch ? 0: 1,
                    style: player.hand[i].CardType == CardType.Member ? ButtonStyle.Success : ButtonStyle.Secondary);
            }
            component.WithButton("Passer", $"hand_{gameId}P{player.redOrBlue}", style: ButtonStyle.Danger);

            await button.RespondWithFilesAsync(fileList, ephemeral: true, components:component.Build());
        }

        public async Task UpdateButtonHand(GamePlayer player, string gameId, int cardIndex)
        {
            ComponentBuilder component = new ComponentBuilder();
            int rowSwitch = player.hand.Count / 2;

            for (int i = 0; i < player.hand.Count; i++)
            {
                component.WithButton($"Jouer {player.hand[i].Name}",
                    $"hand_{gameId}|{i}{player.redOrBlue}",
                    row: i < rowSwitch ? 0 : 1,
                    style: player.hand[i].CardType == CardType.Member ? ButtonStyle.Success : ButtonStyle.Secondary,
                    disabled: i == cardIndex);
            }

            await player.handMessage.ModifyOriginalResponseAsync(m =>
            {
                m.Components = component.Build();
            });
        }

        public async Task WrongButton(SocketMessageComponent button)
        {
            await button.RespondAsync("Ce bouton ne vous est pas attribué", ephemeral: true);
        }

        public async Task CantPlayNow(SocketMessageComponent button)
        {
            await button.RespondAsync("Vous ne pouvez faire cela pour le moment", ephemeral: true);
        }
    }
}
