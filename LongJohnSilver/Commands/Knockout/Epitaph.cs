using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using LongJohnSilver.Enums;
using LongJohnSilver.Database.DataMethodsKnockout;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands.Knockout
{
    public class Epitaph : ModuleBase<SocketCommandContext>
    {
        [Command("epitaph")]
        public async Task EpitaphAsync([Remainder]string input = "")
        {
            var kModel = KnockoutModel.ForChannel(Context.Channel.Id);

            if (!StateChecker.IsKnockoutChannel(Context) || StateChecker.IsPrivateMessage(Context))
            {
                return;
            }

            if (input == "")
            {
                await Context.Channel.SendMessageAsync(":x: Please enter a valid epitaph.");
                return;
            }

            if (input.Count() > 199)
            {
                await Context.Channel.SendMessageAsync(":x: Epitaph too long!");
                return;
            }

            switch (kModel.KnockoutStatus)
            {
                case KnockoutStatus.NoKnockout:
                    await Context.Channel.SendMessageAsync(":x: No Knockout ongoing. Feel free to start a new one!");
                    return;
                case KnockoutStatus.KnockoutInProgress:
                    break;
                case KnockoutStatus.KnockoutFinished:
                    await Context.Channel.SendMessageAsync(":x: This knockout is finished.");
                    return;
                case KnockoutStatus.KnockoutUnderConstruction:
                    await Context.Channel.SendMessageAsync(":x: This knockout is still under construction! Patience!");
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var result = kModel.WriteEpitaph(Context.User.Id, input);

            if (!result)
            {
                await Context.Channel.SendMessageAsync(":x: You are not eligible to write an epitaph for a contender.");
                return;
            }

            await Context.Channel.SendMessageAsync(":skull: Engraved!");
        }
    }
}
