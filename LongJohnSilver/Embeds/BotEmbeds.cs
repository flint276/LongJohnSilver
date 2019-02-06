using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LongJohnSilver.Database;

namespace LongJohnSilver.Embeds
{
    static public class BotEmbeds
    {
        private static IMessage lastShowKnockoutSent;

        // Public Display Methods

        public static async Task ShowKnockout(SocketCommandContext context, KnockOutHandler knockoutData)
        {
            await ShowKnockout(context, context.Channel as IMessageChannel, knockoutData);
        }

        public static async Task ShowKnockout(SocketCommandContext context, IMessageChannel channel, KnockOutHandler knockoutData)
        {
            Data_ShowKnockout.Construct(context, knockoutData);

            var knockoutEmbed = EmbedBuilder_ShowKnockout();

            if (lastShowKnockoutSent != null)
            {
                await lastShowKnockoutSent.DeleteAsync();
            }

            lastShowKnockoutSent = await channel.SendMessageAsync("", false, knockoutEmbed);
        }

        public static async Task DraftBeingCreated(SocketCommandContext context, KnockOutHandler knockoutData)
        {
            await DraftBeingCreated(context, context.Channel as IMessageChannel, knockoutData);
        }

        public static async Task DraftBeingCreated(SocketCommandContext context, IMessageChannel channel, KnockOutHandler knockoutData)
        {
            Data_DraftBeingCreated.Construct(context, knockoutData);

            var draftConstructingEmbed = EmbedBuilder_DraftBeingCreated();

            await channel.SendMessageAsync("", false, draftConstructingEmbed);
        }

        public static async Task KnockoutIsOver(SocketCommandContext context, KnockOutHandler knockoutData)
        {
            lastShowKnockoutSent = null;
            await KnockoutIsOver(context, context.Channel as IMessageChannel, knockoutData);            
        }

        public static async Task KnockoutIsOver(SocketCommandContext context, IMessageChannel channel, KnockOutHandler knockoutData)
        {
            Data_KnockoutIsOver.Construct(context, knockoutData);

            var draftKnockoutIsOverEmbed = EmbedBuilder_KnockoutIsOver();

            await channel.SendMessageAsync("", false, draftKnockoutIsOverEmbed);
        }

        // Data Methods

        private static class Data_ShowKnockout
        {
            public static string Title;
            public static List<string> AllLivingContenders;
            public static List<string> AllFallenContenders;
            public static string Username;
            public static string UserAvatar;
            public static string PlayersReadyString;

            public static void Construct(SocketCommandContext context, KnockOutHandler knockoutData)
            {
                var userId = knockoutData.KnockoutCreatorUlong;

                Title = knockoutData.KnockoutTitle;
                AllLivingContenders = knockoutData.AllLivingContendersByScoreOrderList();
                AllFallenContenders = knockoutData.AllFallenContendersByScoreOrderListWithEpitaph();                
                Username = context.Client.GetUser(userId).Username;
                UserAvatar = context.Client.GetUser(userId).GetAvatarUrl();               

                PlayersReadyString = "";
                
                foreach (ulong playerId in knockoutData.AllPlayerIds)
                {
                    if(context.Guild.GetUser(playerId) != null)
                    {
                        if (knockoutData.PlayerWentLastTime(playerId) || knockoutData.TurnsLeftForPlayer(playerId) == 0)
                        {
                            PlayersReadyString += "~~";
                        }

                        PlayersReadyString += context.Guild.GetUser(playerId).Username;
                        if (knockoutData.PlayerWentLastTime(playerId) || knockoutData.TurnsLeftForPlayer(playerId) == 0)
                        {
                            PlayersReadyString += "~~";
                        }
                        PlayersReadyString += ", ";
                    }
                }
                

            }

        }

        private static class Data_DraftBeingCreated
        {
            public static string Username;

            public static void Construct(SocketCommandContext context, KnockOutHandler knockoutData)
            {
                var userId = knockoutData.KnockoutCreatorUlong;

                Username = context.Client.GetUser(userId).Username;
            }

        }

        private static class Data_KnockoutIsOver
        {
            public static string Title;
            public static List<string> ContenderList;

