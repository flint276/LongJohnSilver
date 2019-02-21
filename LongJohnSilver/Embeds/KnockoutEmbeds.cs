using System;
using System.Collections.Generic;
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
                embedText += fieldText;

                if (embedText.Length <= 800) continue;

                embed.AddField(titleText, embedText);
                embedText = "";
                titleText = ".";
            }

            if (embedText != "") embed.AddField(titleText, embedText);
            embedText = "";

            titleText = "The Fallen";

            foreach (var fieldText in embedData.TheFallenFields)
            {
                embedText += fieldText;

                if (embedText.Length <= 800) continue;

                embed.AddField(titleText, embedText);
                embedText = "";
                titleText = ".";
            }

            if (embedText != "") embed.AddField(titleText, embedText);            

            embed.WithFooter($"Knockout brought to you by {embedData.Username}", embedData.UserAvatar);
            
            if (_lastShowKnockoutSent != null)
            {
                await _lastShowKnockoutSent.DeleteAsync();
            }
            
            _lastShowKnockoutSent = await embedData.ChannelToSendTo.SendMessageAsync("", false, embed.Build());
        }
    }
}
