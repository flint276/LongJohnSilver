using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;
using Discord.Commands;
using LongJohnSilver.MethodsKnockout;

namespace LongJohnSilver.Embeds
{
    public struct KnockoutIsOverDataStruct
    {
        public string Title;
        public List<string> ContenderList;
        public IMessageChannel ChannelToSendTo;
    }

    public static class KnockoutIsOverDataBuilder
    {
        public static KnockoutIsOverDataStruct BuildData(SocketCommandContext context, KnockoutModel kModel)
        {
            return new KnockoutIsOverDataStruct
            {
                Title = kModel.KnockoutName,
                ContenderList = kModel.AllContendersWithScore.OrderByDescending(x => x.Value).Select(x => x.Key).ToList(),
                ChannelToSendTo = context.Client.GetChannel(kModel.GameChannel) as IMessageChannel
            };
        }
    }    
}
