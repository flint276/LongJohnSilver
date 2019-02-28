using System;
using System.Threading.Tasks;
using Discord.Commands;
using LongJohnSilver.Enums;
using LongJohnSilver.MethodsKnockout;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands.Knockout.Creation
{
    public class AbortKnockoutCreation : ModuleBase<SocketCommandContext>
    {
        [Command("quit")]
        public async Task AbortKnockoutAsync()
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
                case KnockoutStatus.KnockoutInProgress:
                case KnockoutStatus.KnockoutFinished:
                    await Context.Channel.SendMessageAsync(":x: You are not making a knockout at the moment!");
                    return;
                case KnockoutStatus.KnockoutUnderConstruction:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();                    
            }

            kModel.DeleteAllData();

            await Context.Channel.SendMessageAsync("Database cleared!");

            if (Context.Client.GetChannel(kModel.GameChannel) is Discord.IMessageChannel gameChannel)
                await gameChannel.SendMessageAsync("Knockout Creation Aborted By Creator. You are free to create a new knockout.");
        }
    }
}
