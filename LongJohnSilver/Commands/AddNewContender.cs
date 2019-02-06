using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using LongJohnSilver.Database;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands
{
    public class AddNewContender : ModuleBase<SocketCommandContext>
    {
        [Command("add")]
        public async Task AddKnockoutAsync([Remainder]string input = "")
        {
            if (!Context.IsPrivate)
            {
                await Context.Channel.SendMessageAsync(":x: Command Only for Knockout Creation and only for use in PM with Bot!");
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
               
            knockouts.AddNewContender(input);

            await Context.Channel.SendMessageAsync($"You have added the contender **{input}**");
        }
    }
}
