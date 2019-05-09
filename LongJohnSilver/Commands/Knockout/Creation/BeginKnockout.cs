using System.Threading.Tasks;
using Discord.Commands;
using LongJohnSilver.Embeds;
using LongJohnSilver.Enums;
using LongJohnSilver.Database.DataMethodsKnockout;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands.Knockout.Creation
{
    public class BeginKnockout : ModuleBase<SocketCommandContext>
    {
        [Command("begin")]
        public async Task BeginKnockoutAsync()
        {
            var kModel = KnockoutModel.ForUser(Context.User.Id);

            if (!Context.IsPrivate)
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
                    await Context.Channel.SendMessageAsync(":x: This knockout has already started! No more changes!");
                    return;
                case KnockoutStatus.KnockoutFinished:
                    await Context.Channel.SendMessageAsync(":x: This knockout is finished, please feel free to create a new one!");
                    return;
                case KnockoutStatus.KnockoutUnderConstruction:
                    break;
                default:
                    await Context.Channel.SendMessageAsync(":x: Right. This shouldn't have happened. Someone call RedFlint.");
                    return;
            }

            if (kModel.AllContenderNames.Count < 4)
            {
                await Context.Channel.SendMessageAsync(":x: Knockouts are over when it reaches the Top 3. Please add more Contenders.");
                return;
            }

            if (kModel.KnockoutName == "Knockout Under Construction")
            {
                await Context.Channel.SendMessageAsync(":x: Please Name your Knockout");
                return;
            }

            kModel.KnockoutStatus = KnockoutStatus.KnockoutInProgress;

            await Context.Channel.SendMessageAsync("You're done! Please check in main channel for the knockout!");

            if (Context.Client.GetChannel(kModel.GameChannel) is Discord.IMessageChannel channel)
            {
                await channel.SendMessageAsync("A New Knockout Has Been Created!");
            }

            var embedData = ShowKnockoutDataBuilder.BuildData(Context, kModel);
            await KnockoutEmbeds.ShowKnockout(embedData);
        }
    }
}
