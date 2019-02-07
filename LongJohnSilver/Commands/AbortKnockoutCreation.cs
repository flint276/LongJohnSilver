using System.Threading.Tasks;
using Discord.Commands;
using LongJohnSilver.Database;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands
{
    public class AbortKnockoutCreation : ModuleBase<SocketCommandContext>
    {
        [Command("quit")]
        public async Task AbortKnockoutAsync()
        {
            if (!Context.IsPrivate)
            {
                //await Context.Channel.SendMessageAsync(":x: Command Only for Knockout Creation and only for use in PM with Bot!");
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
                    await Context.Channel.SendMessageAsync(":x: You are not making a knockout at the moment!");
                    return;
                case 2:
                    await Context.Channel.SendMessageAsync(":x: You are not making a knockout at the moment!");
                    return;
                case 3:
                    await Context.Channel.SendMessageAsync(":x: You are not making a knockout at the moment!");
                    return;
                case 4:
                    break;
                default:
                    await Context.Channel.SendMessageAsync(":x: Right. This shouldn't have happened. Someone call RedFlint.");
                    return;
            }

            var chnl = Context.Client.GetChannel(knockouts.KnockoutChannelUlong) as Discord.IMessageChannel;
                        
            knockouts.EmptyDatabase();
            
            await Context.Channel.SendMessageAsync("Database cleared!");

            await chnl.SendMessageAsync("Knockout Creation Aborted By Creator. You are free to create a new knockout.");
        }
    }
}
