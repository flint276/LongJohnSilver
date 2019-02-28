using System;
using System.Threading.Tasks;
using Discord.Commands;
using LongJohnSilver.Embeds;
using LongJohnSilver.Enums;
using LongJohnSilver.Database.DataMethodsKnockout;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands.Knockout.Creation
{
    public class PreviewKnockout : ModuleBase<SocketCommandContext>
    {
        [Command("preview")]
        public async Task PreviewKnockoutAsync()
        {
            var kModel = KnockoutModel.ForUser(Context.User.Id);

            if (!StateChecker.IsPrivateMessage(Context))
            {
                return;
            }

            if (kModel.GameChannel == 0)
            {
                await Context.Channel.SendMessageAsync(":x: You are not making a knockout at the moment!");
                return;
            }

            switch (kModel.KnockoutStatus)
            {
                case KnockoutStatus.NoKnockout:
                    await Context.Channel.SendMessageAsync(":x: No Knockout is being created at the moment!");
                    return;
                case KnockoutStatus.KnockoutInProgress:
                    await Context.Channel.SendMessageAsync(":x: This knockout has already started! Preview in main channel.");
                    return;
                case KnockoutStatus.KnockoutFinished:
                    await Context.Channel.SendMessageAsync(":x: This knockout is finished, see the results in the main channel.");
                    return;
                case KnockoutStatus.KnockoutUnderConstruction:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var embedData = ShowKnockoutDataBuilder.BuildData(Context, kModel);
            await KnockoutEmbeds.ShowKnockout(embedData);
        }
    }
}
