using System;
using System.Collections.Generic;
using System.Text;
using Discord.Commands;
using Discord.WebSocket;
using LongJohnSilver.Database;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Extensions
{
    public static class SocketCommandContextExtensions
    {
        public static bool IsGuildAdmin(this SocketCommandContext context)
        {
            // Bot moderator permissions for RedFlint on my normal guild.
            if (context.IsRedFlintOnMyChannels()) return true;

            // Return true if user has Admin Permissions
            return context.User is SocketGuildUser currentUser && currentUser.GuildPermissions.Administrator;
        }

        /// <summary>
        /// Works on the assumption the Channel Ops have Kick Permissions but are not Administrators
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool IsChannelOp(this SocketCommandContext context)
        {
            // Bot moderator permissions for RedFlint on my normal guild.
            if (context.IsRedFlintOnMyChannels()) return true;

            // Return true if user has Kick Permissions
            return context.User is SocketGuildUser currentUser && currentUser.GuildPermissions.KickMembers;
        }

        public static bool IsKnockoutChannel(this SocketCommandContext context)
        {
            var roleHandler = new RoleHandler(Factory.GetDatabase());
            return roleHandler.GetRoleForChannel(context.Channel.Id) == "knockout";
        }

        public static bool IsGamingChannel(this SocketCommandContext context)
        {
            var roleHandler = new RoleHandler(Factory.GetDatabase());
            return roleHandler.GetRoleForChannel(context.Channel.Id) == "gaming";
        }

        public static bool IsGeneralChannel(this SocketCommandContext context)
        {
            var roleHandler = new RoleHandler(Factory.GetDatabase());
            return roleHandler.GetRoleForChannel(context.Channel.Id) == "general";
        }

        public static bool IsDraftChannel(this SocketCommandContext context)
        {
            var roleHandler = new RoleHandler(Factory.GetDatabase());
            return roleHandler.GetRoleForChannel(context.Channel.Id) == "draft";
        }

        private static bool IsRedFlintOnMyChannels(this SocketCommandContext context)
        {
            return (context.Guild.Id != 401050736399482890 && context.User.Id == 402715727444049931);

        }
    }
}
