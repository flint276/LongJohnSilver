using System;
using System.Threading.Tasks;
using Discord.Commands;
using LongJohnSilver.MethodsKnockout;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands.Knockout.Creation
{
    public class RemoveContender : ModuleBase<SocketCommandContext>
    {
        [Command("remove")]
        public async Task RemoveContenderAsync([Remainder]string input = "")
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
                    await Context.Channel.SendMessageAsync(":x: This knockout has already started! No more changes!");
                    return;
                case KnockoutStatus.KnockoutFinished:
                    await Context.Channel.SendMessageAsync(":x: This knockout is finished, please feel free to create a new one!");
                    return;
                case KnockoutStatus.KnockoutUnderConstruction:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var modResult = kModel.DeleteContender(input);

            switch (modResult)
            {                                    
                case ModificationResult.ValueIsEmpty:
                    await Context.Channel.SendMessageAsync(":x: No Value Entered!");
                    return;
                case ModificationResult.Success:
                    await Context.Channel.SendMessageAsync($"You have removed **{input}** as a contender!");
                    return;
                case ModificationResult.Missing:
                    await Context.Channel.SendMessageAsync($":x: There is no contender called **{input}**!");
                    return;
                case ModificationResult.IllegalCharacter:
                case ModificationResult.Duplicate:                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