            public static void Construct(SocketCommandContext context, KnockOutHandler knockoutData)
            {
                Title = knockoutData.KnockoutTitle;
                ContenderList = knockoutData.AllContendersByScoreOrderList();
            }
        }

        // Embed Build Methods
        private static Embed EmbedBuilder_DraftBeingCreated()
        {            
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithAuthor("Knockout Being Created!");
            Embed.WithColor(40, 200, 150);
            Embed.WithDescription($"{Data_DraftBeingCreated.Username} is currently creating a new Knockout!");

            return Embed.Build();                        
        }

        private static Embed EmbedBuilder_ShowKnockout()
        {
            // Build up a list of living and fallen string sequences, dividing to accomodate the 1024 char limit.
            int ranking = 0;

            List<string> theLivingFields = new List<string>();
            List<string> theFallenFields = new List<string>();

            string tempLivingString = "";
            string tempFallenString = "";


            foreach (string knockoutText in Data_ShowKnockout.AllLivingContenders)
            {
                ranking += 1;
                tempLivingString = tempLivingString + $"**{ranking})** {knockoutText}\n";
                if (tempLivingString.Count() > 800)
                {
                    theLivingFields.Add(tempLivingString);
                    tempLivingString = "";
                }
            }

            if (tempLivingString != "")
            {
                theLivingFields.Add(tempLivingString);
            }
            

            foreach (string knockoutText in Data_ShowKnockout.AllFallenContenders)
            {
                ranking += 1;
                tempFallenString = tempFallenString + $"**{ranking})** {knockoutText}\n";
                if (tempFallenString.Count() > 800)
                {
                    theFallenFields.Add(tempFallenString);
                    tempFallenString = "";
                }
            }

            if (tempFallenString != "")
            {
                theFallenFields.Add(tempFallenString);
            }


            // Build the Embed
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithAuthor(Data_ShowKnockout.Title);
            Embed.WithColor(40, 200, 150);
            Embed.WithDescription($"**Current Players:** {Data_ShowKnockout.PlayersReadyString}");

            foreach (string fieldText in theLivingFields)
            {
                if (fieldText == theLivingFields.First())
                {
                    Embed.AddField("The Living", theLivingFields.First());
                }
                else
                {
                    Embed.AddField(".", fieldText);
                }
            }

            foreach (string fieldText in theFallenFields)
            {
                if (fieldText == theFallenFields.First())
                {
                    Embed.AddField("The Fallen", fieldText);
                }
                else
                {
                    Embed.AddField(".", fieldText);
                }
            }                    

            Embed.WithFooter($"Knockout brought to you by {Data_ShowKnockout.Username}", Data_ShowKnockout.UserAvatar);

            return Embed.Build();
        }

        private static Embed EmbedBuilder_KnockoutIsOver()
        {
            // Populate the results fields accomodating the 1024 char limit
            int ranking = 0;            
            List<string> theResultsFields = new List<string>();
            string tempResults = "";

            foreach (string knockoutText in Data_KnockoutIsOver.ContenderList)
            {
                ranking += 1;
                
                tempResults = tempResults + $"**{ranking})** {knockoutText}\n";


                if (tempResults.Count() > 800)
                {
                    theResultsFields.Add(tempResults);
                    tempResults = "";
                }
            }

            if (tempResults != "")
            {
                theResultsFields.Add(tempResults);
            }

            // Build the Embed
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithAuthor(Data_KnockoutIsOver.Title);
            Embed.WithColor(40, 200, 150);

            Embed.WithDescription("The Knockout is over!!!");

            foreach (string fieldText in theResultsFields)
            {
                if (fieldText == theResultsFields.First())
                {
                    Embed.AddField("The Final Results", theResultsFields.First());
                }
                else
                {
                    Embed.AddField(".", fieldText);
                }
            }            

            return Embed.Build();
        }

        public static Embed PlayerVotingReportEmbed(string username, string avatarURL, string choiceToAdd, string choiceToSub)
        {
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithAuthor(username, avatarURL);
            Embed.WithColor(40, 200, 150);
            Embed.WithDescription($"+ {choiceToAdd}\n- {choiceToSub}");

            return Embed.Build();
        }
    }
}
