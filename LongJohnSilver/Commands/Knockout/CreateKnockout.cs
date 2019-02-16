using System.Threading.Tasks;
using Discord.Commands;
using LongJohnSilver.Database;
using LongJohnSilver.Embeds;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands.Knockout
{
    public class CreateKnockout : ModuleBase<SocketCommandContext>
    {
        [Command("createknockout")]
        public async Task AddKnockoutAsync()
        {
            if (!ChannelCheck.IsKnockoutChannel(Context))
            {
                return;
            }

            if (Context.IsPrivate)
            {
                //await Context.Channel.SendMessageAsync("Please use this command in the knockout channel!");
                return;
            }

            var knockouts = new KnockOutHandler(Context.Channel.Id, Factory.GetDatabase());

            switch (knockouts.KnockoutStatus)
            {
                case 1:
                    break;
                case 2:
                    await Context.Channel.SendMessageAsync(":x: A knockout is already in progress!");
                    return;
                case 3:
                    break;
                case 4:
                    await Context.Channel.SendMessageAsync(":x: A knockout is already being built by someone, sorry!");
                    return;
                default:
                    await Context.Channel.SendMessageAsync(":x: Right. This shouldn't have happened. Someone call RedFlint.");
                    break;
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
            
            knockouts.CreateNewKnockout(Context.User.Id);

            await BotEmbeds.DraftBeingCreated(Context, knockouts);

            return;                     
        }
    }
}
