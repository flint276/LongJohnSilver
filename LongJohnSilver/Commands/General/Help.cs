using System.Threading.Tasks;
using Discord.Commands;
using LongJohnSilver.Extensions;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands.General
{
    public class Help : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        public async Task HelpAsync()
        {
            if (Context.IsKnockoutChannel())
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

            if (Context.IsGeneralChannel())
            {
                await Context.Channel.SendMessageAsync(
                    "**!ver** - Show the latest version of the bot.\n" +
                    "**!weather** *<Location>* - Show a simple weather report for that location.\n" +
                    "Have fun! If I break, please let RedFlint know!"
                    );
                return;
            }

            if (Context.IsGamingChannel())
            {
                await Context.Channel.SendMessageAsync(
                    "**!showgamertags** *<username>* or *<service>* - Show the gamertags for a user or all gamertags for a service.\n" +
                    "**!setgamertag** *<service>* *<gamertag>* - Set your gamertag for a particular service.\n" +
                    "**!removegamertag** *<service>* - Removes your tag against a particular service.\n" +                    
                    "Have fun! If I break, please let RedFlint know!"
                );
                return;
            }
        }
    }
}
