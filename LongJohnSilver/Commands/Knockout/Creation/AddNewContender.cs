using System.Threading.Tasks;
using Discord.Commands;
using LongJohnSilver.Database;
using LongJohnSilver.MethodsKnockout;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands.Knockout.Creation
{
    public class AddNewContender : ModuleBase<SocketCommandContext>
    {
        [Command("add")]
        public async Task AddKnockoutAsync([Remainder]string input = "")
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

            if (input.Contains("/"))
            {
                await Context.Channel.SendMessageAsync(":x: I told you that you couldn't choose Face/Off! (or whatever other film you've found with a / in it. V/H/S maybe...)");
                return;
            }

            var kModel = new KnockoutModel(Context);

            if (kModel.GameChannel == 0)
            {
                await Context.Channel.SendMessageAsync(":x: You are not making a knockout at the moment!");
                return;
            }

            switch (kModel.KnockoutStatus)
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
               
            kModel.AddNewContender(input);

            await Context.Channel.SendMessageAsync($"You have added the contender **{input}**");
        }
    }
}
