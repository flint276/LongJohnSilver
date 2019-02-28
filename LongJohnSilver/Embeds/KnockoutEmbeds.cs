using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace LongJohnSilver.Embeds
{
    public static class KnockoutEmbeds
    {
        private static IMessage _lastShowKnockoutSent;

        public static async Task ShowKnockout(ShowKnockoutDataStruct embedData)
        {
            var embed = new EmbedBuilder();
            embed.WithAuthor(embedData.Title);
            embed.WithColor(40, 200, 150);
            embed.WithDescription($"**Current Players:** {embedData.PlayersReadyString}");

            var embedText = "";
            var titleText = "The Living";

            foreach (var fieldText in embedData.TheLivingFields)
            {
                if ((embedText + fieldText).Length > 1000)
                {
                    embed.AddField(titleText, embedText);
                    embedText = "";
                    titleText = ".";
                }

                embedText += fieldText;
            }

            if (embedText != "") embed.AddField(titleText, embedText);
            embedText = "";

            titleText = "The Fallen";

            foreach (var fieldText in embedData.TheFallenFields)
            {
                if ((embedText + fieldText).Length > 1000)
                {
                    embed.AddField(titleText, embedText);
                    embedText = "";
                    titleText = ".";
                }

                embedText += fieldText;
            }

            if (embedText != "") embed.AddField(titleText, embedText);            

            embed.WithFooter($"Knockout brought to you by {embedData.Username}", embedData.UserAvatar);
            
            if (_lastShowKnockoutSent != null)
            {
                await _lastShowKnockoutSent.DeleteAsync();
            }
            
            _lastShowKnockoutSent = await embedData.ChannelToSendTo.SendMessageAsync("", false, embed.Build());
        }

        public static async Task CreatingKnockout(CreatingKnockoutDataStruct embedData)
        {
            var embed = new EmbedBuilder();
            embed.WithAuthor("Knockout Being Created!");
            embed.WithColor(40, 200, 150);
            embed.WithDescription($"{embedData.Username} is currently creating a new Knockout!");

            await embedData.ChannelToSendTo.SendMessageAsync("", false, embed.Build());
        }

        public static async Task KnockoutIsOver(KnockoutIsOverDataStruct embedData)
        {
            // Populate the results fields accommodating the 1024 char limit
            var ranking = 0;
            var theResultsFields = new List<string>();
            var tempResults = "";

            foreach (var knockoutText in embedData.ContenderList)
            {                
                ranking += 1;

                var tempResultsString = $"**{ranking})** {knockoutText}\n";
                
                if (tempResults.Length + tempResultsString.Length > 1000)
                {
                    theResultsFields.Add(tempResults);
                    tempResults = "";
                };

                tempResults = tempResults + tempResultsString;                
            }

            if (tempResults != "")
            {
                theResultsFields.Add(tempResults);
            }

            // Build the Embed
            var embed = new EmbedBuilder();
            embed.WithAuthor(embedData.Title);
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

            await embedData.ChannelToSendTo.SendMessageAsync("", false, embed.Build());            
        }

        public static async Task VotingReport(KnockoutVotingReportDataStruct embedData)
        {
            var embed = new EmbedBuilder();
            embed.WithAuthor(embedData.Username, embedData.AvatarUrl);
            embed.WithColor(40, 200, 150);
            embed.WithDescription($"+ {embedData.ChoiceToAdd}\n- {embedData.ChoiceToSub}");

            await embedData.ChannelToSendTo.SendMessageAsync("", false, embed.Build());
        }
    }
}
