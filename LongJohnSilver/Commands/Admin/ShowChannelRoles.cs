using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using LongJohnSilver.Database;
using LongJohnSilver.Extensions;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands.Admin
{
    public class ShowChannelRoles : ModuleBase<SocketCommandContext>
    {
        // ReSharper disable once StringLiteralTypo
        [Command("showchannelroles")]
        public async Task ShowChannelRolesAsync()
        {
            if (!Context.IsGuildAdmin())
            {
                await Context.Channel.SendMessageAsync(":x: You are not guild admin!");
                return;
            }

            if (Context.IsPrivate)
            {
                await Context.Channel.SendMessageAsync(
                    ":x: Please use this command in the guild you wish to see the roles for");
            }

            var roleHandler = new RoleHandler(Factory.GetDatabase());
            var allGuildTextChannels = Context.Guild.TextChannels;
            var botUserId = Context.Client.CurrentUser.Id;

            foreach (var guildChannel in allGuildTextChannels)
            {
                var users = guildChannel.Users;
                foreach (var user in users)
                {
                    if (user.Id == botUserId)
                    {
                        var role = roleHandler.GetRoleForChannel(guildChannel.Id);
                        await Context.Channel.SendMessageAsync($"{guildChannel.Name} : {role}");
                    }
                }
            }     
        }        
    }
}
