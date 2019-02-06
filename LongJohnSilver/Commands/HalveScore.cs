﻿using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LongJohnSilver.Database;
using LongJohnSilver.Embeds;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands
{
    public class HalveScore : ModuleBase<SocketCommandContext>
    {
        [Command ("snap")]
        public async Task HalveAsync()
        {
            SocketGuildUser CurrentUser = Context.User as SocketGuildUser;
            if (!(CurrentUser.GuildPermissions.KickMembers))
            {
                await Context.Channel.SendMessageAsync(":x: You are not a bot moderator!");
                return;
            }

            if (Context.IsPrivate)
            {
                await Context.Channel.SendMessageAsync("Please use this command in the knockout channel!");
                return;
            }

            var knockouts = new KnockOutHandler(Context.Channel.Id, Factory.GetDatabase());

            switch (knockouts.KnockoutStatus)
            {
                case 1:
                    await Context.Channel.SendMessageAsync(":x: No Active Knockout In Progress");
                    return;
                case 2:
                    break;
                case 3:
                    await Context.Channel.SendMessageAsync(":x: No Active Knockout In Progress");
                    return;
                case 4:
                    await Context.Channel.SendMessageAsync(":x: No Active Knockout In Progress");
                    return;
                default:
                    await Context.Channel.SendMessageAsync(":x: Right. This shouldn't have happened. Someone call RedFlint.");
                    return;
            }

            await Context.Channel.SendMessageAsync("https://i.imgur.com/5NFHKiJ.gif");
            await Context.Channel.SendMessageAsync("**When I'm done, half of the votes will still exist. Perfectly balanced, as all things should be.**");                      
            knockouts.HalveScores();
            await BotEmbeds.ShowKnockout(Context, knockouts);           
        }

    }
}
