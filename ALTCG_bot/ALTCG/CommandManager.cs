using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ALTCG_bot
{
    internal class CommandManager
    {
        private SocketGuild _guild;

        public void SetGuild(SocketGuild guild) { _guild = guild; }

        public void SetupCommand()
        {
            CreateChallengeSlashCommand();
            SlashCommandBuild("register", "Pour s'inscrire en tant que nouveau joueur à ce jeu incroyable !");
            SlashCommandBuild("join", "Vous rajoute le role pour que le bot puisse vous trouver et vous ping");
            SlashCommandBuild("unjoin", "Vous retire le role et interdit au bot de vous ping");

            //config command
            SlashCommandBuild("set-match-channel", "Set the match channel to this channel");
            SlashCommandBuild("set-challenge-channel", "Set the challenge channel to this channel");
            SlashCommandBuild("set-swap-channel", "Set the swap channel to this channel");
        }

        public async Task SlashCommandHandler(SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                case "play-random":
                    await command.RespondAsync($"Commande non disponible pour le moment, un peu de patience !", ephemeral: true);
                    break;

                case "challenge":
                    AltcgBot.altcgBot.MessageManager.ChallengeHandler(command);
                    break;

                case "register":
                    await command.RespondAsync($"Commande non disponible pour le moment, un peu de patience !", ephemeral: true);
                    break;

                case "join":
                    await command.RespondAsync($"Commande non disponible pour le moment, un peu de patience !", ephemeral: true);
                    break;

                case "unjoin":
                    await command.RespondAsync($"Commande non disponible pour le moment, un peu de patience !", ephemeral: true);
                    break;

                /*
                 * Commandes liées à la config du bot
                 */
                case "set-match-channel":
                    SetMatchChannel(command);
                    break;

                case "set-challenge-channel":
                    SetChallengeChannel(command);
                    break;

                case "set-swap-channel":
                    SetSwapChannel(command);
                    break;

                case "status":
                    await command.RespondAsync("Normallement je devais avoir le status des config du bot mais flemme de faire la commande");
                    break;

                default:
                    break;
            }
        }

        private async void SlashCommandBuild(string name, string description)
        {
            SlashCommandBuilder command = new SlashCommandBuilder();
            command.WithName(name);
            command.WithDescription(description);

            try
            {
                await _guild.CreateApplicationCommandAsync(command.Build());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private async void CreateChallengeSlashCommand()
        {
            SlashCommandBuilder command = new SlashCommandBuilder();
            command.WithName("challenge");
            command.WithDescription("Defi un adversaire de ton choix");
            command.AddOption("user", ApplicationCommandOptionType.User, "La personne defiee", isRequired: true);

            try
            {
                await _guild.CreateApplicationCommandAsync(command.Build());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private async void SetMatchChannel(SocketSlashCommand command)
        {
            if (command.User.Id == 727970687095930991)
            {
                AltcgBot.altcgBot.MessageManager.SetGameChannel(command.Channel.Id);
                FileManager.storedData.matchChannelId = command.Channel.Id;
                FileManager.StoreData();
                await command.RespondAsync("Channel de match set et store", ephemeral: true);
            }
            else
            {
                await command.RespondAsync("Vous n'avez aps l'autorisation d'utiliser cette commande", ephemeral: true);
            }
        }

        private async void SetChallengeChannel(SocketSlashCommand command)
        {
            if (command.User.Id == 727970687095930991)
            {
                AltcgBot.altcgBot.MessageManager.SetChallengeChannel(command.Channel.Id);
                FileManager.storedData.challengeChannelId = command.Channel.Id;
                FileManager.StoreData();
                await command.RespondAsync("Channel de challenge set et store", ephemeral: true);
            }
            else
            {
                await command.RespondAsync("Vous n'avez aps l'autorisation d'utiliser cette commande", ephemeral: true);
            }
        }

        private async void SetSwapChannel(SocketSlashCommand command)
        {
            if (command.User.Id == 727970687095930991)
            {
                FileManager.storedData.imageSwapChannelId = command.Channel.Id;
                FileManager.StoreData();
                await command.RespondAsync("Channel de swap d'image set et store", ephemeral: true);
            }
            else
            {
                await command.RespondAsync("Vous n'avez aps l'autorisation d'utiliser cette commande", ephemeral: true);
            }
        }

        private async void RegisterPlayer(SocketSlashCommand command)
        {

        }
    }
}
