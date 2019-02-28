using System;
using System.Threading.Tasks;
using Discord.Commands;
using LongJohnSilver.Enums;
using LongJohnSilver.Database.DataMethodsKnockout;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands.Knockout 
{
    public class NewDay : ModuleBase<SocketCommandContext>
    {
        // ReSharper disable once StringLiteralTypo
        [Command("newday")]
        public async Task NewDayAsync()
        {
            var kModel = KnockoutModel.ForChannel(Context.Channel.Id);

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

            switch (kModel.KnockoutStatus)
            {
                case KnockoutStatus.KnockoutInProgress:
                    break;
                case KnockoutStatus.NoKnockout:
                case KnockoutStatus.KnockoutFinished:
                case KnockoutStatus.KnockoutUnderConstruction:
                    await Context.Channel.SendMessageAsync(":x: No Active Knockout In Progress");
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            kModel.NewDay();
            await Context.Channel.SendMessageAsync("It is a glorious new day. Everyone's turns are reset!");
        }
    }
}
