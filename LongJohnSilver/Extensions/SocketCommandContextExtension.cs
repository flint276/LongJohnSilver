using System;
using System.Collections.Generic;
using System.Text;
using Discord.Commands;
using Discord.WebSocket;

namespace LongJohnSilver.Extensions
{
    public static class SocketCommandContextExtension
    {
        public static bool IsGuildAdmin(this SocketCommandContext context)
        {
            if (context.IsRedFlintOnMyChannels()) return true;

            return context.User is SocketGuildUser currentUser && currentUser.GuildPermissions.Administrator;
        }

        /// <summary>
        /// Works on the assumption the Channel Ops have Kick Permissions but are not Administrators
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool IsChannelOp(this SocketCommandContext context)
        {
            if (context.IsRedFlintOnMyChannels()) return true;

            return context.User is SocketGuildUser currentUser && currentUser.GuildPermissions.KickMembers;
        }

        /// <summary>
        /// Override for RedFlint on my primary Guild, essentially 'faking' admin/channelop rights.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static bool IsRedFlintOnMyChannels(this SocketCommandContext context)
        {
            return (context.Guild.Id != 401050736399482890 && context.User.Id == 402715727444049931);

        }
    }
}
