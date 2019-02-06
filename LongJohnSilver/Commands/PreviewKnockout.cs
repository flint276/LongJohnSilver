﻿using System;
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
    public class PreviewKnockout : ModuleBase<SocketCommandContext>
    {
        [Command("preview")]
        public async Task PreviewKnockoutAsync()
        {            
            if (!Context.IsPrivate)
            {
                await Context.Channel.SendMessageAsync(":x: Command Only for Knockout Creation and only for use in PM with Bot!");
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
                    await Context.Channel.SendMessageAsync(":x: This knockout has already started! Preview in main channel.");
                    return;
                case 3:
                    await Context.Channel.SendMessageAsync(":x: This knockout is finished, see the results in the main channel.");
                    return;
                case 4:
                    break;
                default:
                    await Context.Channel.SendMessageAsync(":x: Right. This shouldn't have happened. Someone call RedFlint.");
                    return;
            }

            await BotEmbeds.ShowKnockout(Context, knockouts);
        }
    }
}
