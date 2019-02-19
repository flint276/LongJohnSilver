using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Discord.Commands;

namespace LongJohnSilver.MethodsKnockout
{
    public class KnockoutModel
    {
        private KnockoutData KData;
        private SocketCommandContext Context;
        
        public KnockoutModel(SocketCommandContext context)
        {
            Context = context;
            KData = Context.IsPrivate ? new KnockoutData(KnockoutData.GetGameChannelForUser(Context.User.Id)) : new KnockoutData(context.Channel.Id);
        }

        public ulong GameChannel => KData.GameChannel;
        public int KnockoutStatus => KData.KnockoutStatus;

        public void AddNewContender(string input)
        {
            KData.NewContender = input;
        }
    }
}
