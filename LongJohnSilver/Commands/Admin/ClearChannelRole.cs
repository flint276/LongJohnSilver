﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using LongJohnSilver.Database;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands.Admin
{
    public class ClearChannelRole : ModuleBase<SocketCommandContext>
    {
        // ReSharper disable once StringLiteralTypo
        [Command("clearchannelrole")]
        public async Task ClearChannelRoleAsync()
        {
            if (!StateChecker.IsGuildAdmin(Context))
            {
                await Context.Channel.SendMessageAsync(":x: You are not guild admin!");
                return;
            }

            if (StateChecker.IsPrivateMessage(Context))
            {
                await Context.Channel.SendMessageAsync(
                    ":x: Please use this command in the channel you wish to clear the role for");
            }

            var roleHandler = new RoleHandler(Factory.GetDatabase());
            roleHandler.ClearRoleForChannel(Context.Channel.Id);
        }
    }
}
