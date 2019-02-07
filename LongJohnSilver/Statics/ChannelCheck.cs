using System;
using System.Collections.Generic;
using System.Text;
using Discord.Commands;

// TEMPORARY CLASS SPECIFIC ONLY TO CURRENT DISCORD BUILD, NEED TO EXTRACT THIS TO A DATABASE OR JSON SETTING
namespace LongJohnSilver.Statics
{
    public static class ChannelCheck
    {
        public static bool IsKnockoutChannel(SocketCommandContext context)
        {
            if (context.Guild.Id != 401050736399482890) return true;
            return context.Channel.Id == 534073626215841812;
        }
    }
}
