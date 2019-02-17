using System.Threading.Tasks;
using Discord.Commands;
using LongJohnSilver.Database;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands.Knockout
{
    public class RemoveContender : ModuleBase<SocketCommandContext>
    {
        [Command("remove")]
        public async Task RemoveContenderAsync([Remainder]string input = "")
        {
            if (!StateChecker.IsPrivateMessage(Context))
            {
                return;
            }

            if (input == "")
            {
                await Context.Channel.SendMessageAsync(":x: No Value Entered!");
                return;
            }

            var channelId = KnockOutHandler.ChannelForUser(Context.User.Id, Factory.GetDatabase());

            if (channelId == 0)
            {
                await Context.Channel.SendMessageAsync(":x: You are not making a knockout at the moment!");
                return;
            }

            var knockouts = new KnockOutHandler(channelId, Factory.GetDatabase());

            if (knockouts.KnockoutCreatorUlong != Context.User.Id)
            {
                await Context.Channel.SendMessageAsync(":x: You are not making a knockout at the moment!");
                return;
            }

            switch (knockouts.KnockoutStatus)
            {
                case 1:
                    await Context.Channel.SendMessageAsync(":x: No Knockout is being created at the moment!");
                    return;
                case 2:
                    await Context.Channel.SendMessageAsync(":x: This knockout has already started! No more changes!");
                    return;
                case 3:
                    await Context.Channel.SendMessageAsync(":x: This knockout is finished, please feel free to create a new one!");
                    return;
                case 4:
                    break;
                default:
                    await Context.Channel.SendMessageAsync(":x: Right. This shouldn't have happened. Someone call RedFlint.");
                    return;
            }

            if (!knockouts.DeleteContender(input))
            {
                await Context.Channel.SendMessageAsync($":x: No Exact Match Found for **{input}**. Please Try Again");
                return;
            }

            await Context.Channel.SendMessageAsync($"You have removed the contender **{input}**");
        }
    }
}
