using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Commands;
using LongJohnSilver.Database.DataMethodsKnockout;

namespace LongJohnSilver.Embeds
{
    public struct KnockoutVotingReportDataStruct
    {
        public string Username;
        public string AvatarUrl;
        public string ChoiceToAdd;
        public string ChoiceToSub;
        public IMessageChannel ChannelToSendTo;
    }

    public static class KnockoutVotingReportDataBuilder
    {
        public static KnockoutVotingReportDataStruct BuildData(SocketCommandContext context, KnockoutModel kModel, string choiceToAdd, string choiceToSub)
        {
            return new KnockoutVotingReportDataStruct
            {
                Username = context.User.Username,
                AvatarUrl = context.User.GetAvatarUrl(),
                ChoiceToAdd = choiceToAdd,
                ChoiceToSub = choiceToSub,
                ChannelToSendTo = context.Channel
            };
        }
    }
}
