using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using LongJohnSilver.Database;

namespace LongJohnSilver.Embeds
{
    public static class BotEmbeds
    {
        private static IMessage _lastShowKnockoutSent;

        // Public Display Methods

        public static async Task ShowKnockout(SocketCommandContext context, KnockOutHandler knockoutData)
        {
            await ShowKnockout(context, context.Channel, knockoutData);
        }

        public static async Task ShowKnockout(SocketCommandContext context, IMessageChannel channel, KnockOutHandler knockoutData)
        {
            DataShowKnockout.Construct(context, knockoutData);

            var knockoutEmbed = EmbedBuilder_ShowKnockout();

            if (_lastShowKnockoutSent != null)
            {
                await _lastShowKnockoutSent.DeleteAsync();
            }

            _lastShowKnockoutSent = await channel.SendMessageAsync("", false, knockoutEmbed);
        }

        public static async Task DraftBeingCreated(SocketCommandContext context, KnockOutHandler knockoutData)
        {
            await DraftBeingCreated(context, context.Channel, knockoutData);
        }

        public static async Task DraftBeingCreated(SocketCommandContext context, IMessageChannel channel, KnockOutHandler knockoutData)
        {
            DataDraftBeingCreated.Construct(context, knockoutData);

            var draftConstructingEmbed = EmbedBuilder_DraftBeingCreated();

            await channel.SendMessageAsync("", false, draftConstructingEmbed);
        }

        public static async Task KnockoutIsOver(SocketCommandContext context, KnockOutHandler knockoutData)
        {
            _lastShowKnockoutSent = null;
            await KnockoutIsOver(context, context.Channel, knockoutData);            
        }

        public static async Task KnockoutIsOver(SocketCommandContext context, IMessageChannel channel, KnockOutHandler knockoutData)
        {
            DataKnockoutIsOver.Construct(context, knockoutData);

            var draftKnockoutIsOverEmbed = EmbedBuilder_KnockoutIsOver();

            await channel.SendMessageAsync("", false, draftKnockoutIsOverEmbed);
        }

        // Data Methods

        private static class DataShowKnockout
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
                
                foreach (var playerId in knockoutData.AllPlayerIds)
                {     
                    // The AlwaysDownloadUsers setting should make this check redundant, but keeping it for safety                    
                    if (context.Guild.GetUser(playerId) == null) continue;

                    var username = context.Guild.GetUser(playerId).Username;

                    if (knockoutData.PlayerWentLastTime(playerId) || knockoutData.TurnsLeftForPlayer(playerId) == 0)
                    {
                        PlayersReadyString += $"~~{username}~~, ";
                    }
                    else
                    {
                        PlayersReadyString += $"{context.Guild.GetUser(playerId).Username}, ";
                    }
                }
                

            }

        }

        private static class DataDraftBeingCreated
        {
            public static string Username;

            public static void Construct(SocketCommandContext context, KnockOutHandler knockoutData)
            {
                var userId = knockoutData.KnockoutCreatorUlong;
                Username = context.Client.GetUser(userId).Username;
            }

        }

        private static class DataKnockoutIsOver
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
            var embed = new EmbedBuilder();
            embed.WithAuthor("Knockout Being Created!");
            embed.WithColor(40, 200, 150);
            embed.WithDescription($"{DataDraftBeingCreated.Username} is currently creating a new Knockout!");

            return embed.Build();                        
        }

        private static Embed EmbedBuilder_ShowKnockout()
        {
            // Build up a list of living and fallen string sequences, dividing to accomodate the 1024 char limit.
            var ranking = 0;

            var theLivingFields = new List<string>();
            var theFallenFields = new List<string>();

            var tempLivingString = "";
            var tempFallenString = "";


            foreach (var knockoutText in DataShowKnockout.AllLivingContenders)
            {
                ranking += 1;
                tempLivingString = tempLivingString + $"**{ranking})** {knockoutText}\n";

                if (tempLivingString.Length <= 800) continue;

                theLivingFields.Add(tempLivingString);
                tempLivingString = "";
            }

            if (tempLivingString != "")
            {
                theLivingFields.Add(tempLivingString);
            }
            

            foreach (var knockoutText in DataShowKnockout.AllFallenContenders)
            {
                ranking += 1;
                tempFallenString = tempFallenString + $"**{ranking})** {knockoutText}\n";

                if (tempFallenString.Length <= 800) continue;

                theFallenFields.Add(tempFallenString);
                tempFallenString = "";
            }

            if (tempFallenString != "")
            {
                theFallenFields.Add(tempFallenString);
            }


            // Build the Embed
            var embed = new EmbedBuilder();
            embed.WithAuthor(DataShowKnockout.Title);
            embed.WithColor(40, 200, 150);
            embed.WithDescription($"**Current Players:** {DataShowKnockout.PlayersReadyString}");

            foreach (var fieldText in theLivingFields)
            {
                if (fieldText == theLivingFields.First())
                {
                    embed.AddField("The Living", theLivingFields.First());
                }
                else
                {
                    embed.AddField(".", fieldText);
                }
            }

            foreach (var fieldText in theFallenFields)
            {
                embed.AddField(fieldText == theFallenFields.First() ? "The Fallen" : ".", fieldText);
            }                    

            embed.WithFooter($"Knockout brought to you by {DataShowKnockout.Username}", DataShowKnockout.UserAvatar);

            return embed.Build();
        }

        private static Embed EmbedBuilder_KnockoutIsOver()
        {
            // Populate the results fields accommodating the 1024 char limit
            var ranking = 0;            
            var theResultsFields = new List<string>();
            var tempResults = "";

            foreach (var knockoutText in DataKnockoutIsOver.ContenderList)
            {
                ranking += 1;
                
                tempResults = tempResults + $"**{ranking})** {knockoutText}\n";

                if (tempResults.Length <= 800) continue;

                theResultsFields.Add(tempResults);
                tempResults = "";
            }

            if (tempResults != "")
            {
                theResultsFields.Add(tempResults);
            }

            // Build the Embed
            var embed = new EmbedBuilder();
            embed.WithAuthor(DataKnockoutIsOver.Title);
            embed.WithColor(40, 200, 150);

            embed.WithDescription("The Knockout is over!!!");

            foreach (var fieldText in theResultsFields)
            {
                if (fieldText == theResultsFields.First())
                {
                    embed.AddField("The Final Results", theResultsFields.First());
                }
                else
                {
                    embed.AddField(".", fieldText);
                }
            }            

            return embed.Build();
        }

        public static Embed PlayerVotingReportEmbed(string username, string avatarUrl, string choiceToAdd, string choiceToSub)
        {
            var embed = new EmbedBuilder();
            embed.WithAuthor(username, avatarUrl);
            embed.WithColor(40, 200, 150);
            embed.WithDescription($"+ {choiceToAdd}\n- {choiceToSub}");

            return embed.Build();
        }
    }
}
