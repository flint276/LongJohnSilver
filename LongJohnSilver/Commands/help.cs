using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands
{
    public class Help : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        public async Task HelpAsync()
        {
            if (!ChannelCheck.IsKnockoutChannel(Context))
            {
                return;
            }

            await Context.Channel.SendMessageAsync("To create a knockout, please type **!createknockout** and follow the instructions in PM.\n" +
                "Please don't forget to type **!quit** in PM if you change your mind, this frees up the bot for someone else!\n" +
                "To play, when a vote is in progress, simply type **!vote Option To Vote Up/Option to Vote Down**\n" +
                "I'll try and guess if you don't type the name correctly, but do try your best. If you just type the beginning of the sentence\n" +
                "(and it is a unique choice), I'll chose that! So _'The Assassination'_ should suffice if you have to type _'The Assassination of Jesse James by the Coward Robert Ford'_, for example!\n" +
                "If the current knockout has scrolled off the screen you can type **!showknockout** to see the current standings\n" +
                "Have fun! If I break, please let Redflint know!"
                );
        }

    }
}
