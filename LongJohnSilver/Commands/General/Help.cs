﻿using System.Threading.Tasks;
using Discord.Commands;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands.General
{
    public class Help : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        public async Task HelpAsync()
        {
            if (!StateChecker.IsKnockoutChannel(Context))
            {
                await Context.Channel.SendMessageAsync(
                    "**!createknockout** - Creates a new knockout, instructions follow in a private message.\n" +
                    "(please don't forget to type **!quit** if you change your mind or the bot will remain locked in creation mode!\n" +
                    "**!vote** *<Up>*/*<Down>* - Vote for the contenders you wish to raise and decrease.\n" +
                    "**!showknockout** - Refresh and show the current knockout at the bottom of the screen.\n" +
                    "**!epitaph** *<eulogy for the fallen* - Use this to type a silly little message to honor the contender you last eliminated.\n" +
                    "**!snap** - Halve the current scores, can only be used by the knockout creator.\n" +
                    "**!newday** - Reset the counters for the hour, can only be used by channel ops.\n" +
                    "**!rebuild** - Completely delete the current or in progress knockout. Only for channel ops! Use with caution!\n" +
                    "Have fun! If I break, please let RedFlint know!"
                    );                
                return;
            }

            if (!StateChecker.IsGeneralChannel(Context))
            {
                await Context.Channel.SendMessageAsync(
                    "**!ver** - Show the latest version of the bot.\n" +
                    "**!weather** *<Location>* - Show a simple weather report for that location.\n" +
                    "Have fun! If I break, please let RedFlint know!"
                    );
                return;
            }
        }
    }
}
