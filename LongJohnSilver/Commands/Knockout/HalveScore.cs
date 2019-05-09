using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using LongJohnSilver.Embeds;
using LongJohnSilver.Enums;
using LongJohnSilver.Database.DataMethodsKnockout;
using LongJohnSilver.Extensions;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands.Knockout
{
    public class HalveScore : ModuleBase<SocketCommandContext>
    {
        [Command ("snap")]
        public async Task HalveAsync()
        {
            var kModel = KnockoutModel.ForChannel(Context.Channel.Id);

            if (!Context.IsKnockoutChannel())
            {
                return;
            }

            if (Context.IsPrivate)
            {
                await Context.Channel.SendMessageAsync("Please use this command in the knockout channel!");
                return;
            }

            if (Context.User.Id != kModel.KnockoutOwner)
            {
                await Context.Channel.SendMessageAsync(":x: You are no the creator of this knockout!");
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

            await Context.Channel.SendMessageAsync("https://i.imgur.com/5NFHKiJ.gif");
            await Context.Channel.SendMessageAsync("**When I'm done, half of the votes will still exist. Perfectly balanced, as all things should be.**");                      
            kModel.HalveScores();
            var embedData = ShowKnockoutDataBuilder.BuildData(Context, kModel);
            await KnockoutEmbeds.ShowKnockout(embedData);
        }

    }
}
