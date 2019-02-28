using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Commands;
using LongJohnSilver.MethodsKnockout;

namespace LongJohnSilver.Embeds
{
    public struct CreatingKnockoutDataStruct
    {
        public string Username;
        public IMessageChannel ChannelToSendTo;
    }

    public static class CreatingKnockoutDataBuilder
    {
        public static CreatingKnockoutDataStruct BuildData(SocketCommandContext context, KnockoutModel kModel)
        {
            var userId = kModel.KnockoutOwner;
            var username = context.Client.GetUser(userId).Username;

            return new CreatingKnockoutDataStruct
            {
                Username = username,
                ChannelToSendTo = context.Channel
            };            
        }
    }
}
