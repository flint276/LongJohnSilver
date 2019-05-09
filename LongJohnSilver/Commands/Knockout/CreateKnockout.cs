using System;
using System.Threading.Tasks;
using Discord.Commands;
using LongJohnSilver.Embeds;
using LongJohnSilver.Enums;
using LongJohnSilver.Database.DataMethodsKnockout;
using LongJohnSilver.Extensions;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands.Knockout
{
    public class CreateKnockout : ModuleBase<SocketCommandContext>
    {
        [Command("createknockout")]
        public async Task AddKnockoutAsync()
        {
            var kModel = KnockoutModel.ForChannel(Context.Channel.Id);

            if (!Context.IsKnockoutChannel() || Context.IsPrivate)
            {
                return;
            }            

            switch (kModel.KnockoutStatus)
            {
                case KnockoutStatus.NoKnockout:
                    break;
                case KnockoutStatus.KnockoutInProgress:
                    await Context.Channel.SendMessageAsync(":x: A knockout is already in progress!");
                    return;
                case KnockoutStatus.KnockoutFinished:
                    break;
                case KnockoutStatus.KnockoutUnderConstruction:
                    await Context.Channel.SendMessageAsync(":x: A knockout is already being built by someone, sorry!");
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await Discord.UserExtensions.SendMessageAsync(Context.User, 
                "Commands to create your own Knockout (all commands in this window please):\n\n" +
                "**!name** _The Name Of Your Knockout_\n" +
                "**!add** _The Name of Your Knockout Contender to add (no slashes please! I'm afraid Face/Off is not allowed for now!)_\n" +
                "**!remove** _The Name of a Knockout Contender to delete (case sensitive)_\n" +
                "**!preview** _Preview your Knockout_\n" +
                "**!begin** _Start your Knockout, NO CHANGES CAN BE MADE BEYOND THIS POINT_\n" +
                "**!quit** _Abandon and Delete your Knockout_\n"
                );
            
            kModel.AddNewKnockout(Context.User.Id);

            var embedData = CreatingKnockoutDataBuilder.BuildData(Context, kModel);
            await KnockoutEmbeds.CreatingKnockout(embedData);            
        }
    }
}
