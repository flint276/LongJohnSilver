using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using LongJohnSilver.Database;
using LongJohnSilver.Embeds;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands
{
    public class BeginKnockout : ModuleBase<SocketCommandContext>
    {
        [Command("begin")]
        public async Task BeginKnockoutAsync()
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

            if (knockouts.ContendersCount < 4)
            {
                await Context.Channel.SendMessageAsync(":x: Knockouts are over when it reaches the Top 3. Please add more Contenders.");
                return;
            }

            if (knockouts.KnockoutTitle == "" || knockouts.KnockoutTitle == "No Knockout In Progress" || knockouts.KnockoutTitle == "Knockout Under Construction")
            {
                await Context.Channel.SendMessageAsync(":x: Please Name your Knockout");
                return;
            }

            knockouts.SetKnockoutToActive();

            await Context.Channel.SendMessageAsync("You're done! Please check in main channel for the knockout!");

            var chnl = Context.Client.GetChannel(knockouts.KnockoutChannelUlong) as Discord.IMessageChannel;
            await chnl.SendMessageAsync("A New Knockout Has Been Created!");

            await BotEmbeds.ShowKnockout(Context, chnl, knockouts);
        }
    }
}
