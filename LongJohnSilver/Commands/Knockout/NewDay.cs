using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using LongJohnSilver.Database;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands.Knockout 
{
    public class NewDay : ModuleBase<SocketCommandContext>
    {
        // ReSharper disable once StringLiteralTypo
        [Command("newday")]
        public async Task NewDayAsync()
        {
            if (!StateChecker.IsKnockoutChannel(Context) || StateChecker.IsPrivateMessage(Context))
            {
                return;
            }

            if (!(StateChecker.IsChannelOp(Context)))
            {
                await Context.Channel.SendMessageAsync(":x: You are not a bot moderator!");
                return;
            }

            if (Context.IsPrivate)
            {
                await Context.Channel.SendMessageAsync("Please use this command in the knockout channel!");
                return;
            }

            var knockouts = new KnockOutHandler(Context.Channel.Id, Factory.GetDatabase());
            knockouts.NewDay();
            await Context.Channel.SendMessageAsync("It is a glorious new day. Everyone's turns are reset!");
            return;
        }
    }
}
